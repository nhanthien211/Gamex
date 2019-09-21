using GamexEntity;
using GamexEntity.Enumeration;
using GamexRepository;
using GamexService.Interface;
using GamexService.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GamexEntity.Constant;
using GamexService.Utilities;
using OfficeOpenXml;

namespace GamexService.Implement
{
    public class CompanyService : ICompanyService
    {
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<Exhibition> _exhibitionRepository;
        private readonly IRepository<Booth> _boothRepository;
        private readonly IRepository<AspNetUsers> _accountRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CompanyService(IRepository<Company> companyRepository, IRepository<Exhibition> exhibitionRepository, 
            IRepository<Booth> boothRepository,
            IRepository<AspNetUsers> accountRepository,
            IUnitOfWork unitOfWork)
        {
            _companyRepository = companyRepository;
            _exhibitionRepository = exhibitionRepository;
            _boothRepository = boothRepository;
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
        }

        public SelectCompanyViewModel SelectCompanyRegisterStatus(SelectCompanyViewModel model)
        {
            var company = _companyRepository.GetSingleProjection(
                c => new SelectCompanyViewModel
                {
                    CompanyTaxId = c.TaxNumber,
                    CompanyName = c.Name,
                    Status = c.StatusId,
                    CompanyId = c.CompanyId
                },
                c => string.Equals(c.TaxNumber, model.CompanyTaxId)
                );
            return company;
        }

        public bool RegisterNewCompany(CompanyRegisterViewModel model, string companyId)
        {
            Company company = new Company
            {
                CompanyId = companyId,
                Name =  model.Name,
                Email = model.Email,
                Phone =  model.Phone,
                Website = model.Website,
                TaxNumber = model.TaxNumber,
                StatusId = (int) CompanyStatusEnum.Pending
            };
            _companyRepository.Insert(company);
            try
            {
                int result = _unitOfWork.SaveChanges();
                return result > 0;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool IsCompanyRegistered(string taxNumber)
        {
            var company = _companyRepository.GetSingleProjection(
                c => c.TaxNumber,
                c => string.Equals(c.TaxNumber, taxNumber)
            );
            if (!string.IsNullOrEmpty(company))
            {
                //company  registered
                return true;
            }
            return false;
        }

        public void RemoveCompany(string companyId)
        {
            var company = _companyRepository.GetById(companyId);
            _companyRepository.Delete(company);
            _unitOfWork.SaveChanges();
        }

        public ExhibitionDetailViewOnlyModel GetExhibitionDetail(string exhibitionId, string type)
        {
            switch (type)
            {
                case ExhibitionTypes.New:
                case ExhibitionTypes.Upcoming:
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
                        e => e.ExhibitionId == exhibitionId && e.StartDate > DateTime.Now
                    );
                case ExhibitionTypes.Ongoing:
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
                case ExhibitionTypes.Past:
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
                        e => e.ExhibitionId == exhibitionId && e.EndDate < DateTime.Now
                    );
                default:
                    return null;
            }
        }

        public string GetCompanyBoothInExhibition(string exhibitionId, string companyId)
        {
            var booth = _boothRepository.GetListProjection(
                b => b.BoothNumber,
                b => b.CompanyId == companyId && b.ExhibitionId == exhibitionId
                ).ToList();
            var result = "";
            foreach (var number in booth)
            {
                result += number + " ";  
            }
            return result;
        }
    
        public bool IsCompanyHasJoinExhibition(string exhibitionId, string companyId)
        {
            return  _boothRepository.GetSingleProjection(
                        b => b.Id,
                            b => b.CompanyId == companyId && b.ExhibitionId == exhibitionId 
                     ) != 0;
        }

