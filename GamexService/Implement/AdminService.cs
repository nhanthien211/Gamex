﻿using GamexEntity;
using GamexEntity.Constant;
using GamexEntity.Enumeration;
using GamexRepository;
using GamexService.Interface;
using GamexService.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GamexService.Implement
{
    public class AdminService : IAdminService
    {
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<AspNetUsers> _userRepository;
        private readonly IRepository<Reward> _rewardRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AdminService(IRepository<Company> companyRepository, IRepository<AspNetUsers> userRepository, IRepository<Reward> rewardRepository, IUnitOfWork unitOfWork)
        {
            _companyRepository = companyRepository;
            _userRepository = userRepository;
            _rewardRepository = rewardRepository;
            _unitOfWork = unitOfWork;
        }

        public List<CompanyTableViewModel> LoadCompanyJoinRequestDataTable(string sortColumnDirection, string searchValue, int skip, int take)
        {
            var companyRequestList = _companyRepository.GetPagingProjection(
                c => new CompanyTableViewModel
                {
                    CompanyName = c.Name,
                    TaxNumber = c.TaxNumber,
                    Email = c.Email,
                    CompanyId = c.CompanyId
                },
                c => c.StatusId == (int)CompanyStatusEnum.Pending && (c.Name.Contains(searchValue) || c.TaxNumber.Contains(searchValue) || c.Email.Contains(searchValue)),
                c => c.Name, sortColumnDirection, take, skip
                );
            return companyRequestList.ToList();
        }

        public bool ApproveOrRejectCompanyRequest(string companyId, bool isApproved, ref string userId)
        {
            var user = _userRepository.GetSingle(u => u.CompanyId == companyId);
            var company = _companyRepository.GetSingle(c => c.CompanyId == companyId);

            if (user == null || company == null) return false;
            userId = user.Id;
            if (isApproved)
            {
                _companyRepository.Update(company);
                company.StatusId = (int)CompanyStatusEnum.Active;
                
                int updateResult;
                try
                {
                    updateResult = _unitOfWork.SaveChanges();
                }
                catch (Exception)
                {
                    return false;
                }

                if (updateResult > 0)
                {
                    
                    return true;
                }
                return false;
            }
            _companyRepository.Delete(company);
            _userRepository.Delete(user);

            int deleteResult;
            try
            {
                deleteResult = _unitOfWork.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }
            if (deleteResult > 0)
            {
                return true;
            }

            return false;
        }

        public List<CompanyTableViewModel> LoadCompanyDataTable(string sortColumnDirection, string searchValue, int skip, int take)
        {
            var companyList = _companyRepository.GetPagingProjection(
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
            return companyList.ToList();
        }

        public List<OrganizerTableViewModel> LoadOrganizerDataTable(string sortColumnDirection, string searchValue, int skip, int take)
        {
            var organizerList = _userRepository.GetPagingProjection(
                a => new OrganizerTableViewModel
                {
                    FullName = a.LastName + " " + a.FirstName,
                    Email =  a.Email,
                    Status = a.AccountStatus.Status,
                    Id =  a.Id
                }, 
                a => a.StatusId != (int) AccountStatusEnum.Pending 
                && a.AspNetRoles.Select(r => r.Name).Contains(AccountRole.Organizer) 
                && (a.FirstName.Contains(searchValue) || a.LastName.Contains(searchValue) 
                || a.Email.Contains(searchValue) || a.AccountStatus.Status.Contains(searchValue)), 
                a => a.FirstName, sortColumnDirection, take, skip
            );
            return organizerList.ToList();
        }

        public bool CreateReward(string userId, CreateRewardViewModel model)
        {
            var reward = new Reward
            {
                CreatedBy =  userId,
                Content = model.Content,
                Description = model.Description,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                IsActive = true,
                PointCost = model.PointCost,
                Quantity = model.Quantity
            };
            _rewardRepository.Insert(reward);
            try
            {
                var result = _unitOfWork.SaveChanges();
                if (result > 0)
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

        public List<RewardListViewModel> LoadRewardDataTable(string sortColumn, string sortColumnDirection, string searchValue, int skip, int take)
        {
            int sortOption;
            try
            {
                sortOption = Convert.ToInt32(sortColumn);
            }
            catch (Exception)
            {
                return null;
            }

            switch (sortOption)
            {
                case 1:
                    var rewardList = _rewardRepository.GetPagingProjection(
                    r => new
                    {
                        RewardId = r.RewardId,
                        RewardDescription = r.Description,
                        Quantity = r.Quantity,
                        IsActive = r.IsActive
                    },
                    r => r.Description.Contains(searchValue),
                    r => r.Quantity, sortColumnDirection, take, skip
                    );
                    return rewardList.Select(r => new RewardListViewModel
                    {
                        Status = r.IsActive ? "Enable" : "Disable",
                        Quantity = r.Quantity,
                        RewardId = r.RewardId,
                        RewardDescription = r.RewardDescription
                    }).ToList();
                case 2:
                    rewardList = _rewardRepository.GetPagingProjection(
                        r => new
                        {
                            RewardId = r.RewardId,
                            RewardDescription = r.Description,
                            Quantity = r.Quantity,
                            IsActive = r.IsActive
                        },
                        r => r.Description.Contains(searchValue),
                        r => r.IsActive, sortColumnDirection, take, skip
                    );
                    return rewardList.Select(r => new RewardListViewModel
                    {
                        Status = r.IsActive ? "Enable" : "Disable",
                        Quantity = r.Quantity,
                        RewardId = r.RewardId,
                        RewardDescription = r.RewardDescription
                    }).ToList();
                default:
                    return null;
            }
        }

        public RewardDetailViewModel GetRewardDetail(string rewardId)
        {
            int id;
            try
            {
                id = Convert.ToInt32(rewardId);
            }
            catch (Exception)
            {
                return null;
            }

            return _rewardRepository.GetSingleProjection( 
                r => new RewardDetailViewModel
                {
                    Content = r.Content,
                    Description = r.Description,
                    StartDate = r.StartDate,
                    EndDate = r.EndDate,
                    PointCost = r.PointCost,
                    Quantity = r.Quantity,
                    IsActive = r.IsActive,
                    RewardId = r.RewardId
                },
                r => r.RewardId == id
                );
        }

        public bool UpdateReward(RewardDetailViewModel model)
        {
            var reward = _rewardRepository.GetById(model.RewardId);
            if (reward == null)
            {
                return false;
            }
            _rewardRepository.Update(reward);
            reward.Content = model.Content;
            reward.Description = model.Description;
            reward.IsActive = model.IsActive;
            reward.PointCost = model.PointCost;
            reward.Quantity = model.Quantity;
            reward.StartDate = model.StartDate;
            reward.EndDate = model.EndDate;
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
