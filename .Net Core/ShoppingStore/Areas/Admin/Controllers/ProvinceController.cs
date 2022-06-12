using System;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Common.Attributes;
using ShoppingStore.Application.ViewModels.City;
using ShoppingStore.Application.ViewModels.DynamicAccess;

namespace ShoppingStore.Areas.Admin.Controllers
{
    [DisplayName("مدیریت استان ها")]
    public class ProvinceController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uw;
        private const string ProvinceNotFound = "استان یافت نشد.";
        public ProvinceController(IMapper mapper, IUnitOfWork uw)
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
        public async Task<IActionResult> GetProvinces(string search, string order, int offset, int limit, string sort)
        {
            List<CityProvinceViewModel> viewModels;
            int total = _uw.BaseService<Province>().CountEntities();

            if (!search.HasValue())
                search = "";

            if (limit == 0)
                limit = total;

            if (sort == "name")
            {
                if (order == "asc")
                    viewModels = await _uw.AdminProvinces.GetPaginateProvincesAsync(offset, limit, "Name", search);
                else
                    viewModels = await _uw.AdminProvinces.GetPaginateProvincesAsync(offset, limit, "Name desc", search);
            }
            else
                viewModels = await _uw.AdminProvinces.GetPaginateProvincesAsync(offset, limit, "InsertTime", search);

            if (search != "")
                total = viewModels.Count();

            return Json(new { total = total, rows = viewModels });
        }

        [HttpGet, DisplayName("درج و ویرایش"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> RenderProvince(int provinceId)
        {
            var viewModel = new CityProvinceViewModel();
            if (provinceId != 0)
            {
                var province = await _uw.BaseService<Province>().FindByIdAsync(provinceId);
                if (province != null)
                {
                    viewModel.Id = provinceId;
                    viewModel.ProvinceName = province.Name;
                }
                else
                    ModelState.AddModelError(string.Empty, ProvinceNotFound);
            }
            return PartialView("_RenderProvince", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate(CityProvinceViewModel viewModel)
        {
            ModelState.Remove("CityName");
            if (ModelState.IsValid)
            {
                if (viewModel.Id != 0)
                {
                    var province = await _uw.BaseService<Province>().FindByIdAsync(viewModel.Id);
                    if (province != null)
                    {
                        province.Name = viewModel.ProvinceName;
                        province.UpdateTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now);
                        _uw.BaseService<Province>().Update(province);
                        await _uw.BaseService<Province>().Commit();
                        TempData["notification"] = EditSuccess;
                    }
                    else
                        ModelState.AddModelError(string.Empty, ProvinceNotFound);
                }
                else
                {
                    var province = new Province();
                    province.Name = viewModel.ProvinceName;
                    var provinces = await _uw.BaseService<Province>().FindByConditionAsync(null, p => p.OrderByDescending(p => p.Id));
                    if (provinces.Count() != 0)
                        province.Id = provinces.FirstOrDefault().Id + 1;
                    else
                        province.Id = 1;
                    province.InsertTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now);
                    await _uw.BaseService<Province>().CreateAsync(province);
                    await _uw.BaseService<Province>().Commit();
                    TempData["notification"] = InsertSuccess;
                }
            }
            return PartialView("_RenderProvince", viewModel);
        }

        [HttpGet, AjaxOnly, DisplayName("حذف"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> Delete(int provinceId)
        {
            if (provinceId == 0)
                ModelState.AddModelError(string.Empty, ProvinceNotFound);
            else
            {
                var province = await _uw.BaseService<Province>().FindByIdAsync(provinceId);
                if (province == null)
                    ModelState.AddModelError(string.Empty, ProvinceNotFound);
                else
                    return PartialView("_DeleteConfirmation", province);
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("Delete"), AjaxOnly]
        public async Task<IActionResult> DeleteConfirmed(Province model)
        {
            if (model.Id == 0)
                ModelState.AddModelError(string.Empty, ProvinceNotFound);
            else
            {
                var province = await _uw.BaseService<Province>().FindByIdAsync(model.Id);
                if (province == null)
                    ModelState.AddModelError(string.Empty, ProvinceNotFound);
                else
                {
                    province.RemoveTime = DateTime.Now;
                    _uw.BaseService<Province>().Update(province);
                    await _uw.BaseService<Province>().Commit();
                    TempData["notification"] = DeleteSuccess;
                    return PartialView("_DeleteConfirmation", province);
                }
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("DeleteGroup"), AjaxOnly, DisplayName("حذف گروهی"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> DeleteGroupConfirmed(string[] btSelectItem)
        {
            if (btSelectItem.Count() == 0)
                ModelState.AddModelError(string.Empty, "هیچ استانی برای حذف انتخاب نشده است.");
            else
            {
                var splited = new string[btSelectItem.Length];
                foreach (var item in btSelectItem)
                    splited = item.Split(',');

                foreach (var splite in splited)
                {
                    var province = await _uw.BaseService<Province>().FindByIdAsync(int.Parse(splite));
                    province.RemoveTime = DateTime.Now;
                    _uw.BaseService<Province>().Update(province);
                }
                await _uw.BaseService<Province>().Commit();
                return Ok(DeleteGroupSuccess);
            }
            return BadRequest(ProvinceNotFound);
        }
    }
}
