using GamexApi.Identity;
using GamexApi.Models;
using GamexApi.Results;
using GamexApi.Utilities;
using GamexEntity.Constant;
using GamexEntity.Enumeration;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace GamexApi.Controllers
{
    [Authorize]
    [System.Web.Mvc.RequireHttps]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        //Example setup for Identity resource
        private const string LocalLoginProvider = "Local";
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationSignInManager _signInManager;
        private readonly IAuthenticationManager _authenticationManager;
        private readonly ApplicationRoleManager _roleManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, 
            ApplicationSignInManager signInManager, 
            IAuthenticationManager authenticationManager, 
            ApplicationRoleManager roleManager, ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authenticationManager = authenticationManager;
            _roleManager = roleManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // Get api/Account
        public async Task<AccountViewModel> GetAccount() {
            var userId = User.Identity.GetUserId();
            var user = await _userManager.FindByIdAsync(userId);
            return new AccountViewModel {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = user.Roles.ToList().Select(r => r.RoleId.ToString()) as IList<string>
            };
        }

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            return new UserInfoViewModel
            {
                Email = User.Identity.GetEmail(),
                FirstName = User.Identity.GetFirstName(),
                LastName = User.Identity.GetLastName(),
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }

        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            _authenticationManager.SignOut(OAuthDefaults.AuthenticationType);
            return Ok();
        }

        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            IdentityUser user = await _userManager.FindByIdAsync(User.Identity.GetUserId());

            if (user == null)
            {
                return null;
            }

            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

            foreach (IdentityUserLogin linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                Email = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
        }

//        // POST api/Account/ChangePassword
//        [Route("ChangePassword")]
//        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }
//
//            IdentityResult result = await _userManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
//                model.NewPassword);
//            
//            if (!result.Succeeded)
//            {
//                return GetErrorResult(result);
//            }
//
//            return Ok();
//        }

        // POST api/Account/SetPassword
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await _userManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/AddExternalLogin
        [Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            IdentityResult result = await _userManager.AddLoginAsync(User.Identity.GetUserId(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await _userManager.RemovePasswordAsync(User.Identity.GetUserId());
            }
            else
            {
                result = await _userManager.RemoveLoginAsync(User.Identity.GetUserId(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null) {
            string redirectUri = string.Empty;

            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            var redirectUriValidationResult = ValidateClientAndRedirectUri(this.Request, ref redirectUri);
            if (!string.IsNullOrWhiteSpace(redirectUriValidationResult)) {
                return BadRequest(redirectUriValidationResult);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                _authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalBearer);
                return new ChallengeResult(provider, this);
            }

            var externalLoginInfo = new UserLoginInfo(externalLogin.LoginProvider, externalLogin.ProviderKey);
            ApplicationUser user = await _userManager.FindAsync(externalLoginInfo);
            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                _authenticationManager.SignOut(OAuthDefaults.AuthenticationType);
                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(_userManager,
                    OAuthDefaults.AuthenticationType);
                _authenticationManager.SignIn(oAuthIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                _authenticationManager.SignIn(identity);
            }
            return Ok();
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = _authenticationManager.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }

        // POST api/Account/
        [AllowAnonymous]
        //[Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser() {
                UserName = model.Username, 
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Point = 0,
                TotalPointEarned = 0,
                StatusId = (int) AccountStatusEnum.Active
            };
            IdentityResult result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            result = _userManager.AddToRole(user.Id, AccountRole.User);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            return Ok();
        }

        // POST api/Account/RegisterExternal
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal()
        {
            var info = await _authenticationManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return InternalServerError();
            }
            var user = _userManager.FindByEmail(info.ExternalIdentity.FindFirstValue(CustomClaimTypes.Email));
            if (user == null)
            {
                user = new ApplicationUser
                {
                    FirstName = info.ExternalIdentity.FindFirstValue(CustomClaimTypes.FirstName),
                    LastName = info.ExternalIdentity.FindFirstValue(CustomClaimTypes.LastName),
                    Email = info.ExternalIdentity.FindFirstValue(CustomClaimTypes.Email),
                    UserName = info.ExternalIdentity.FindFirstValue(CustomClaimTypes.Email),
                    Point = 0,
                    TotalPointEarned = 0,
                    StatusId = (int) AccountStatusEnum.Active
                };
                var addResult = await _userManager.CreateAsync(user);
                if (!addResult.Succeeded)
                {
                    return GetErrorResult(addResult);
                }
                _userManager.AddToRole(user.Id, AccountRole.User);
            }
            var result = await _userManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result); 
            }
            return Ok();
        }

        [HttpPut]
        [Authorize(Roles = AccountRole.User)]
        [Route("Password")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            var hasPassword = await _userManager.HasPasswordAsync(User.Identity.GetUserId());
            if ((hasPassword &&
                string.IsNullOrEmpty(model.OldPassword)))
            {   
                ModelState.AddModelError("ErrorMessage", "Please submit your current password");
                return BadRequest(ModelState);
            }
            //start change password
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (hasPassword)
            {
                var result = await _userManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }
            }
            else
            {
                var result = await _userManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }
            }
            return Ok();
        }


        #region Helpers

        private string ValidateClientAndRedirectUri(HttpRequestMessage request, ref string redirectUriOutput) {
            Uri redirectUri;
            var redirectUriString = GetQueryString(Request, "redirect_uri");
            if (string.IsNullOrWhiteSpace(redirectUriString)) {
                return "redirect_uri is required";
            }

            bool validUri = Uri.TryCreate(redirectUriString, UriKind.Absolute, out redirectUri);
            if (!validUri) {
                return "redirect_uri is invalid";
            }

            var clientId = GetQueryString(Request, "client_id");
            if (string.IsNullOrWhiteSpace(clientId)) {
                return "client_id is required";
            }

            //var client = _repo.FindClient(clientId);

            //if (client == null) {
            //    return string.Format("Client_id '{0}' is not registered in the system.", clientId);
            //}

            //if (!string.Equals(client.AllowedOrigin, redirectUri.GetLeftPart(UriPartial.Authority), StringComparison.OrdinalIgnoreCase)) {
            //    return string.Format("The given URL is not allowed by Client_id '{0}' configuration.", clientId);
            //}

            redirectUriOutput = redirectUri.AbsoluteUri;

            return string.Empty;

        }

        private string GetQueryString(HttpRequestMessage request, string key) {
            var queryStrings = request.GetQueryNameValuePairs();
            if (queryStrings == null) {
                return null;
            }

            var match = queryStrings.FirstOrDefault(keyValue => string.Compare(keyValue.Key, key, true) == 0);
            if (string.IsNullOrEmpty(match.Value)) {
                return null;
            }

            return match.Value;
        }


        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("ErrorMessage", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }
            public string ExternalAccessToken { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));
//                if (UserName != null)
//                {
//                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
//                }
                claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                claims.Add(new Claim(CustomClaimTypes.FirstName, FirstName, null, LoginProvider));
                claims.Add(new Claim(CustomClaimTypes.LastName, LastName, null, LoginProvider));
                claims.Add(new Claim(CustomClaimTypes.Email, Email, null, LoginProvider));


                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(CustomClaimTypes.Email),
                    Email = identity.FindFirstValue(CustomClaimTypes.Email),
                    FirstName = identity.FindFirstValue(CustomClaimTypes.FirstName),
                    LastName = identity.FindFirstValue(CustomClaimTypes.LastName),
                    ExternalAccessToken = identity.FindFirstValue("ExternalAccessToken")
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }
}
