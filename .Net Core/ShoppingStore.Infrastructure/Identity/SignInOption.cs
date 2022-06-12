using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using ShoppingStore.Infrastructure.Contracts;

namespace ShoppingStore.Infrastructure.Identity
{
    public class SignInOption<TUser> : ISignInOption<TUser> where TUser : class
    {
        public SignInOption(UserManager<TUser> userManager,
          IApplicationUserManager applicationUserManager,
          IOptions<IdentityOptions> optionsAccessor,
          ILogger<SignInManager<TUser>> logger)
        {
            if (userManager == null)
                throw new ArgumentNullException(nameof(userManager));

            Logger = logger;
            UserManager = userManager;
            ApplicationUserManager = applicationUserManager;
            Options = optionsAccessor?.Value ?? new IdentityOptions();
        }

        public virtual ILogger Logger { get; set; }
        public IdentityOptions Options { get; set; }
        public UserManager<TUser> UserManager { get; set; }
        private readonly IApplicationUserManager ApplicationUserManager;

        public virtual async Task<bool> CanSignInAsync(TUser user)
        {
            if (Options.SignIn.RequireConfirmedEmail && !(await UserManager.IsEmailConfirmedAsync(user)))
            {
                Logger.LogWarning(0, "User {userId} cannot sign in without a confirmed email.", await UserManager.GetUserIdAsync(user));
                return false;
            }
            if (Options.SignIn.RequireConfirmedPhoneNumber && !(await UserManager.IsPhoneNumberConfirmedAsync(user)))
            {
                Logger.LogWarning(1, "User {userId} cannot sign in without a confirmed phone number.", await UserManager.GetUserIdAsync(user));
                return false;
            }

            return true;
        }

        public virtual async Task<SignInResult> PhoneNumberSignInAsync(TUser user, string phoneNumber, bool lockoutOnFailure)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return await CheckPhoneNumberSignInAsync(user, phoneNumber, lockoutOnFailure);
        }

        public virtual async Task<SignInResult> CheckPhoneNumberSignInAsync(TUser user, string phoneNumber, bool lockoutOnFailure)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var error = await PreSignInCheck(user);
            if (error != null)
                return error;

            if (ApplicationUserManager.CheckPhoneNumber(phoneNumber))
            {
                var alwaysLockout = AppContext.TryGetSwitch("Microsoft.AspNetCore.Identity.CheckPhoneNumberSignInAlwaysResetLockoutOnSuccess", out var enabled) && enabled;
                if (alwaysLockout)
                    await UserManager.ResetAccessFailedCountAsync(user);

                return SignInResult.Success;
            }
            Logger.LogWarning(2, "User {userId} failed to provide the correct phoneNumber.", await UserManager.GetUserIdAsync(user));

            if (UserManager.SupportsUserLockout && lockoutOnFailure)
            {
                await UserManager.AccessFailedAsync(user);
                if (await UserManager.IsLockedOutAsync(user))
                    return await LockedOut(user);
            }
            return SignInResult.Failed;
        }

        protected virtual async Task<bool> IsLockedOut(TUser user)
        {
            return UserManager.SupportsUserLockout && await UserManager.IsLockedOutAsync(user);
        }

        protected virtual async Task<SignInResult> LockedOut(TUser user)
        {
            Logger.LogWarning(3, "User {userId} is currently locked out.", await UserManager.GetUserIdAsync(user));
            return SignInResult.LockedOut;
        }

        protected virtual async Task<SignInResult> PreSignInCheck(TUser user)
        {
            if (!await CanSignInAsync(user))
                return SignInResult.NotAllowed;
            if (await IsLockedOut(user))
                return await LockedOut(user);
            return null;
        }
    }
}