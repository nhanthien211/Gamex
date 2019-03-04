using GamexApiService.Interface;
using GamexApiService.ViewModel;
using GamexEntity;
using GamexRepository;
using System.Collections.Generic;
using System.Linq;

namespace GamexApiService.Implement
{
    public class ExhibitionService : IExhibitionService
    {
        private IRepository<Exhibition> _exhibitionRepo;
        private IUnitOfWork _unitOfWork;

        public ExhibitionService(IRepository<Exhibition> exhibitionRepo, IUnitOfWork unitOfWork)
        {
            _exhibitionRepo = exhibitionRepo;
            _unitOfWork = unitOfWork;
        }

        public List<ExhibitionViewModel> GetExhibitions()
        {
            var exhibitionList = _exhibitionRepo.GetListProjection(
                e => new 
                {
                    ExhibitionId = e.ExhibitionId,
                    Name = e.Name,
                    Address = e.Address,
                    Description = e.Description,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    Location = e.Location,
                    Logo = e.Logo,
                    OrganizerId = e.OrganizerId
                });
            return exhibitionList.Select(e => new ExhibitionViewModel
            {
                ExhibitionId = e.ExhibitionId,
                Name = e.Name,
                Address = e.Address,
                Description = e.Description,
                StartDate = e.StartDate.ToLongDateString(),
                EndDate = e.EndDate.ToLongDateString(),
                Lat = e.Location == null ? string.Empty : e.Location.Latitude.Value.ToString(),
                Lng = e.Location == null ? string.Empty : e.Location.Longitude.Value.ToString(),
                Logo = e.Logo,
                OrganizerId = e.OrganizerId
            }).ToList();
        }
    }
}
