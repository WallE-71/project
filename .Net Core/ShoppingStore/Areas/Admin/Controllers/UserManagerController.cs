using System;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Domain.Entities.Identity;
using ShoppingStore.Infrastructure.Contracts;
using ShoppingStore.Application.ViewModels.CreditCart;
using ShoppingStore.Application.ViewModels.UserManager;
using ShoppingStore.Application.ViewModels.DynamicAccess;

namespace ShoppingStore.Areas.Admin.Controllers
{
    [DisplayName("مدیریت کاربران")]
    public class UserManagerController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uw;
        private readonly IWebHostEnvironment _env;
        private readonly IApplicationUserManager _userManager;
        private readonly IApplicationRoleManager _roleManager;
        private const string UserNotFound = "کاربر یافت نشد.";
        public UserManagerController(
            IMapper mapper,
            IUnitOfWork uw,
            IWebHostEnvironment env,
            IApplicationUserManager userManager,
            IApplicationRoleManager roleManager)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
            _env = env;
            _env.CheckArgumentIsNull(nameof(_env));
            _mapper = mapper;
            _mapper.CheckArgumentIsNull(nameof(_mapper));
            _userManager = userManager;
            _userManager.CheckArgumentIsNull(nameof(_userManager));
            _roleManager = roleManager;
            _roleManager.CheckArgumentIsNull(nameof(_roleManager));
        }

        [HttpGet, DisplayName("مشاهده"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetUsers(string search, string order, int offset, int limit, string sort)
        {
            List<UserViewModel> users;
            int total = _userManager.Users.Count();

            if (string.IsNullOrWhiteSpace(search))
                search = "";

            if (limit == 0)
                limit = total;

            if (sort == "firstName")
            {
                if (order == "asc")
                    users = await _userManager.GetPaginateUsersAsync(offset, limit, "FirstName", search);
                else
                    users = await _userManager.GetPaginateUsersAsync(offset, limit, "FirstName desc", search);
            }
            else if (sort == "lastName")
            {
                if (order == "asc")
                    users = await _userManager.GetPaginateUsersAsync(offset, limit, "LastName", search);
                else
                    users = await _userManager.GetPaginateUsersAsync(offset, limit, "LastName desc", search);
            }
            else if (sort == "userName")
            {
                if (order == "asc")
                    users = await _userManager.GetPaginateUsersAsync(offset, limit, "UserName", search);
                else
                    users = await _userManager.GetPaginateUsersAsync(offset, limit, "UserName desc", search);
            }
            else if (sort == "insertTime")
            {
                if (order == "asc")
                    users = await _userManager.GetPaginateUsersAsync(offset, limit, "InsertTime", search);
                else
                    users = await _userManager.GetPaginateUsersAsync(offset, limit, "InsertTime desc", search);
            }
            else
                users = await _userManager.GetPaginateUsersAsync(offset, limit, "InsertTime", search);

            if (search != "")
                total = users.Count;

            foreach (var viewModel in users)
            {
                var roleIds = viewModel.Roles.Select(ur => ur.RoleId).ToList();
                viewModel.RolesId.AddRange(roleIds);
                foreach (var roleId in roleIds)
                {
                    var role = await _uw.BaseService<Role>().GetFirstOrDefaultAsync(ur => ur.Id == roleId);
                    viewModel.RolesName.Add(role.Name);
                }

                var fileStore = await _uw.BaseService<FileStore>().GetFirstOrDefaultAsync(f => f.UserId == viewModel.Id);
                if (fileStore != null)
                    viewModel.Image = fileStore.ImageName;

                var city = await _uw.BaseService<City>().GetFirstOrDefaultAsync(c => c.Id == viewModel.CityId);
                if (city != null)
                {
                    viewModel.CityName = city.Name;
                    var province = await _uw.BaseService<Province>().GetFirstOrDefaultAsync(c => c.Id == city.ProvinceId);
                    viewModel.ProvinceName = province.Name;
                }
            }

            return Json(new { total = total, rows = users });
        }

        [HttpGet, DisplayName("درج و ویرایش"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> RenderUser(int? userId)
        {
            var viewModel = new UserViewModel();
            var userWithRoles = new UserViewModel();
            ViewBag.Roles = _roleManager.GetAllRoles();
            if (userId != null)
            {
                userWithRoles = await _userManager.FindUserWithRolesByIdAsync((int)userId);
                var roleIds = await _uw.AdminUsers.GetRoleIdsAsync(userId);
                viewModel = _mapper.Map<UserViewModel>(userWithRoles);
                viewModel.PersianBirthDate = viewModel.BirthDate.ConvertMiladiToShamsi("yyyy/MM/dd");
                viewModel.ListOfRoles = new ListOfRolesViewModel(_roleManager.GetAllRoles(), roleIds);
                var city = await _uw.BaseService<City>().GetFirstOrDefaultAsync(f => f.Id == userWithRoles.CityId);
                if(city != null)
                    viewModel.CityName = city.Name;
            }
            viewModel.Cities = await _uw.AdminCities.GetCitiesAsync();
            return PartialView("_RenderUser", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate(UserViewModel viewModel)
        {
            ViewBag.Roles = _roleManager.GetAllRoles();
            viewModel.ListOfRoles = new ListOfRolesViewModel(_roleManager.GetAllRoles(), viewModel.RoleIds);
            if (viewModel.Id != null)
            {
                ModelState.Remove("ImageFile");
                ModelState.Remove("Password");
                ModelState.Remove("ConfirmPassword");
            }

            if (ModelState.IsValid)
            {
                var city = await _uw.BaseService<City>().GetFirstOrDefaultAsync(c => c.Name == viewModel.CityName);
                IdentityResult result = null;
                if (viewModel.ImageFile != null)
                    viewModel.Image = _userManager.CheckAvatarFileName(viewModel.ImageFile.FileName);

                viewModel.BirthDate = viewModel.PersianBirthDate.ConvertShamsiToMiladi();
                if (viewModel.Id != null)
                {
                    var user = await _userManager.FindByIdAsync(viewModel.Id.ToString());
                    user.Roles = null;
                    user.Bio = viewModel.Bio;
                    user.Email = viewModel.Email;
                    user.UpdateTime = DateTime.Now;
                    user.Address = viewModel.Address;
                    user.LastName = viewModel.LastName;
                    user.UserName = viewModel.UserName;
                    user.FirstName = viewModel.FirstName;
                    user.BirthDate = viewModel.BirthDate;
                    user.Gender = viewModel.Gender.Value;
                    user.CityId = city == null ? 0 : city.Id;
                    user.PhoneNumber = viewModel.PhoneNumber;
                    var tempString = await _userManager.GetRolesAsync(user);
                    result = await _userManager.RemoveFromRolesAsync(user, tempString);

                    var newRoles = new List<string>();
                    foreach (var id in viewModel.ListOfRoles.RoleIds)
                    {
                        var tempRole = _roleManager.Roles.FirstOrDefault(r => r.Id == id);
                        newRoles.Add(tempRole.Name);
                    }
                    result = await _userManager.AddToRolesAsync(user, newRoles);
                    if (result.Succeeded)
                        result = await _userManager.UpdateAsync(user);
                }
                else
                {
                    if (viewModel.RoleIds != null)
                        viewModel.Roles = viewModel.RoleIds.Select(c => new UserRole { RoleId = c }).ToList();
                    else
                        viewModel.Roles = null;

                    viewModel.EmailConfirmed = true;
                    viewModel.CityId = city == null ? 0 : city.Id;
                    viewModel.InsertTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now);
                    result = await _userManager.CreateAsync(_mapper.Map<User>(viewModel), viewModel.Password);
                }

                if (result.Succeeded)
                {
                    if (viewModel.ImageFile != null)
                    {
                        var lastImage = await _uw.AdminFiles.FindImageAsync(null, null, null, viewModel.Id);
                        await InsertImage(viewModel.ImageFile, viewModel.Image, viewModel.Id, lastImage);
                    }
                    TempData["notification"] = OperationSuccess;
                }
                else
                    ModelState.AddErrorsFromResult(result);
            }
            return PartialView("_RenderUser", viewModel);
        }

        [HttpGet, DisplayName("حذف"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> Delete(string userId)
        {
            if (!userId.HasValue())
                ModelState.AddModelError(string.Empty, UserNotFound);
            else
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    ModelState.AddModelError(string.Empty, UserNotFound);
                else
                    return PartialView("_DeleteConfirmation", user);
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(User model)
        {
            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user == null)
                ModelState.AddModelError(string.Empty, UserNotFound);
            else
            {
                user.IsActive = false;
                user.RemoveTime = DateTime.Now;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    var lastImage = await _uw.AdminFiles.FindImageAsync(null, 0, 0, user.Id);
                    FileExtensions.DeleteFile($"{_env.WebRootPath}/avatars/{lastImage}");
                    TempData["notification"] = DeleteSuccess;
                    return PartialView("_DeleteConfirmation", user);
                }
                else
                    ModelState.AddErrorsFromResult(result);
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("DeleteGroup"), DisplayName("حذف گروهی"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> DeleteGroupConfirmed(string[] btSelectItem)
        {
            if (btSelectItem.Count() == 0)
                ModelState.AddModelError(string.Empty, "هیچ کاربری برای حذف انتخاب نشده است.");
            else
            {
                var splited = new string[btSelectItem.Length];
                foreach (var item in btSelectItem)
                    splited = item.Split(',');

                foreach (var splite in splited)
                {
                    var user = await _userManager.FindByIdAsync(splite);
                    user.IsActive = false;
                    user.RemoveTime = DateTime.Now;
                    await _userManager.UpdateAsync(user);
                }
                return Ok(DeleteGroupSuccess);
            }
            return BadRequest(UserNotFound);
        }

        [HttpGet, DisplayName("مدیریت کاربر"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> Details(int userId)
        {
            if (userId == 0)
                return NotFound();
            else
            {
                var user = await _userManager.FindUserWithRolesByIdAsync(userId);
                if (user == null)
                    return NotFound();
                else
                    return View(user);
            }
        }

        [HttpGet, DisplayName("ویرایش اطلاعات")]
        public async Task<IActionResult> EditInfo(int userId)
        {
            var viewModel = new UserViewModel();
            if (userId != 0)
            {
                viewModel = _mapper.Map<UserViewModel>(await _userManager.FindUserWithRolesByIdAsync((int)userId));
                viewModel.PersianBirthDate = viewModel.BirthDate.ConvertMiladiToShamsi("yyyy/MM/dd");
            }
            else
                ModelState.AddModelError(string.Empty, UserNotFound);
            return PartialView("_EditInfoUser", viewModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditInfo(UserViewModel viewModel)
        {
            var user = await _userManager.FindByIdAsync(viewModel.Id.ToString());
            user.Bio = viewModel.Bio;
            user.Email = viewModel.Email;
            user.Address = viewModel.Address;
            user.LastName = viewModel.LastName;
            user.UserName = viewModel.UserName;
            user.FirstName = viewModel.FirstName;
            user.Gender = viewModel.Gender.Value;
            user.PhoneNumber = viewModel.PhoneNumber;
            user.BirthDate = viewModel.PersianBirthDate.ConvertShamsiToMiladi();
            IdentityResult result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                TempData["notification"] = OperationSuccess;
            else
                ModelState.AddErrorsFromResult(result);
            return RedirectToAction("Details", new { userId = user.Id });
        }

        [HttpGet]
        public IActionResult IncreaseOfCredit(int userId)
        {
            var viewModel = new CreditCartViewModel();
            if (userId == 0)
                ModelState.AddModelError(string.Empty, UserNotFound);
            else
            {
                var creditCart = _uw.BaseService<CreditCart>().FindByConditionAsync(c => c.UserId == userId).Result.FirstOrDefault();
                if (creditCart != null)
                    viewModel = _mapper.Map<CreditCartViewModel>(creditCart);
                viewModel.UserId = userId;
                return PartialView("_IncreaseOfCredit", viewModel);
            }
            return PartialView("_IncreaseOfCredit", viewModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> IncreaseOfCredit(CreditCartViewModel viewModel)
        {
            ModelState.Remove("Name");
            if (ModelState.IsValid)
            {
                if (viewModel.Credit < 80000)
                {
                    ViewBag.AlertSuccess = "حداقل مبلغ حساب باید ۸۰۰۰۰ تومان باشد!";
                    return PartialView("_IncreaseOfCredit", viewModel);
                }
                if (viewModel.Credit > 9999999)
                {
                    ViewBag.AlertSuccess = "رقم اعتبار نباید بیشتر از ۹۹۹۹۹۹۹ تومان باشد!";
                    return PartialView("_IncreaseOfCredit", viewModel);
                }
                var credit = _uw.BaseService<CreditCart>().FindByConditionAsync(c => c.UserId == viewModel.UserId).Result.FirstOrDefault();
                if (credit == null)
                {
                    viewModel.InsertTime = DateTime.Now;
                    await _uw.BaseService<CreditCart>().CreateAsync(_mapper.Map<CreditCart>(viewModel));
                }
                else
                {
                    viewModel.UpdateTime = DateTime.Now;
                    _uw.BaseService<CreditCart>().Update(_mapper.Map<CreditCart>(viewModel));
                }
                await _uw.BaseService<CreditCart>().Commit();
                TempData["notification"] = OperationSuccess;
            }
            return RedirectToAction("Details", new { userId = viewModel.UserId });
        }

        public async Task InsertImage(IFormFile file, string imageName, int? userId, string lastImage)
        {
            var images = new List<string>();
            images.Add(imageName);
            await file.UploadFileAsync($"{_env.WebRootPath}/assets/images/avatars/{imageName}");
            if (lastImage.HasValue())
            {
                FileExtensions.DeleteFile($"{_env.WebRootPath}/assets/images/avatars/{lastImage}");
                await _uw.AdminFiles.UpdateImagesAsync(null, null, null, userId, images);
                return;
            }
            await _uw.AdminFiles.InsertImagesAsync(null, null, null, userId, images);
        }

        /// <summary>
        /// نمایش صفحه بازنشانی کلمه عبور
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ResetPassword(int userId)
        {
            var User = await _userManager.FindByIdAsync(userId.ToString());
            if (User == null)
                return NotFound();

            var viewModel = new ResetPasswordViewModel
            {
                UserId = userId,
                Email = User.Email,
            };
            return View(viewModel);
        }

        /// <summary>
        /// انجام عملیات بازنشانی کلمه عبور
        /// </summary>
        /// <param name="ViewModel"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var User = await _userManager.FindByIdAsync(viewModel.UserId.ToString());
                if (User == null)
                    return NotFound();

                await _userManager.RemovePasswordAsync(User);
                var result = await _userManager.AddPasswordAsync(User, viewModel.NewPassword);
                if (result.Succeeded)
                    ViewBag.AlertSuccess = "بازنشانی کلمه عبور با موفقیت انجام شد.";
                else
                    ModelState.AddErrorsFromResult(result);

                viewModel.Email = User.Email;
            }
            return View(viewModel);
        }

        /// <summary>
        /// فعال و غیر فعال کردن فقل حساب کاربر
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ChangeLockOutEnable(int userId)
        {
            string ResultJsonData;
            var User = await _userManager.FindByIdAsync(userId.ToString());
            if (User == null)
                return NotFound();
            else
            {
                if (User.LockoutEnabled)
                {
                    User.LockoutEnabled = false;
                    ResultJsonData = "غیرفعال";
                }
                else
                {
                    User.LockoutEnabled = true;
                    ResultJsonData = "فعال";
                }
                await _userManager.UpdateAsync(User);
                return Json(ResultJsonData);
            }
        }

        /// <summary>
        /// فعال و غیر فعال کردن کاربر
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> InActiveOrActiveUser(int userId)
        {
            string ResultJsonData;
            var User = await _userManager.FindByIdAsync(userId.ToString());
            if (User == null)
                return NotFound();

            if (User.IsActive)
            {
                User.IsActive = false;
                ResultJsonData = "غیرفعال";
            }
            else
            {
                User.IsActive = true;
                ResultJsonData = "فعال";
            }
            await _userManager.UpdateAsync(User);
            return Json(ResultJsonData);
        }

        /// <summary>
        /// فعال و غیر فعال کردن احرازهویت دو مرحله ای
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ChangeTwoFactorEnabled(int userId)
        {
            string ResultJsonData;
            var User = await _userManager.FindByIdAsync(userId.ToString());
            if (User == null)
                return NotFound();

            if (User.TwoFactorEnabled)
            {
                User.TwoFactorEnabled = false;
                ResultJsonData = "غیرفعال";
            }
            else
            {
                User.TwoFactorEnabled = true;
                ResultJsonData = "فعال";
            }
            await _userManager.UpdateAsync(User);
            return Json(ResultJsonData);
        }

        /// <summary>
        /// تایید و عدم تایید وضعیت ایمیل کاربر
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ChangeEmailConfirmed(int userId)
        {
            string ResultJsonData;
            var User = await _userManager.FindByIdAsync(userId.ToString());
            if (User == null)
                return NotFound();

            if (User.EmailConfirmed)
            {
                ResultJsonData = "تایید نشده";
                User.EmailConfirmed = false;
            }
            else
            {
                User.EmailConfirmed = true;
                ResultJsonData = "تایید شده";
            }
            var Result = await _userManager.UpdateAsync(User);
            return Json(ResultJsonData);
        }

        /// <summary>
        /// تایید و عدم تایید وضعیت شماره موبایل کاربر
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ChangePhoneNumberConfirmed(int userId)
        {
            string ResultJsonData;
            var User = await _userManager.FindByIdAsync(userId.ToString());
            if (User == null)
                return NotFound();

            if (User.PhoneNumberConfirmed)
            {
                ResultJsonData = "تایید نشده";
                User.PhoneNumberConfirmed = false;
            }
            else
            {
                ResultJsonData = "تایید شده";
                User.PhoneNumberConfirmed = true;
            }
            var Result = await _userManager.UpdateAsync(User);
            return Json(ResultJsonData);
        }

        /// <summary>
        /// قفل و خروج از حالت قفل حساب کاربر
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> LockOrUnLockUserAccount(int userId)
        {
            string ResultJsonData;
            var User = await _userManager.FindByIdAsync(userId.ToString());
            if (User == null)
                return NotFound();

            if (User.LockoutEnd == null)
            {
                ResultJsonData = "قفل شده";
                User.LockoutEnd = DateTimeOffset.UtcNow.AddMinutes(20);
            }
            else
            {
                if (User.LockoutEnd > DateTime.Now)
                {
                    ResultJsonData = "قفل نشده";
                    User.LockoutEnd = null;
                }
                else
                {
                    ResultJsonData = "قفل شده";
                    User.LockoutEnd = DateTimeOffset.UtcNow.AddMinutes(20);
                }
            }
            var Result = await _userManager.UpdateAsync(User);
            return Json(ResultJsonData);
        }
    }
}
