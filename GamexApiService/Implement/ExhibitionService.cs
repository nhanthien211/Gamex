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

        public List<ExhibitionShortViewModel> GetExhibitions(string type, int take, int skip,
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
                    const double range = 5000; // meters
                    filter = e =>
                        e.IsActive && e.Location.Distance(DbGeography.FromText("POINT(" + lng + " " + lat + ")")) <= range;
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
                    e.Logo
                },
                filter,
                sort,
                "asc",
                queryTake,
                querySkip);

            if (type.Equals(ExhibitionTypes.Ongoing)) {
                var checkedInExhibitionIds = GetCheckedInExhibitionIds(accountId);
                if (!string.IsNullOrEmpty(accountId)) {
                    // exclude checked in exhibitions
                    exhibitionList = exhibitionList.Where(e => !checkedInExhibitionIds.Contains(e.ExhibitionId))
                        .Skip(skip).Take(take);
                }
            }

            return exhibitionList.Select(e => new ExhibitionShortViewModel() {
                ExhibitionId = e.ExhibitionId,
                Name = e.Name,
                Address = e.Address,
                StartDate = e.StartDate.ToLongDateString(),
                EndDate = e.EndDate.ToLongDateString(),
                Logo = e.Logo
            }).ToList();
        }

        private bool HasBookmarked(string accountId, string exhibitionId) {
            return _exhibitionAttendeeRepo.GetSingle(ea =>
                       ea.AccountId.Equals(accountId) && ea.ExhibitionId.Equals(exhibitionId)
                                                      && ea.BookmarkDate != null) != null;
        }

        public ExhibitionDetailViewModel GetExhibition(string accountId, string exhibitionId) {
            var exhibition = _exhibitionRepo.GetSingle(
                e => e.IsActive && e.ExhibitionId.Equals(exhibitionId), e => e.Booth.Select(b => b.Company));
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
                StartDate = exhibition.StartDate.ToLongDateString(),
                EndDate = exhibition.EndDate.ToLongDateString(),
                //Lat = exhibition.Location.Latitude?.ToString(),
                //Lng = exhibition.Location.Longitude?.ToString(),
                Logo = exhibition.Logo,
                IsBookmarked = HasBookmarked(accountId, exhibitionId),
                ListCompany = companies.Select(c => new CompanyShortViewModel() {
                    CompanyId = c.CompanyId,
                    Name = c.Name,
                    Logo = c.Logo,
                    Booths = c.Booth.Select(b => b.BoothNumber).ToArray()
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

            if (HasCheckedIn(accountId, exhibitionId)) {
                return new ServiceActionResult() {
                    Ok = false,
                    Message = "You has already checked in this exhibition!"
                };
            }

            _exhibitionAttendeeRepo.Insert(checkin);
            try {
                var affectedRows = _unitOfWork.SaveChanges();
                if (affectedRows == 1) {
                    return new ServiceActionResult() { Ok = true, Message = "Check in success!"};
                }
            } catch (Exception ex) {
                return ServiceActionResult.ErrorResult;
            }

            return ServiceActionResult.ErrorResult;
        }

        public bool HasCheckedIn(string accountId, string exhibitionId) {
            var exhibitionAttendee = _exhibitionAttendeeRepo.GetSingle(
                ea => ea.AccountId.Equals(accountId) && ea.ExhibitionId.Equals(exhibitionId)
                                                     && ea.CheckinTime != null
            );
            return exhibitionAttendee != null;
        }

        public bool IsOnGoing(string exhibitionId) {
            var exhibition = _exhibitionRepo.GetById(exhibitionId);
            var now = DateTime.Now;
            return exhibition.StartDate <= now && now <= exhibition.EndDate;
        }
    }
}
