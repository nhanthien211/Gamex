using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamexEntity;
using GamexRepository;
using GamexService.Interface;
using GamexService.Utilities;
using GamexService.ViewModel;

namespace GamexService.Implement
{
    public class OrganizerService : IOrganizerService
    {
        private readonly IRepository<Exhibition> _exhibitionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OrganizerService(IRepository<Exhibition> exhibitionRepository, IUnitOfWork unitOfWork)
        {
            _exhibitionRepository = exhibitionRepository;
            _unitOfWork = unitOfWork;
        }

        public bool CreateExhibition(CreateExhibitionViewModel model, string exhibitionCode, string uploadUrl, string organizerId)
        {
            
            var exhibition = new Exhibition
            {
                ExhibitionId = exhibitionCode,
                Logo = uploadUrl,
                OrganizerId = organizerId,
                Description = model.Description,
                Name = model.Name,
                Address = model.Address,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                IsActive = true
            };
            if (model.Latitude.HasValue && model.Longitude.HasValue)
            {
                exhibition.Location = MyUtilities.CreateDbGeography(model.Latitude.Value, model.Longitude.Value);
            }
            _exhibitionRepository.Insert(exhibition);
            int result;
            try
            {
                result = _unitOfWork.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }
            return result > 0;
        }
    }
}
