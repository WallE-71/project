using System;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Application.ViewModels;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Domain.Common.Attributes;
using ShoppingStore.Application.ViewModels.DynamicAccess;

namespace ShoppingStore.Areas.Admin.Controllers
{
    [DisplayName("مدیریت رنگ ها")]
    public class ColorController : BaseController
    {
        private readonly IUnitOfWork _uw;
        private const string ColorNotFound = "رنگ یافت نشد.";
        public ColorController(IUnitOfWork uw)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
        }

        [HttpGet, DisplayName("مشاهده"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetColors(string search, string order, int offset, int limit, string sort)
        {
            List<BaseViewModel<int>> viewModels;
            int total = _uw.BaseService<Color>().CountEntities();

            if (!search.HasValue())
                search = "";

            if (limit == 0)
                limit = total;

            if (sort == "name")
            {
                if (order == "asc")
                    viewModels = await _uw.AdminColors.GetPaginateColorsAsync(offset, limit, "Name", search);
                else
                    viewModels = await _uw.AdminColors.GetPaginateColorsAsync(offset, limit, "Name desc", search);
            }
            else
                viewModels = await _uw.AdminColors.GetPaginateColorsAsync(offset, limit, "Name", search);

            if (search != "")
                total = viewModels.Count();

            return Json(new { total = total, rows = viewModels });
        }

        [HttpGet, DisplayName("درج و ویرایش"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> RenderColor(int colorId)
        {
            var color = new Color();
            if (colorId != 0)
            {
                var findColor = await _uw.BaseService<Color>().FindByIdAsync(colorId);
                if (findColor != null)
                    color.Name = findColor.Name;
                else
                    ModelState.AddModelError(string.Empty, ColorNotFound);
            }
            return PartialView("_RenderColor", color);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate(Color model)
        {
            if (ModelState.IsValid)
            {
                var findColor = await _uw.BaseService<Color>().FindByIdAsync(model.Id);
                if (model.Id != 0)
                {
                    findColor.Name = model.Name;
                    findColor.UpdateTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now);
                    _uw.BaseService<Color>().Update(findColor);
                    await _uw.BaseService<Color>().Commit();
                    TempData["notification"] = EditSuccess;
                }
                else
                {
                    var color = new Color();
                    color.Name = model.Name;
                    var colors = await _uw.BaseService<Color>().FindByConditionAsync(null, p => p.OrderByDescending(p => p.Id));
                    if (colors.Count() != 0)
                        color.Id = colors.FirstOrDefault().Id + 1;
                    else
                        color.Id = 1;
                    color.InsertTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now);
                    await _uw.BaseService<Color>().CreateAsync(color);
                    await _uw.BaseService<Color>().Commit();
                    TempData["notification"] = InsertSuccess;
                }
            }
            return PartialView("_RenderColor", model);
        }

        [HttpGet, AjaxOnly, DisplayName("حذف"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> Delete(int colorId)
        {
            if (colorId == 0)
                ModelState.AddModelError(string.Empty, ColorNotFound);
            else
            {
                var color = await _uw.BaseService<Color>().FindByIdAsync(colorId);
                if (color == null)
                    ModelState.AddModelError(string.Empty, ColorNotFound);
                else
                    return PartialView("_DeleteConfirmation", color);
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("Delete"), AjaxOnly]
        public async Task<IActionResult> DeleteConfirmed(City model)
        {
            if (model.Id == 0)
                ModelState.AddModelError(string.Empty, ColorNotFound);
            else
            {
                var color = await _uw.BaseService<Color>().FindByIdAsync(model.Id);
                if (color == null)
                    ModelState.AddModelError(string.Empty, ColorNotFound);
                else
                {
                    color.RemoveTime = DateTime.Now;
                    _uw.BaseService<Color>().Update(color);
                    await _uw.BaseService<Color>().Commit();
                    TempData["notification"] = DeleteSuccess;
                    return PartialView("_DeleteConfirmation", color);
                }
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("DeleteGroup"), AjaxOnly, DisplayName("حذف گروهی"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> DeleteGroupConfirmed(string[] btSelectItem)
        {
            if (btSelectItem.Count() == 0)
                ModelState.AddModelError(string.Empty, "هیچ شهری برای حذف انتخاب نشده است.");
            else
            {
                var splited = new string[btSelectItem.Length];
                foreach (var item in btSelectItem)
                    splited = item.Split(',');

                foreach (var splite in splited)
                {
                    var color = await _uw.BaseService<Color>().FindByIdAsync(int.Parse(splite));
                    color.RemoveTime = DateTime.Now;
                    _uw.BaseService<Color>().Update(color);
                }
                await _uw.BaseService<Color>().Commit();
                return Ok(DeleteGroupSuccess);
            }
            return BadRequest(ColorNotFound);
        }
    }
}
