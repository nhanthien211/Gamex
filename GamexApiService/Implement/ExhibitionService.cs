using GamexApiService.Interface;
using GamexApiService.Models;
using GamexEntity;
using GamexEntity.Constant;
using GamexRepository;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Linq.Expressions;
using GamexEntity.Enumeration;
using System.Configuration;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GamexApiService.Implement {
    public class ExhibitionService : IExhibitionService {
        private IRepository<Exhibition> _exhibitionRepo;
        private IRepository<ExhibitionAttendee> _exhibitionAttendeeRepo;
        private IUnitOfWork _unitOfWork;

        public ExhibitionService(
            IRepository<Exhibition> exhibitionRepo,
            IRepository<ExhibitionAttendee> exhibitionAttendeeRepo,
            IUnitOfWork unitOfWork) {
            _exhibitionRepo = exhibitionRepo;
            _exhibitionAttendeeRepo = exhibitionAttendeeRepo;
            _unitOfWork = unitOfWork;
        }

        private List<string> GetCheckedInExhibitionIds(string accountId) {
            var checkedIn = _exhibitionAttendeeRepo.GetList(ea =>
                ea.AccountId.Equals(accountId) && ea.CheckinTime != null);
            return checkedIn.Select(e => e.ExhibitionId).ToList();
        }

        public List<ExhibitionShortViewModel> GetExhibitions(string list, string type, int take, int skip,
            string lat, string lng, string accountId) {

            Expression<Func<Exhibition, bool>> filter;
            Expression<Func<Exhibition, DateTime>> sort = e => e.StartDate;
            var queryTake = take;
            var querySkip = skip;

            switch (type) {
                case ExhibitionTypes.Ongoing:
                    filter = e =>
                        e.IsActive && e.StartDate <= DateTime.Now && e.EndDate > DateTime.Now;
                    queryTake = 0;
                    querySkip = 0;
                    sort = null;
                    break;
                case ExhibitionTypes.Upcoming:
                    filter = e => e.IsActive && e.StartDate > DateTime.Now;
                    break;
                case ExhibitionTypes.NearYou:
                    filter = e =>
                        e.IsActive && e.Location.Distance(DbGeography.FromText("POINT(" + lng + " " + lat + ")")) <= (double) DistanceEnum.Near 
                        && e.EndDate >= DateTime.Now;
                    break;
                case ExhibitionTypes.Past:
                    filter = e => e.IsActive && e.EndDate < DateTime.Now;
                    queryTake = 0;
                    querySkip = 0;
                    break;
                default:
                    filter = e => e.IsActive;
                    break;
            }

            var exhibitionList = _exhibitionRepo.GetPagingProjection(
                e => new {
                    e.ExhibitionId,
                    e.Name,
                    e.Address,
                    e.StartDate,
                    e.EndDate,
                    e.Logo,
                    e.Location
                },
                filter,
                sort,
                "asc",
                queryTake,
                querySkip);

            var isCheckedInList = !string.IsNullOrEmpty(list) && list.Equals(ExhibitionLists.CheckedIn);
            if (isCheckedInList) {
                // list == "checked-in" && (type == "ongoing" || type == "past")
                var checkedInExhibitionIds = GetCheckedInExhibitionIds(accountId);
                exhibitionList = exhibitionList.Where(e => checkedInExhibitionIds.Contains(e.ExhibitionId))
                    .Skip(skip).Take(take);
            }

            if (!isCheckedInList && type.Equals(ExhibitionTypes.Ongoing)) {
                var checkedInExhibitionIds = GetCheckedInExhibitionIds(accountId);
                // exclude checked in exhibitions
                exhibitionList = exhibitionList.Where(e => !checkedInExhibitionIds.Contains(e.ExhibitionId))
                    .Skip(skip).Take(take);
            }

            return exhibitionList.Select(e => new ExhibitionShortViewModel() {
                ExhibitionId = e.ExhibitionId,
                Name = e.Name,
                Address = e.Address,
                StartDate = e.StartDate.ToString("f"),
                EndDate = e.EndDate.ToString("f"),
                Logo = e.Logo,
                Latitude = e.Location?.Latitude != null ? e.Location.Latitude.Value.ToString() : "",
                Longitude = e.Location?.Longitude != null ? e.Location.Longitude.Value.ToString() : ""
            }).ToList();
        }

        private bool HasBookmarked(string accountId, string exhibitionId) {
            return _exhibitionAttendeeRepo.GetSingle(ea =>
                       ea.AccountId.Equals(accountId) && ea.ExhibitionId.Equals(exhibitionId)
                                                      && ea.BookmarkDate != null) != null;
        }

        public ExhibitionDetailViewModel GetExhibition(string accountId, string exhibitionId) {
            var exhibition = _exhibitionRepo.GetSingle(
                e => e.IsActive && e.ExhibitionId.Equals(exhibitionId), e => e.Booth.Select(b => b.Company.Booth));
            if (exhibition == null) {
                return null;
            }
            var companies = exhibition.Booth.Select(b => b.Company);     
            return new ExhibitionDetailViewModel {
                ExhibitionId = exhibition.ExhibitionId,
                Name = exhibition.Name,
                Description = exhibition.Description,
                Address = exhibition.Address,
                //OrganizerId = exhibition.OrganizerId,
                StartDate = exhibition.StartDate.ToString("f"),
                EndDate = exhibition.EndDate.ToString("f"),
                Lat = exhibition.Location?.Latitude != null ? exhibition.Location.Latitude.Value.ToString() : "",
                Lng = exhibition.Location?.Longitude != null ? exhibition.Location.Longitude.Value.ToString() : "",
                Logo = exhibition.Logo,
                IsBookmarked = HasBookmarked(accountId, exhibitionId),
                ListCompany = companies.Select(c => new CompanyShortViewModel() {
                    CompanyId = c.CompanyId,
                    Name = c.Name,
                    Logo = c.Logo,
                    Booths = c.Booth.Where(b => b.ExhibitionId == exhibitionId).Select(b => b.BoothNumber).ToArray()
                }).ToList().RemoveRedundancies()
            };
        }

        public ServiceActionResult CheckInExhibition(string accountId, string exhibitionId) {
            var checkin = new ExhibitionAttendee {
                ExhibitionId = exhibitionId,
                AccountId = accountId,
                CheckinTime = DateTime.Now
            };
            var exhibition = _exhibitionRepo.GetById(exhibitionId);

            if (exhibition == null) {
                return new ServiceActionResult {
                    Ok = false,
                    Message = "Check in failed: exhibition not existed!"
                };
            }

            if (checkin.CheckinTime < exhibition.StartDate) {
                return new ServiceActionResult() {
                    Ok = false,
                    Message = "Check in failed: the exhibition hasn't started yet!"
                };
            }
            if (checkin.CheckinTime > exhibition.EndDate) {
                return new ServiceActionResult() {
                    Ok = false,
                    Message = "Check in failed: the exhibition has ended!"
                };
            }

            var exhibitionAttendeeRow = _exhibitionAttendeeRepo.GetSingle(
                ea => ea.ExhibitionId.Equals(exhibitionId) && ea.AccountId.Equals(accountId));
            if (exhibitionAttendeeRow?.CheckinTime != null) {
                return new ServiceActionResult() {
                    Ok = false,
                    Message = "You has already checked in this exhibition!"
                };
            }

            if (exhibitionAttendeeRow == null) {
                _exhibitionAttendeeRepo.Insert(checkin);
            }
            else {
                _exhibitionAttendeeRepo.Update(exhibitionAttendeeRow);
                exhibitionAttendeeRow.CheckinTime = DateTime.Now;
            }
            try {
                var affectedRows = _unitOfWork.SaveChanges();
                if (affectedRows == 1) {
                    return new ServiceActionResult() { Ok = true, Message = "Check in success!" };
                }
            } catch (Exception ex) {
                return ServiceActionResult.ErrorResult;
            }

            return ServiceActionResult.ErrorResult;
        }

        public bool HasCheckedIn(string accountId, string exhibitionId) {
            var exhibitionAttendee = _exhibitionAttendeeRepo.GetSingle(
                ea => ea.AccountId.Equals(accountId) && ea.ExhibitionId.Equals(exhibitionId)
                                                     && ea.CheckinTime != null);
            return exhibitionAttendee != null;
        }

        public bool IsOnGoing(string exhibitionId) {
            var exhibition = _exhibitionRepo.GetById(exhibitionId);
            var now = DateTime.Now;
            return exhibition.StartDate <= now && now <= exhibition.EndDate;
        }

        public List<ExhibitionShortViewModel> GetExhibitionListRouteLengthNear(string lat, string lng, List<ExhibitionShortViewModel> exhibitionList)
       {
            var apiUrl = ConfigurationManager.AppSettings.Get("GoogleDistanceMatrixApiUrl");
            var apiKey = ConfigurationManager.AppSettings.Get("GoogleMapApiKey");
            var destinationArray = exhibitionList.Select(e => new
            {
                Latitude = e.Latitude,
                Longitude = e.Longitude
            }).ToArray().Select(d => string.Join(",", d.Latitude, d.Longitude)).ToArray();
            var destinationAddresses = string.Join("|", destinationArray);
            var requestUrl = $"{apiUrl}?origins={lat},{lng}&destinations={destinationAddresses}&key={apiKey}";
            var request = (HttpWebRequest) WebRequest.Create(requestUrl);
            var response = request.GetResponse();
            var dataStream = response.GetResponseStream();
            var reader = new StreamReader(dataStream);
            var responseString = reader.ReadToEnd();
            response.Close();
            var responseObject = JsonConvert.DeserializeObject<GoogleMapResponse>(responseString);
            for (int i = 0; i < responseObject.Rows[0].Elements.Length; i++)
            {
                if (responseObject.Rows[0].Elements[i].Distance.Value > (double) DistanceEnum.Near)
                {
                    exhibitionList.RemoveAt(i);
                }
            }
            return exhibitionList;
        }
    }
}
