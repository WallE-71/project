using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Domain.Entities.Identity;
using ShoppingStore.Infrastructure.Contracts;
using ShoppingStore.Application.ViewModels.UserManager;

namespace ShoppingStore.Infrastructure.Identity
{
    public class ApplicationUserManager : UserManager<User>, IApplicationUserManager
    {
        private readonly IServiceProvider _services;
        private readonly IUserStore<User> _userStore;
        private readonly ILookupNormalizer _keyNormalizer;
        private readonly IOptions<IdentityOptions> _options;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<ApplicationUserManager> _logger;
        private readonly ApplicationIdentityErrorDescriber _errors;
        private readonly IEnumerable<IUserValidator<User>> _userValidators;
        private readonly IEnumerable<IPasswordValidator<User>> _passwordValidators;
        public ApplicationUserManager(
            ApplicationIdentityErrorDescriber errors,
            ILookupNormalizer keyNormalizer,
            ILogger<ApplicationUserManager> logger,
            IOptions<IdentityOptions> options,
            IPasswordHasher<User> passwordHasher,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            IServiceProvider services,
            IUserStore<User> userStore,
            IEnumerable<IUserValidator<User>> userValidators)
            : base(userStore, options, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _errors = errors;
            _logger = logger;
            _options = options;
            _services = services;
            _userStore = userStore;
            _keyNormalizer = keyNormalizer;
            _passwordHasher = passwordHasher;
            _userValidators = userValidators;
            _passwordValidators = passwordValidators;
        }

