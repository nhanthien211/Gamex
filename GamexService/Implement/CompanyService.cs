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
        private readonly IUnitOfWork _unitOfWork;

        public CompanyService(IRepository<Company> companyRepository, IUnitOfWork unitOfWork)
        {
            _companyRepository = companyRepository;
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

        public bool RegisterNewCompany(CompanyRegisterViewModel model)
        {
            Company company = new Company
            {
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

        public List<CompanyTableViewModel> LoadCompanyJoinRequestDataTable(string sortColumnDirection, string searchValue, int skip, int take)
        {
            var companyRequestList = _companyRepository.GetListProjection(
                c => new CompanyTableViewModel
                {
                    CompanyName = c.Name,
                    TaxNumber = c.TaxNumber,
                    Email = c.Email,
                    CompanyId = c.CompanyId
                },
                c => c.StatusId == (int) CompanyStatusEnum.Pending && (c.Name.Contains(searchValue) || c.TaxNumber.Contains(searchValue) || c.Email.Contains(searchValue)),
                c => c.Name, sortColumnDirection, take, skip
                );
            return companyRequestList.ToList();
        }

        public void ApproveOrRejectCompanyRequest(int companyId, bool isApproved)
        {
            if (isApproved)
            {
                var company = _companyRepository.GetSingle(c => c.CompanyId == companyId);
                _companyRepository.Update(company);
                company.StatusId = (int) CompanyStatusEnum.Active;
                _unitOfWork.SaveChanges();
            }
            else
            {
                _companyRepository.Delete(companyId);
                _unitOfWork.SaveChanges();
            }
        }

        public List<CompanyTableViewModel> LoadCompanyDataTable(string sortColumnDirection, string searchValue, int skip, int take)
        {
            var companyRequestList = _companyRepository.GetListProjection(
                c => new CompanyTableViewModel
                {
                    CompanyName = c.Name,
                    TaxNumber = c.TaxNumber,
                    Email = c.Email,
                    CompanyId = c.CompanyId
                },
                c => c.StatusId != (int)CompanyStatusEnum.Pending && (c.Name.Contains(searchValue) || c.TaxNumber.Contains(searchValue) || c.Email.Contains(searchValue)),
                c => c.Name, sortColumnDirection, take, skip
            );
            return companyRequestList.ToList();
        }
    }
}
