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

        public List<ExhibitionShortViewModel> GetExhibitions()
        {
            var exhibitionList = _exhibitionRepo.GetListProjection(
                e => new 
                {
                    ExhibitionId = e.ExhibitionId,
                    Name = e.Name,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    Logo = e.Logo
                },
                e => e.IsActive);
            return exhibitionList.Select(e => new ExhibitionShortViewModel()
            {
                ExhibitionId = e.ExhibitionId,
                Name = e.Name,
                StartDate = e.StartDate.ToLongDateString(),
                EndDate = e.EndDate.ToLongDateString(),
                Logo = e.Logo
            }).ToList();
        }
    }
}
