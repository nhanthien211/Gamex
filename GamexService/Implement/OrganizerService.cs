using GamexEntity;
using GamexRepository;
using GamexService.Interface;
using GamexService.Utilities;
using GamexService.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

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
                exhibition.Location = MyUtilities.CreateDbGeography(model.Longitude.Value, model.Latitude.Value);
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

        public List<UpcomingExhibitionViewModel> LoadUpcomingExhibitionDataTable(string sortColumnDirection, string searchValue, int skip, int take, string organizerId)
        {
            var newExhibitionList = _exhibitionRepository.GetPagingProjection(
                e => new
                {
                    ExhibitionId = e.ExhibitionId,
                    ExhibitionName = e.Name,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate
                },
                e => e.OrganizerId == organizerId
                     && e.StartDate > DateTime.Now
                     && e.Name.Contains(searchValue),
                e => e.StartDate, sortColumnDirection, take, skip
            );
            var result = newExhibitionList.Select(e => new UpcomingExhibitionViewModel
            {
                ExhibitionId = e.ExhibitionId,
                ExhibitionName = e.ExhibitionName,
                Time = e.StartDate.ToString("HH:mm dddd, dd MMMM yyyy") + " to " + e.EndDate.ToString("HH:mm dddd, dd MMMM yyyy")
            }).ToList();
            return result;
        }

        public ExhibitionDetailViewModel GetExhibitionDetail(string exhibitionId)
        {
            return _exhibitionRepository.GetSingleProjection(
                e => new ExhibitionDetailViewModel
                {
                    Description = e.Description,
                    ImageUrl = e.Logo,
                    Name = e.Name,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    Address = e.Address,
                    ExhibitionId = e.ExhibitionId,
                    Latitude = e.Location.Latitude,
                    Longitude = e.Location.Longitude
                },
                e => e.ExhibitionId == exhibitionId && e.StartDate > DateTime.Now
            );
        }

        public bool UpdateExhibitionDetail(ExhibitionDetailViewModel model)
        {
            var exhibition = _exhibitionRepository.GetById(model.ExhibitionId);
            if (exhibition != null)
            {
                _exhibitionRepository.Update(exhibition);
                exhibition.Description = model.Description;
                exhibition.Address = model.Address;
                exhibition.EndDate = model.EndDate;
                exhibition.Logo = model.ImageUrl;
                exhibition.StartDate = model.StartDate;
                exhibition.Name = model.Name;
                if (model.Latitude.HasValue && model.Longitude.HasValue)
                {
                    exhibition.Location = MyUtilities.CreateDbGeography(model.Longitude.Value, model.Latitude.Value);
                }
                else
                {
                    exhibition.Location = null;
                }
                int result;
                try
                {
                    result = _unitOfWork.SaveChanges();
                    if (result >= 0)
                    {
                        return true;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }
    }
}
