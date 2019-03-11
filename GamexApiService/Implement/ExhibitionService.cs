using System;
using GamexApiService.Interface;
using GamexApiService.ViewModel;
using GamexEntity;
using GamexRepository;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using GamexEntity.Constant;

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
                ea.AccountId.Equals(accountId));
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
                StartDate = e.StartDate.ToLongDateString(),
                EndDate = e.EndDate.ToLongDateString(),
                Logo = e.Logo
            }).ToList();
        }

        public ExhibitionViewModel GetExhibition(string exhibitionId) {
            var exhibition = _exhibitionRepo.GetSingle(
                e => e.IsActive && e.ExhibitionId.Equals(exhibitionId));
            if (exhibition == null) {
                return null;
            }
            return new ExhibitionViewModel {
                ExhibitionId = exhibition.ExhibitionId,
                Name = exhibition.Name,
                Description = exhibition.Description,
                Address = exhibition.Address,
                OrganizerId = exhibition.OrganizerId,
                StartDate = exhibition.StartDate.ToLongDateString(),
                EndDate = exhibition.EndDate.ToLongDateString(),
                Lat = exhibition.Location.Latitude?.ToString(),
                Lng = exhibition.Location.Longitude?.ToString(),
                Logo = exhibition.Logo
            };
        }
    }
}
