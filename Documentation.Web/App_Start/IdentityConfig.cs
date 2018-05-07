using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Documentation.Web.Models;
using Documentation.Data.DAL.Intefraces;
using Documentation.Data.Entities;
using Documentation.Web.Helper;
using Documentation.Web.Identity;

namespace Documentation.Web
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        private readonly ICustomStore<ApplicationUser> _store;
        public ApplicationUserManager(ICustomStore<ApplicationUser> store)
            : base(store)
        {
            _store = store;
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new DocumentationUserStore(DocumentationContainerHelper.Container.GetInstance<IRepository<User>>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = 
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
        public async Task<ApplicationUser> Login(string userName, string password)
        {
            return await _store.FindByPasswordAsync(userName, password);
        }
        public override async Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        {
            var dbUser = await _store.FindByNameAsync(user.Email);
            if (dbUser != null)
                return IdentityResult.Failed(new string[] { "This email is already registered!" });
            user.PasswordHash = SecurityHelper.EncryptPassword(password);
            await _store.CreateAsync(user);
            return IdentityResult.Success;
        }
        public override async Task<IdentityResult> UpdateAsync(ApplicationUser user)
        {
            await _store.UpdateAsync(user);
            return IdentityResult.Success;
        }
        public override async Task<ApplicationUser> FindAsync(string userName, string password)
        {
            var user = await _store.FindByNameAsync(userName);
            if (user != null && user.PasswordHash == password)
                return user;
            return null;
        }
        public override async Task<ApplicationUser> FindByIdAsync(string userId)
        {
            return await _store.FindByIdAsync(userId);
        }
        public override async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            return await _store.FindByNameAsync(email);
        }

        public override async Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            if (Guid.TryParse(userId, out Guid Id))
            {
                return await _store.UpdatePassword(Id, currentPassword, newPassword);
            }
            return IdentityResult.Failed(new string[] { "problem retreving user data!" });
        }

        public override async Task<IdentityResult> AddPasswordAsync(string userId, string password)
        {
            if (Guid.TryParse(userId, out Guid Id))
            {
                return await _store.AddPassword(Id, password);
            }
            return IdentityResult.Failed(new string[] { "problem retreving user data!" }); ;
        }


        public override async Task<ClaimsIdentity> CreateIdentityAsync(ApplicationUser user, string authenticationType)
        {
            var claim = await base.CreateIdentityAsync(user, authenticationType);
            claim.AddClaim(new Claim("UserId", user.Id.ToString()));
            claim.AddClaim(new Claim("UserName", user.Email));
            claim.AddClaim(new Claim("FirstName", user.FirstName ?? ""));
            claim.AddClaim(new Claim("LastName", user.LastName ?? ""));
            claim.AddClaim(new Claim("FullName", user.FullName ?? ""));
            return claim;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        private readonly ApplicationUserManager _userManager;
        private readonly IAuthenticationManager _authenticationManager;
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
            _userManager = userManager;
            _authenticationManager = authenticationManager;

        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(new ApplicationUserManager(new DocumentationUserStore(DocumentationContainerHelper.Container.GetInstance<IRepository<User>>())), context.Authentication);
        }
        public override async Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
        {
            var user = await _userManager.Login(userName, SecurityHelper.EncryptPassword(password));
            if (user != null)
            {
                _authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                var identity = await _userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                _authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent, ExpiresUtc = DateTime.UtcNow.AddDays(14) }, identity);

                return SignInStatus.Success;
            }
            return SignInStatus.Failure;
        }
        public override async Task SignInAsync(ApplicationUser user, bool isPersistent, bool rememberBrowser)
        {
            var dbUser = await _userManager.Login(user.Email, user.PasswordHash);
            if (dbUser != null)
            {
                _authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                var identity = await _userManager.CreateIdentityAsync(dbUser, DefaultAuthenticationTypes.ApplicationCookie);
                _authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent, ExpiresUtc = DateTime.UtcNow.AddDays(14) }, identity);

            }
        }
    }
    public class ApplicationClaimsIdentityFactory : ClaimsIdentityFactory<ApplicationUser>
    {
        // This claim value is taken from Login View
        public static readonly string userIdKey = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
        public static readonly string userNameKey = "UserName";
        public string UserIdValue { get; set; }
        public string UserNameValue { get; set; }

        public async override Task<ClaimsIdentity> CreateAsync(UserManager<ApplicationUser, string> manager, ApplicationUser user, string authenticationType)
        {
            var identity = await base.CreateAsync(manager, user, authenticationType);
            identity.AddClaim(new Claim(userIdKey, UserIdValue));
            identity.AddClaim(new Claim(userNameKey, UserNameValue));
            return identity;
        }
    }
}
