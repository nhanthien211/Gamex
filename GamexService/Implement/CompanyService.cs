using GamexEntity;
using GamexEntity.Enumeration;
using GamexRepository;
using GamexService.Interface;
using GamexService.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GamexService.Implement
{
    public class CompanyService : ICompanyService
    {
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<Exhibition> _exhibitionRepository;
        private readonly IRepository<Booth> _boothRepository;
        private readonly IRepository<Survey> _surveyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CompanyService(IRepository<Company> companyRepository, IRepository<Exhibition> exhibitionRepository, 
            IRepository<Booth> boothRepository, IRepository<Survey> surveyRepository, IUnitOfWork unitOfWork)
        {
            _companyRepository = companyRepository;
            _exhibitionRepository = exhibitionRepository;
            _boothRepository = boothRepository;
            _surveyRepository = surveyRepository;
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
                Address = model.Address,
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

        public List<CompanyViewExhibitionViewModel> LoadNewExhibitionDataTable(string sortColumnDirection, string searchValue, int skip, int take, string companyId)
        {
            var newExhibitionList = _exhibitionRepository.GetPagingProjection(
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
            var result = newExhibitionList.Select(e => new CompanyViewExhibitionViewModel
            {
                ExhibitionId = e.ExhibitionId,
                ExhibitionName = e.ExhibitionName,
                Time = e.StartDate.ToString("HH:mm dddd, dd MMMM yyyy") + " to " + e.EndDate.ToString("HH:mm dddd, dd MMMM yyyy")
            }).ToList();
            return result;
        }

        public ExhibitionDetailViewOnlyModel GetExhibitionDetail(string exhibitionId)
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
                e => e.ExhibitionId == exhibitionId && e.StartDate > DateTime.Now
                );
        }

        public bool IsCompanyHasJoinExhibition(string exhibitionId, string companyId)
        {
            return  _boothRepository.GetSingleProjection(b => b.Id,
                b => b.CompanyId == companyId && b.ExhibitionId == exhibitionId) != 0;
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

        public List<CompanyViewExhibitionViewModel> LoadUpcomingExhibitionDataTable(string sortColumnDirection, string searchValue, int skip, int take, string companyId)
        {
            var newExhibitionList = _exhibitionRepository.GetPagingProjection(
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
            var result = newExhibitionList.Select(e => new CompanyViewExhibitionViewModel
            {
                ExhibitionId = e.ExhibitionId,
                ExhibitionName = e.ExhibitionName,
                Time = e.StartDate.ToString("HH:mm dddd, dd MMMM yyyy") + " to " + e.EndDate.ToString("HH:mm dddd, dd MMMM yyyy")
            }).ToList();
            return result;
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

        public bool CreateSurvey(CreateSurveyViewModel model, string companyId, string accountId)
        {
            
            var survey = new Survey
            {
                Description = model.Description,
                Title = model.Title,
                ExhibitionId = model.ExhibitionId,
                Point = 100,
                CompanyId = companyId,
                AccountId = accountId,
                IsActive = true
            };
            _surveyRepository.Insert(survey);
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
