using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Infrastructure.Contracts;
using ShoppingStore.Application.ViewModels.Settings;
using ShoppingStore.Application.ViewModels.SiteSetting;
using ShoppingStore.Application.ViewModels.DynamicAccess;

namespace ShoppingStore.Areas.Admin.Controllers
{
    [DisplayName("تنظیمات سایت")]
    public class SiteSettingController : BaseController
    {
        private readonly IWebHostEnvironment _env;
        private readonly IWritableOptions<SiteSettings> _writableLocations;
        public SiteSettingController(IWritableOptions<SiteSettings> writableLocations, IWebHostEnvironment env)
        {
            _env = env;
            _env.CheckArgumentIsNull(nameof(_env));
            _writableLocations = writableLocations;
            _writableLocations.CheckArgumentIsNull(nameof(_writableLocations));
        }

        [HttpGet, DisplayName("مشاهده و ویرایش"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public IActionResult Index()
        {
            var settings = new SettingsViewModel()
            {
                Title = _writableLocations.Value.SiteInfo.Title,
                LogoName = _writableLocations.Value.SiteInfo.Logo,
                EmailHost = _writableLocations.Value.EmailSetting.Host,
                EmailPort = _writableLocations.Value.EmailSetting.Port,
                FaviconName = _writableLocations.Value.SiteInfo.Favicon,
                SenderEmail = _writableLocations.Value.EmailSetting.Email,
                Description = _writableLocations.Value.SiteInfo.Description,
                EmailUsername = _writableLocations.Value.EmailSetting.Username,
                EmailPassword = _writableLocations.Value.EmailSetting.Password,
                MetaDescriptionTag = _writableLocations.Value.SiteInfo.MetaDescriptionTag,
            };

            return View(settings);
        }

        [HttpPost]
        public async Task<IActionResult> Index(SettingsViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.Favicon != null)
                {
                    viewModel.FaviconName = viewModel.Favicon.FileName;
                    await viewModel.Favicon.UploadFileAsync($"{_env.WebRootPath}/assets/img/{viewModel.FaviconName}");
                }
                else
                    viewModel.FaviconName = _writableLocations.Value.SiteInfo.Favicon;

                if (viewModel.Logo != null)
                {
                    viewModel.LogoName = viewModel.Logo.FileName;
                    await viewModel.Logo.UploadFileAsync($"{_env.WebRootPath}/assets/img/{viewModel.LogoName}");
                }
                else
                    viewModel.LogoName = _writableLocations.Value.SiteInfo.Logo;

                _writableLocations.Update(opt =>
                {
                    opt.EmailSetting.Port = viewModel.EmailPort;
                    opt.EmailSetting.Host = viewModel.EmailHost;
                    opt.EmailSetting.Email = viewModel.SenderEmail;
                    opt.EmailSetting.Username = viewModel.EmailUsername;
                    opt.EmailSetting.Password = viewModel.EmailPassword;
                    opt.SiteInfo.Title = viewModel.Title;
                    opt.SiteInfo.Logo = viewModel.LogoName;
                    opt.SiteInfo.Favicon = viewModel.FaviconName;
                    opt.SiteInfo.Description = viewModel.Description;
                    opt.SiteInfo.MetaDescriptionTag = viewModel.MetaDescriptionTag;
                });

                ViewBag.Alert = "ویرایش اطلاعات با موفقیت انجام شد.";
            }
            return View(viewModel);
        }
    }
}
