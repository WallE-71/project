using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Application.ViewModels.Newsletter;
using ShoppingStore.Application.ViewModels.DynamicAccess;

namespace ShoppingStore.Areas.Admin.Controllers
{
    [DisplayName("مدیریت خبرنامه ")]
    public class NewsletterController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uw;
        private const string EmailNotFound = "ایمیل یافت نشد...";
        public const string RegisterSuccess = "عضویت شما در خبرنامه با موفقیت انجام شد.";
        public NewsletterController(IMapper mapper, IUnitOfWork uw)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
            _mapper = mapper;
            _mapper.CheckArgumentIsNull(nameof(_mapper));
        }

        [HttpGet, DisplayName("مشاهده"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetNewsletters(string search, string order, int offset, int limit, string sort)
        {
            List<NewsletterViewModel> newsletter;
            int total = _uw.BaseService<Newsletter>().CountEntities();

            if (!search.HasValue())
                search = "";

            if (limit == 0)
                limit = total;

            if (sort == "Id")
            {
                if (order == "asc")
                    newsletter = await _uw.AdminNewsletters.GetPaginateNewsletterAsync(offset, limit, "Email", search);
                else
                    newsletter = await _uw.AdminNewsletters.GetPaginateNewsletterAsync(offset, limit, "Email desc", search);
            }
            else if (sort == "تاریخ عضویت")
            {
                if (order == "asc")
                    newsletter = await _uw.AdminNewsletters.GetPaginateNewsletterAsync(offset, limit, "InsertTime", search);
                else
                    newsletter = await _uw.AdminNewsletters.GetPaginateNewsletterAsync(offset, limit, "InsertTime desc", search);
            }
            else
                newsletter = await _uw.AdminNewsletters.GetPaginateNewsletterAsync(offset, limit, "InsertTime desc", search);

            if (search != "")
                total = newsletter.Count();

            return Json(new { total = total, rows = newsletter });
        }

        [HttpGet]
        public async Task<IActionResult> ActiveOrInActiveMembers(string email)
        {
            if (email.HasValue())
            {
                var newsletter = await _uw.AdminNewsletters.FindByEmailAsync(email);
                if (newsletter != null)
                {
                    if (newsletter.IsComplete)
                        newsletter.IsComplete = false;
                    else
                        newsletter.IsComplete = true;

                    _uw.BaseService<Newsletter>().Update(newsletter);
                    await _uw.BaseService<Newsletter>().Commit();
                    return Json("Success");
                }
            }
            return Json($"عضوی با ایمیل '{email}' یافت نشد !!!");
        }

        [HttpGet, DisplayName("حذف"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> Delete(string email)
        {
            if (!email.HasValue())
                ModelState.AddModelError(string.Empty, EmailNotFound);
            else
            {
                var newsletter = await _uw.AdminNewsletters.FindByEmailAsync(email);
                if (newsletter == null)
                    ModelState.AddModelError(string.Empty, EmailNotFound);
                else
                    return PartialView("_DeleteConfirmation", newsletter);
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Newsletter model)
        {
            if (!model.Email.HasValue())
                ModelState.AddModelError(string.Empty, EmailNotFound);
            else
            {
                var newsletter = await _uw.AdminNewsletters.FindByEmailAsync(model.Email);
                if (newsletter == null)
                    ModelState.AddModelError(string.Empty, EmailNotFound);
                else
                {
                    newsletter.RemoveTime = DateTime.Now;
                    _uw.BaseService<Newsletter>().Update(newsletter);
                    await _uw.BaseService<Newsletter>().Commit();
                    TempData["notification"] = DeleteSuccess;
                    return PartialView("_DeleteConfirmation", newsletter);
                }
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
                    var newsletter = await _uw.BaseService<Newsletter>().FindByIdAsync(int.Parse(splite));
                    newsletter.RemoveTime = DateTime.Now;
                    _uw.BaseService<Newsletter>().Update(newsletter);
                }
                await _uw.BaseService<Newsletter>().Commit();
                return Ok(DeleteGroupSuccess);
            }
            return BadRequest(EmailNotFound);
        }

        public async Task<IActionResult> RegisterInNewsLetter(NewsletterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _uw.BaseService<Newsletter>().FindByIdAsync(viewModel.Email);
                if (user == null)
                {
                    await _uw.BaseService<Newsletter>().CreateAsync(new Newsletter(viewModel.Email));
                    await _uw.BaseService<Newsletter>().Commit();
                    TempData["notification"] = RegisterSuccess;
                }
                else
                {
                    if (user.IsComplete == true)
                        ModelState.AddModelError(string.Empty, $"شما با ایمیل '{viewModel.Email}' قبلا عضو خبرنامه شده اید.");
                    else
                    {
                        user.IsComplete = true;
                        await _uw.BaseService<Newsletter>().Commit();
                        TempData["notification"] = RegisterSuccess;
                    }
                }
            }
            return PartialView("_RegisterInNewsLetter");
        }
    }
}
