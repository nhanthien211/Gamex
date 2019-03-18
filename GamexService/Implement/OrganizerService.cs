using GamexEntity;
using GamexRepository;
using GamexService.Interface;
using GamexService.Utilities;
using GamexService.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using GamexEntity.Constant;

namespace GamexService.Implement
{
    public class OrganizerService : IOrganizerService
    {
        private readonly IRepository<Exhibition> _exhibitionRepository;
        private readonly IRepository<Booth> _boothRepository;
        private readonly IRepository<Company> _companyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OrganizerService(IRepository<Exhibition> exhibitionRepository, 
            IRepository<Booth> boothRepository, 
            IRepository<Company> companyRepository,
            IUnitOfWork unitOfWork)
        {
            _exhibitionRepository = exhibitionRepository;
            _boothRepository = boothRepository;
            _companyRepository = companyRepository;
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

        public List<ExhibitionTableViewModel> LoadExhibitionDataTable(string type, string sortColumnDirection, string searchValue, int skip, int take, string organizerId)
        {
            switch (type)
            {
                case ExhibitionTypes.Upcoming:
                    var exhibitionList = _exhibitionRepository.GetPagingProjection(
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
                    var result = exhibitionList.Select(e => new ExhibitionTableViewModel
                    {
                        ExhibitionId = e.ExhibitionId,
                        ExhibitionName = e.ExhibitionName,
                        Time = e.StartDate.ToString("HH:mm dddd, dd MMMM yyyy") + " to " + e.EndDate.ToString("HH:mm dddd, dd MMMM yyyy")
                    }).ToList();
                    return result;
                case ExhibitionTypes.Ongoing:
                    exhibitionList = _exhibitionRepository.GetPagingProjection(
                        e => new
                        {
                            ExhibitionId = e.ExhibitionId,
                            ExhibitionName = e.Name,
                            StartDate = e.StartDate,
                            EndDate = e.EndDate
                        },
                        e => e.OrganizerId == organizerId
                             && e.StartDate <= DateTime.Now && e.EndDate >= DateTime.Now
                             && e.Name.Contains(searchValue),
                        e => e.StartDate, sortColumnDirection, take, skip
                    );
                    result = exhibitionList.Select(e => new ExhibitionTableViewModel
                    {
                        ExhibitionId = e.ExhibitionId,
                        ExhibitionName = e.ExhibitionName,
                        Time = e.StartDate.ToString("HH:mm dddd, dd MMMM yyyy") + " to " + e.EndDate.ToString("HH:mm dddd, dd MMMM yyyy")
                    }).ToList();
                    return result;
                default:
                    return null;
            }
            
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

        public ExhibitionDetailViewOnlyModel GetExhibitionDetailViewOnly(string exhibitionId)
        {
            return _exhibitionRepository.GetSingleProjection(
                e => new ExhibitionDetailViewOnlyModel
                {
                    Description = e.Description,
                    Logo = e.Logo,
                    Name = e.Name,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    Address = e.Address,
                    ExhibitionId = e.ExhibitionId
                },
                e => e.ExhibitionId == exhibitionId && e.StartDate <= DateTime.Now
                                                    && e.EndDate >= DateTime.Now
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

        public List<AttendedCompanyViewModel> LoadAttendedCompanyList(string sortColumnDirection, string searchValue, int skip, int take, string exhibitionId)
        {
            return _boothRepository.GetPagingProjection(
                b => new AttendedCompanyViewModel
                {
                    CompanyId = b.CompanyId,
                    ExhibitionId = b.ExhibitionId,
                    CompanyName = b.Company.Name
                },
                b => b.ExhibitionId == exhibitionId && b.Company.Name.Contains(searchValue),
                b => b.Company.Name, sortColumnDirection, take, skip
            ).ToList();
        }

        public AttendedCompanyDetailViewModel GetAttendedCompanyDetail(string exhibitionId, string companyId)
        {
            var company = _companyRepository.GetSingleProjection(
                c => new AttendedCompanyDetailViewModel
                {
                    Description = c.Description,
                    Email = c.Email,
                    CompanyId = c.CompanyId,
                    Address = c.Address,
                    Phone = c.Phone,
                    Website = c.Website,
                    CompanyName = c.Name,
                },
                c => c.CompanyId == companyId
            );
            var booth = _boothRepository.GetList(b => b.CompanyId == companyId && b.ExhibitionId == exhibitionId && !string.IsNullOrEmpty(b.BoothNumber));
            company.ExhibitionId = exhibitionId;
            if (booth != null)
            {
                company.Booth = booth.Select(b => new BootViewModel
                {
                    BoothNumber = b.BoothNumber
                }).ToList();
            }
            
            return company;
        }

        public bool AssignBoothToCompany(AttendedCompanyDetailViewModel model, string exhibitionId, string companyId)
        {
            var currentBoothList =
                _boothRepository.GetList(b => b.CompanyId == companyId && b.ExhibitionId == exhibitionId);
            foreach (var booth in currentBoothList)
            {
                _boothRepository.Delete(booth);
            }

            foreach (var boothNumber in model.Booth)
            {
                var booth = new Booth
                {
                    CompanyId = companyId,
                    ExhibitionId = exhibitionId,
                    BoothNumber = boothNumber.BoothNumber
                };
                _boothRepository.Insert(booth);
            }

            try
            {
                var result = _unitOfWork.SaveChanges();
                if (result >= 0)
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }
    }
}
