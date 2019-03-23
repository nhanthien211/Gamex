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

        public List<ExhibitionShortViewModel> GetExhibitions(string list, string type, int take, int skip,
            string lat, string lng, string name, string accountId) {

            // filter exhibition by type
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
                        e.IsActive && e.Location.Distance(DbGeography.FromText("POINT(" + lng + " " + lat + ")")) <= range
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

            // search exhibition by name
            if (!string.IsNullOrEmpty(name)) {
                // to combine with other filter rule, toggle the 2 lines of code below
                //filter = filter.And(e => e.Name.Contains(name) && e.EndDate >= DateTime.Now);
                filter = e => e.IsActive && e.EndDate >= DateTime.Now && e.Name.Contains(name);
                querySkip = 0;
                queryTake = 0;
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

            // filter exhibition by user checked in exhibition list
            var isCheckedInList = !string.IsNullOrEmpty(list) && list.Equals(ExhibitionLists.CheckedIn);
            if (isCheckedInList) {
                // list == "checked-in" && (type == "ongoing" || type == "past")
                var checkedInExhibitionIds = GetCheckedInExhibitionIds(accountId);
                exhibitionList = exhibitionList.Where(e => checkedInExhibitionIds.Contains(e.ExhibitionId))
                    .Skip(skip).Take(take);
            }

            if (!isCheckedInList && string.IsNullOrEmpty(name) && type.Equals(ExhibitionTypes.Ongoing)) {
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
                Lat = exhibition.Location.Latitude?.ToString(),
                Lng = exhibition.Location.Longitude?.ToString(),
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
            } else {
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
    }

    public static class LambdaExtensions {
        /// <summary>
        /// Composes the specified left expression.
        /// </summary>
        /// <typeparam name="T">Param Type</typeparam>
        /// <param name="leftExpression">The left expression.</param>
        /// <param name="rightExpression">The right expression.</param>
        /// <param name="merge">The merge.</param>
        /// <returns>Returns the expression</returns>
        public static Expression<T> Compose<T>(this Expression<T> leftExpression, Expression<T> rightExpression, Func<Expression, Expression, Expression> merge) {
            var map = leftExpression.Parameters.Select((left, i) => new {
                left,
                right = rightExpression.Parameters[i]
            }).ToDictionary(p => p.right, p => p.left);

            var rightBody = ExpressionRebinder.ReplacementExpression(map, rightExpression.Body);

            return Expression.Lambda<T>(merge(leftExpression.Body, rightBody), leftExpression.Parameters);
        }

        /// <summary>
        /// Performs an "AND" operation
        /// </summary>
        /// <typeparam name="T">Param Type</typeparam>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>Returns the expression</returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right) {
            return left.Compose(right, Expression.And);
        }

        /// <summary>
        /// Performs an "OR" operation
        /// </summary>
        /// <typeparam name="T">Param Type</typeparam>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>Returns the expression</returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right) {
            return left.Compose(right, Expression.Or);
        }
    }

    public class ExpressionRebinder : ExpressionVisitor {
        /// <summary>
        /// The map
        /// </summary>
        private readonly Dictionary<ParameterExpression, ParameterExpression> map;
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionRebinder"/> class.
        /// </summary>
        /// <param name="map">The map.</param>
        public ExpressionRebinder(Dictionary<ParameterExpression, ParameterExpression> map) {
            this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        /// <summary>
        /// Replacements the expression.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="exp">The exp.</param>
        /// <returns>Returns replaced expression</returns>
        public static Expression ReplacementExpression(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp) {
            return new ExpressionRebinder(map).Visit(exp);
        }

        /// <summary>
        /// Visits the <see cref="T:System.Linq.Expressions.ParameterExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override Expression VisitParameter(ParameterExpression node) {
            ParameterExpression replacement;
            if (this.map.TryGetValue(node, out replacement)) {
                node = replacement;
            }

            return base.VisitParameter(node);
        }
    }
}
