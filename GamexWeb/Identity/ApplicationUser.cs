﻿using System.Security.Claims;
using System.Threading.Tasks;
using GamexEntity.Constant;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GamexWeb.Identity
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Point { get; set; }
        public int TotalPointEarned { get; set; }
        public string CompanyId { get; set; }
        public int StatusId { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim(CustomClaimTypes.UserFullName, LastName + " " + FirstName));
            userIdentity.AddClaim(new Claim(CustomClaimTypes.Email, Email));
            if (!string.IsNullOrEmpty(CompanyId))
            {
                userIdentity.AddClaim(new Claim(CustomClaimTypes.CompanyId, CompanyId));
            }
            return userIdentity;
        }
    }

}