        public bool JoinExhibition(string exhibitionId, string companyId)
        {
            var booth = new Booth
            {
                CompanyId = companyId,
                ExhibitionId = exhibitionId,
            };
            _boothRepository.Insert(booth);
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

        public List<ExhibitionTableViewModel> LoadExhibitionDataTable(string type, string sortColumnDirection, string searchValue, int skip, int take, string companyId)
        {
            switch (type)
            {
                case ExhibitionTypes.New:
                    var exhibitionList = _exhibitionRepository.GetPagingProjection(
                        e => new
                        {
                            ExhibitionId = e.ExhibitionId,
                            ExhibitionName = e.Name,
                            StartDate = e.StartDate,
                            EndDate = e.EndDate
                        },
                        e => !e.Booth.Where(b => b.ExhibitionId == e.ExhibitionId).Select(b => b.CompanyId).Contains(companyId)
                             && e.StartDate > DateTime.Now
                             && e.Name.Contains(searchValue),
                        e => e.StartDate, sortColumnDirection, take, skip
                    );
                    var result = exhibitionList.Select(e => new ExhibitionTableViewModel
                    {
                        ExhibitionId = e.ExhibitionId,
                        ExhibitionName = e.ExhibitionName,
                        Time = e.StartDate.ToString("D") + " to " + e.EndDate.ToString("D")
                    }).ToList();
                    return result;
                case ExhibitionTypes.Upcoming:
                    exhibitionList = _exhibitionRepository.GetPagingProjection(
                        e => new
                        {
                            ExhibitionId = e.ExhibitionId,
                            ExhibitionName = e.Name,
                            StartDate = e.StartDate,
                            EndDate = e.EndDate
                        },
                        e => e.Booth.Where(b => b.ExhibitionId == e.ExhibitionId).Select(b => b.CompanyId).Contains(companyId)
                             && e.StartDate > DateTime.Now
                             && e.Name.Contains(searchValue),
                        e => e.StartDate, sortColumnDirection, take, skip
                    );
                    result = exhibitionList.Select(e => new ExhibitionTableViewModel
                    {
                        ExhibitionId = e.ExhibitionId,
                        ExhibitionName = e.ExhibitionName,
                        Time = e.StartDate.ToString("D") + " to " + e.EndDate.ToString("D")
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
                        e => e.Booth.Where(b => b.ExhibitionId == e.ExhibitionId).Select(b => b.CompanyId).Contains(companyId)
                             && e.StartDate <= DateTime.Now && e.EndDate >= DateTime.Now 
                             && e.Name.Contains(searchValue),
                        e => e.StartDate, sortColumnDirection, take, skip
                    );
                    result = exhibitionList.Select(e => new ExhibitionTableViewModel
                    {
                        ExhibitionId = e.ExhibitionId,
                        ExhibitionName = e.ExhibitionName,
                        Time = e.StartDate.ToString("D") + " to " + e.EndDate.ToString("D")
                    }).ToList();
                    return result;
                case ExhibitionTypes.Past:
                    exhibitionList = _exhibitionRepository.GetPagingProjection(
                        e => new
                        {
                            ExhibitionId = e.ExhibitionId,
                            ExhibitionName = e.Name,
                            StartDate = e.StartDate,
                            EndDate = e.EndDate
                        },
                        e => e.Booth.Where(b => b.ExhibitionId == e.ExhibitionId).Select(b => b.CompanyId).Contains(companyId)
                             && e.EndDate < DateTime.Now 
                             && e.Name.Contains(searchValue),
                        e => e.StartDate, sortColumnDirection, take, skip
                    );
                    result = exhibitionList.Select(e => new ExhibitionTableViewModel
                    {
                        ExhibitionId = e.ExhibitionId,
                        ExhibitionName = e.ExhibitionName,
                        Time = e.StartDate.ToString("D") + " to " + e.EndDate.ToString("D")
                    }).ToList();
                    return result;
                default:
                    return null;
            }
            
        }

        public bool QuitExhibition(string exhibitionId, string companyId)
        {
            var deleteList = _boothRepository.GetList(b => b.CompanyId == companyId && b.ExhibitionId == exhibitionId);
            foreach (var record in deleteList)
            {
                _boothRepository.Delete(record);
            }
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

        public CompanyProfileViewModel GetCompanyProfile(string companyId)
        {
            return _companyRepository.GetSingleProjection(
                c => new CompanyProfileViewModel
            {
                    Description = c.Description,
                    Email = c.Email,
                    TaxNumber = c.TaxNumber,
                    ImageUrl = c.Logo,
                    Address = c.Address,
                    CompanyName = c.Name,
                    Phone = c.Phone,
                    Website = c.Website,
                    Latitude = c.Location.Latitude,
                    Longitude = c.Location.Longitude
            },
                c => c.CompanyId == companyId
            );
        }

        public bool UpdateCompanyProfile(CompanyProfileViewModel model, string companyId)
        {
            var company = _companyRepository.GetById(companyId);
            if (company != null)
            {
                company.Description = model.Description;
                company.Address = model.Address;
                company.Email = model.Email;
                company.Logo = model.ImageUrl;
                company.Phone = model.Phone;
                company.Website = model.Website;
                if (model.Latitude.HasValue && model.Longitude.HasValue)
                {
                    company.Location = MyUtilities.CreateDbGeography(model.Longitude.Value, model.Latitude.Value);
                }
                else
                {
                    company.Location = null;
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
            }
            return false;
        }

        public List<EmployeeRequestViewModel> LoadEmployeeRequestDatatable(string sortColumnDirection, string searchValue, int skip, int take, string companyId)
        {
            var employeeRequestList = _accountRepository.GetPagingProjection(
                a => new EmployeeRequestViewModel
                {
                    FullName = a.LastName + " " + a.FirstName,
                    Email = a.Email,
                    UserId = a.Id
                },
                a => (a.FirstName.Contains(searchValue) || a.LastName.Contains(searchValue)) && a.CompanyId == companyId && a.StatusId == (int)AccountStatusEnum.Pending,
                a => a.LastName, sortColumnDirection, take, skip
            );
            return employeeRequestList.ToList();
        }
    }

    
}