        public async Task<IdentityResult> AddOrUpdateClaimsAsync(int userId, string userClaimType, IList<string> selectedUserClaimValues)
        {
            var user = await FindClaimsInUser(userId);

            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "NotFound",
                    Description = "کاربر مورد نظر یافت نشد.",
                });
            }

            if (selectedUserClaimValues == null)
                selectedUserClaimValues = new List<string>();

            var CurrentUserClaimValues = user.Claims.Where(r => r.ClaimType == userClaimType).Select(r => r.ClaimValue).ToList();
            var newClaimValuesToAdd = selectedUserClaimValues.Except(CurrentUserClaimValues).ToList();
            foreach (var claim in newClaimValuesToAdd)
            {
                user.Claims.Add(new UserClaim
                {
                    UserId = userId,
                    ClaimType = userClaimType,
                    ClaimValue = claim,
                });
            }

            var removedClaimValues = CurrentUserClaimValues.Except(selectedUserClaimValues).ToList();
            foreach (var claim in removedClaimValues)
            {
                var roleClaim = user.Claims.SingleOrDefault(r => r.ClaimValue == claim && r.ClaimType == userClaimType);
                if (roleClaim != null)
                    user.Claims.Remove(roleClaim);
            }
            return await UpdateAsync(user);
        }

        public async Task<List<UserViewModel>> GetPaginateUsersAsync(int offset, int limit, string orderBy, string searchText)
        {
            var getDateTimesForSearch = searchText.GetDateTimeForSearch();
            var users = await Users.Include(u => u.Roles)
                    .Where(t => t.FirstName.Contains(searchText) || t.LastName.Contains(searchText)
                        || t.Email.Contains(searchText) || t.UserName.Contains(searchText)
                        || (t.InsertTime >= getDateTimesForSearch.First() && t.InsertTime <= getDateTimesForSearch.Last()))
                    .OrderBy(orderBy).Skip(offset).Take(limit)
                    .Select(user => new UserViewModel
                    {
                        Id = user.Id,
                        Bio = user.Bio,
                        Email = user.Email,
                        Roles = user.Roles,
                        CityId = user.CityId,
                        Address = user.Address,
                        UserName = user.UserName,
                        LastName = user.LastName,
                        IsActive = user.IsActive,
                        FirstName = user.FirstName,
                        PhoneNumber = user.PhoneNumber.En2Fa(),
                        GenderName = user.Gender == GenderType.Male ? "مرد" : "زن",
                        RoleId = user.Roles.Select(ur => ur.Role.Id).FirstOrDefault(),
                        RoleName = user.Roles.Select(ur => ur.Role.Name).FirstOrDefault(),
                        PersianBirthDate = user.BirthDate.DateTimeEn2Fa("yyyy/MM/dd"),
                        PersianInsertTime = user.InsertTime.DateTimeEn2Fa("yyyy/MM/dd ساعت HH:mm:ss"),
                    }).AsNoTracking().ToListAsync();

            foreach (var viewModel in users)
                viewModel.Row = ++offset;
            return users;
        }

        public async Task<UserViewModel> FindUserWithRolesByIdAsync(int userId)
        {
            var user = await Users
                        .Where(u => u.Id == userId)
                        .Include(u => u.FileStores)
                        .Select(user => new UserViewModel
                        {
                            Id = user.Id,
                            Email = user.Email,
                            Roles = user.Roles,
                            Gender = user.Gender,
                            CityId = user.CityId,
                            Address = user.Address,
                            UserName = user.UserName,
                            LastName = user.LastName,
                            IsActive = user.IsActive,
                            BirthDate = user.BirthDate,
                            FirstName = user.FirstName,
                            LockoutEnd = user.LockoutEnd,
                            InsertTime = user.InsertTime,
                            FileStores = user.FileStores,
                            PhoneNumber = user.PhoneNumber,
                            EmailConfirmed = user.EmailConfirmed,
                            LockoutEnabled = user.LockoutEnabled,
                            TwoFactorEnabled = user.TwoFactorEnabled,
                            AccessFailedCount = user.AccessFailedCount,
                            PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                            RolesId = user.Roles.Select(ur => ur.Role.Id).ToList(),
                            RolesName = user.Roles.Select(ur => ur.Role.Name).ToList(),
                            RoleId = user.Roles.Select(ur => ur.Role.Id).FirstOrDefault(),
                            RoleName = user.Roles.Select(ur => ur.Role.Name).FirstOrDefault(),
                        }).FirstOrDefaultAsync();

            if (user.FileStores.Count() != 0)
                user.Image = user.FileStores.FirstOrDefault(f => f.UserId == userId).ImageName;
            return user;
        }

        public async Task<User> FindByPhoneNumberAsync(string phoneNumber)
        {
            return await Users
                .Where(u => u.PhoneNumber == phoneNumber)
                .Select(user => new User
                {
                    Id = user.Id,
                    //Bio = user.Bio,
                    //Email = user.Email,
                    //Claims = user.Claims,
                    //Gender = user.Gender,
                    //Orders = user.Orders,
                    //Address = user.Address,
                    //UserName = user.UserName,
                    //LastName = user.LastName,
                    //IsActive = user.IsActive,
                    //BirthDate = user.BirthDate,
                    //FirstName = user.FirstName,
                    //Roles = user.Roles,
                    //PhoneNumber = user.PhoneNumber,
                    //SecurityStamp = user.SecurityStamp,
                    //InsertTime = user.InsertTime,
                    //PasswordHash = user.PasswordHash,
                    //NormalizedEmail = user.NormalizedEmail,
                    //ConcurrencyStamp = user.ConcurrencyStamp,
                    //NormalizedUserName = user.NormalizedUserName,
                    //PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                    //AccessFailedCount = user.AccessFailedCount,
                    //TwoFactorEnabled = user.TwoFactorEnabled,
                    //EmailConfirmed = user.EmailConfirmed,
                    //LockoutEnabled = user.LockoutEnabled,
                    //LockoutEnd = user.LockoutEnd
                }).FirstOrDefaultAsync();
        }

        public string CheckAvatarFileName(string fileName)
        {
            var j = 1;
            var fileExtension = Path.GetExtension(fileName);
            var fileNameCount = Users.Where(u => u.FileStores.Where(f => f.UserId == u.Id).FirstOrDefault().ImageName == fileName).Count();
            while (fileNameCount != 0)
            {
                fileName = fileName.Replace(fileExtension, "") + j + fileExtension;
                fileNameCount = Users.Where(u => u.FileStores.Where(f => f.UserId == u.Id).FirstOrDefault().ImageName == fileName).Count();
                j++;
            }
            return fileName;
        }

        public int GetUserID(HttpContext context)
        {
            var userId = 0;
            var principal = context.User as ClaimsPrincipal;
            Int32.TryParse(GetUserId(principal), out userId);
            return userId;
        }

        public bool CheckPhoneNumber(string phoneNumber) => Users.Any(u => u.PhoneNumber == phoneNumber);
        public Task<User> FindClaimsInUser(int userId) => Users.Include(c => c.Claims).FirstOrDefaultAsync(c => c.Id == userId);
    }
}
