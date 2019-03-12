using GamexEntity.Constant;
using GamexService.Interface;
using GamexService.ViewModel;
using GamexWeb.Identity;
using GamexWeb.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GamexWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationSignInManager _signInManager;
        private readonly IAuthenticationManager _authenticationManager;
        private readonly ApplicationRoleManager _roleManager;
        private readonly IAccountService _accountService;
        private readonly IIdentityMessageService _emailService;

        public AccountController(ApplicationUserManager userManager, 
            ApplicationSignInManager signInManager, 
            IAuthenticationManager authenticationManager, 
            ApplicationRoleManager roleManager, 
            IAccountService accountService,
            IIdentityMessageService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authenticationManager = authenticationManager;
            _roleManager = roleManager;
            _accountService = accountService;
            _emailService = emailService;
        }

        [HttpGet]
        [Authorize(Roles = AccountRole.Admin + ", " + AccountRole.Company + ", " + AccountRole.Organizer)]
        [Route("Account")]
        public ActionResult AccountInfo()
        {
            var profile = _accountService.GetProfileView(User.Identity.GetUserId());
            return View(profile);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl = null)
        {
            return View("~/Views/Home/index.cshtml");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Home/index.cshtml", model);
            }

            //check if user not registered
            var result = _accountService.GetLoginAccount(model.Id);
            if (result.Id == null)
            {
                ModelState.AddModelError("ErrorMessage", result.ErrorMessage);
                return View("~/Views/Home/index.cshtml", model);
            }

            //check if normal user
            var role = _userManager.GetRoles(result.UserId).FirstOrDefault(r => r == AccountRole.User);
            if (role != null)
            {
                ModelState.AddModelError("ErrorMessage", "This account is not allowed. Please refer to our mobile app.");
                return View("~/Views/Home/index.cshtml", model);
            }            

            var loginResult = _signInManager.PasswordSignIn(result.Id, model.Password, model.RememberMe, true);
            switch (loginResult)
            {
                case SignInStatus.Failure:
                    ModelState.AddModelError("ErrorMessage", "Invalid Username or Password");
                    return View("~/Views/Home/index.cshtml", model);
                case SignInStatus.LockedOut:
                    ModelState.AddModelError("ErrorMessage", "Too many attempts, please try again later");
                    return View("~/Views/Home/index.cshtml", model);
                case SignInStatus.Success:
                    return RedirectToAction("AccountInfo", "Account");
            }
            //if we got this far, something went wrong 
            ModelState.AddModelError("ErrorMessage", "Unknown Error");
            return View("~/Views/Home/index.cshtml", model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Logout()
        {
            _authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AccountRole.Admin + ", " + AccountRole.Company + ", " + AccountRole.Organizer)]
        [Route("Account")]
        public ActionResult UpdateProfile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Account/AccountInfo.cshtml", model);
            }
            //check username duplicate email
            var isDuplicateUsername = _accountService.IsUsernameDuplicate(model.Username, User.Identity.GetUserId());
            if (isDuplicateUsername)
            {
                ModelState.AddModelError("Username", "Name " + model.Username + " is already taken");
                return View("~/Views/Account/AccountInfo.cshtml", model);
            }

            var user = _userManager.FindById(User.Identity.GetUserId());
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.UserName = model.Username;
            
            var result = _userManager.Update(user);
            if (result.Succeeded)
            {
                User.AddUpdateClaim(CustomClaimTypes.UserFullName, user.LastName + " " + user.FirstName, _authenticationManager);
                User.AddUpdateClaim(CustomClaimTypes.Email, user.Email, _authenticationManager);
                model.IsSuccessful = true;
                return View("~/Views/Account/AccountInfo.cshtml", model);
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
            model.IsSuccessful = false;
            return View("~/Views/Account/AccountInfo.cshtml", model);
        }

        [HttpGet]
        [Authorize(Roles = AccountRole.Admin + ", " + AccountRole.Company + ", " + AccountRole.Organizer)]
        [Route("Account/Password")]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AccountRole.Admin + ", " + AccountRole.Company + ", " + AccountRole.Organizer)]
        [Route("Account/Password")]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var result =
                _userManager.ChangePassword(User.Identity.GetUserId(), model.CurrentPassword, model.NewPassword);
            model.IsSuccessful = result.Succeeded;
            if (!result.Succeeded)
            {
                string error = "";
                foreach (string message in result.Errors)
                {
                    error = error + message;
                }
                model.ErrorMessage = error;
            }
            else
            {
                _emailService.Send(new IdentityMessage
                {
                    Body = "Your password has been changed",
                    Destination = User.Identity.GetEmail(),
                    Subject = "[SECURITY ALERT] PASSWORD CHANGED"
                });
            }
            return View(model);
        }

        

        #region Sample
        //        //
        //        // GET: /Account/Login
        //        [AllowAnonymous]
        //        public ActionResult Login(string returnUrl)
        //        {
        //            ViewBag.ReturnUrl = returnUrl;
        //            return View();
        //        }
        //
        //        //
        //        // POST: /Account/Login
        //        [HttpPost]
        //        [AllowAnonymous]
        //        [ValidateAntiForgeryToken]
        //        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        //        {
        //            if (!ModelState.IsValid)
        //            {
        //                return View(model);
        //            }
        //
        //            // This doesn't count login failures towards account lockout
        //            // To enable password failures to trigger account lockout, change to shouldLockout: true
        //            var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
        //            switch (result)
        //            {
        //                case SignInStatus.Success:
        //                    return RedirectToLocal(returnUrl);
        //                case SignInStatus.LockedOut:
        //                    return View("Lockout");
        //                case SignInStatus.RequiresVerification:
        //                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
        //                case SignInStatus.Failure:
        //                default:
        //                    ModelState.AddModelError("", "Invalid login attempt.");
        //                    return View(model);
        //            }
        //        }
        //
        //        //
        //        // GET: /Account/VerifyCode
        //        [AllowAnonymous]
        //        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        //        {
        //            // Require that the user has already logged in via username/password or external login
        //            if (!await signInManager.HasBeenVerifiedAsync())
        //            {
        //                return View("Error");
        //            }
        //            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        //        }
        //
        //        //
        //        // POST: /Account/VerifyCode
        //        [HttpPost]
        //        [AllowAnonymous]
        //        [ValidateAntiForgeryToken]
        //        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        //        {
        //            if (!ModelState.IsValid)
        //            {
        //                return View(model);
        //            }
        //
        //            // The following code protects for brute force attacks against the two factor codes. 
        //            // If a user enters incorrect codes for a specified amount of time then the user account 
        //            // will be locked out for a specified amount of time. 
        //            // You can configure the account lockout settings in IdentityConfig
        //            var result = await signInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
        //            switch (result)
        //            {
        //                case SignInStatus.Success:
        //                    return RedirectToLocal(model.ReturnUrl);
        //                case SignInStatus.LockedOut:
        //                    return View("Lockout");
        //                case SignInStatus.Failure:
        //                default:
        //                    ModelState.AddModelError("", "Invalid code.");
        //                    return View(model);
        //            }
        //        }
        //
        //        //
        //        // GET: /Account/Register
        //        [AllowAnonymous]
        //        public ActionResult Register()
        //        {
        //            return View();
        //        }
        //
        //        //
        //        // POST: /Account/Register
        //        [HttpPost]
        //        [AllowAnonymous]
        //        [ValidateAntiForgeryToken]
        //        public async Task<ActionResult> Register(RegisterViewModel model)
        //        {
        //            if (ModelState.IsValid)
        //            {
        //                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        //                var result = await userManager.CreateAsync(user, model.Password);
        //                if (result.Succeeded)
        //                {
        //                    await signInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
        //                    
        //                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
        //                    // Send an email with this link
        //                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
        //                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
        //                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
        //
        //                    return RedirectToAction("Index", "Home");
        //                }
        //                AddErrors(result);
        //            }
        //
        //            // If we got this far, something failed, redisplay form
        //            return View(model);
        //        }
        //
        //        //
        //        // GET: /Account/ConfirmEmail
        //        [AllowAnonymous]
        //        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        //        {
        //            if (userId == null || code == null)
        //            {
        //                return View("Error");
        //            }
        //            var result = await userManager.ConfirmEmailAsync(userId, code);
        //            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        //        }
        //
        //        //
        //        // GET: /Account/ForgotPassword
        //        [AllowAnonymous]
        //        public ActionResult ForgotPassword()
        //        {
        //            return View();
        //        }
        //
        //        //
        //        // POST: /Account/ForgotPassword
        //        [HttpPost]
        //        [AllowAnonymous]
        //        [ValidateAntiForgeryToken]
        //        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        //        {
        //            if (ModelState.IsValid)
        //            {
        //                var user = await userManager.FindByNameAsync(model.Email);
        //                if (user == null || !(await userManager.IsEmailConfirmedAsync(user.Id)))
        //                {
        //                    // Don't reveal that the user does not exist or is not confirmed
        //                    return View("ForgotPasswordConfirmation");
        //                }
        //
        //                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
        //                // Send an email with this link
        //                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
        //                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
        //                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
        //                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
        //            }
        //
        //            // If we got this far, something failed, redisplay form
        //            return View(model);
        //        }
        //
        //        //
        //        // GET: /Account/ForgotPasswordConfirmation
        //        [AllowAnonymous]
        //        public ActionResult ForgotPasswordConfirmation()
        //        {
        //            return View();
        //        }
        //
        //        //
        //        // GET: /Account/ResetPassword
        //        [AllowAnonymous]
        //        public ActionResult ResetPassword(string code)
        //        {
        //            return code == null ? View("Error") : View();
        //        }
        //
        //        //
        //        // POST: /Account/ResetPassword
        //        [HttpPost]
        //        [AllowAnonymous]
        //        [ValidateAntiForgeryToken]
        //        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        //        {
        //            if (!ModelState.IsValid)
        //            {
        //                return View(model);
        //            }
        //            var user = await userManager.FindByNameAsync(model.Email);
        //            if (user == null)
        //            {
        //                // Don't reveal that the user does not exist
        //                return RedirectToAction("ResetPasswordConfirmation", "Account");
        //            }
        //            var result = await userManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
        //            if (result.Succeeded)
        //            {
        //                return RedirectToAction("ResetPasswordConfirmation", "Account");
        //            }
        //            AddErrors(result);
        //            return View();
        //        }
        //
        //        //
        //        // GET: /Account/ResetPasswordConfirmation
        //        [AllowAnonymous]
        //        public ActionResult ResetPasswordConfirmation()
        //        {
        //            return View();
        //        }
        //
        //        //
        //        // POST: /Account/ExternalLogin
        //        [HttpPost]
        //        [AllowAnonymous]
        //        [ValidateAntiForgeryToken]
        //        public ActionResult ExternalLogin(string provider, string returnUrl)
        //        {
        //            // Request a redirect to the external login provider
        //            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        //        }
        //
        //        //
        //        // GET: /Account/SendCode
        //        [AllowAnonymous]
        //        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        //        {
        //            var userId = await signInManager.GetVerifiedUserIdAsync();
        //            if (userId == null)
        //            {
        //                return View("Error");
        //            }
        //            var userFactors = await userManager.GetValidTwoFactorProvidersAsync(userId);
        //            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
        //            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        //        }
        //
        //        //
        //        // POST: /Account/SendCode
        //        [HttpPost]
        //        [AllowAnonymous]
        //        [ValidateAntiForgeryToken]
        //        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        //        {
        //            if (!ModelState.IsValid)
        //            {
        //                return View();
        //            }
        //
        //            // Generate the token and send it
        //            if (!await signInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
        //            {
        //                return View("Error");
        //            }
        //            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        //        }
        //
        //        //
        //        // GET: /Account/ExternalLoginCallback
        //        [AllowAnonymous]
        //        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        //        {
        //            var loginInfo = await authenticationManager.GetExternalLoginInfoAsync();
        //            if (loginInfo == null)
        //            {
        //                return RedirectToAction("Login");
        //            }
        //
        //            // Sign in the user with this external login provider if the user already has a login
        //            var result = await signInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
        //            switch (result)
        //            {
        //                case SignInStatus.Success:
        //                    return RedirectToLocal(returnUrl);
        //                case SignInStatus.LockedOut:
        //                    return View("Lockout");
        //                case SignInStatus.RequiresVerification:
        //                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
        //                case SignInStatus.Failure:
        //                default:
        //                    // If the user does not have an account, then prompt the user to create an account
        //                    ViewBag.ReturnUrl = returnUrl;
        //                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
        //                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
        //            }
        //        }
        //
        //        //
        //        // POST: /Account/ExternalLoginConfirmation
        //        [HttpPost]
        //        [AllowAnonymous]
        //        [ValidateAntiForgeryToken]
        //        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        //        {
        //            if (User.Identity.IsAuthenticated)
        //            {
        //                return RedirectToAction("Index", "Manage");
        //            }
        //
        //            if (ModelState.IsValid)
        //            {
        //                // Get the information about the user from the external login provider
        //                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
        //                if (info == null)
        //                {
        //                    return View("ExternalLoginFailure");
        //                }
        //                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        //                var result = await userManager.CreateAsync(user);
        //                if (result.Succeeded)
        //                {
        //                    result = await userManager.AddLoginAsync(user.Id, info.Login);
        //                    if (result.Succeeded)
        //                    {
        //                        await signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //                        return RedirectToLocal(returnUrl);
        //                    }
        //                }
        //                AddErrors(result);
        //            }
        //
        //            ViewBag.ReturnUrl = returnUrl;
        //            return View(model);
        //        }
        //
        //        //
        //        // POST: /Account/LogOff
        //        [HttpPost]
        //        [ValidateAntiForgeryToken]
        //        public ActionResult LogOff()
        //        {
        //            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        //            return RedirectToAction("Index", "Home");
        //        }
        //
        //        //
        //        // GET: /Account/ExternalLoginFailure
        //        [AllowAnonymous]
        //        public ActionResult ExternalLoginFailure()
        //        {
        //            return View();
        //        }
        #endregion
        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion


        
    }
}