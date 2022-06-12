﻿using System;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using ShoppingStore.Domain.Common.Attributes;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Domain.Entities.Identity;
using ShoppingStore.Infrastructure.Contracts;
using ShoppingStore.Application.ViewModels.RoleManager;
using ShoppingStore.Application.ViewModels.DynamicAccess;

namespace ShoppingStore.Areas.Admin.Controllers
{
    [DisplayName("مدیریت نقش ها")]
    public class RoleManagerController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IApplicationRoleManager _roleManager;
        private const string RoleNotFound = "نقش یافت نشد.";
        public RoleManagerController(IMapper mapper, IApplicationRoleManager roleManager)
        {
            _mapper = mapper;
            _mapper.CheckArgumentIsNull(nameof(_mapper));
            _roleManager = roleManager;
            _roleManager.CheckArgumentIsNull(nameof(_roleManager));
        }

        [HttpGet, DisplayName("مشاهده"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetRoles(string search, string order, int offset, int limit, string sort)
        {
            List<RolesViewModel> roles;
            int total = _roleManager.Roles.Count();

            if (string.IsNullOrWhiteSpace(search))
                search = "";

            if (limit == 0)
                limit = total;

            if (sort == "عنوان نقش")
            {
                if (order == "asc")
                    roles = await _roleManager.GetPaginateRolesAsync(offset, limit, true, search);
                else
                    roles = await _roleManager.GetPaginateRolesAsync(offset, limit, false, search);
            }
            else
                roles = await _roleManager.GetPaginateRolesAsync(offset, limit, null, search);

            if (search != "")
                total = roles.Count();

            return Json(new { total = total, rows = roles });
        }

        [HttpGet, AjaxOnly, DisplayName("درج و ویرایش"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> RenderRole(int? roleId)
        {
            var roleViewModel = new RolesViewModel();
            if (roleId != null)
            {
                var role = await _roleManager.FindByIdAsync(roleId.ToString());
                if (role != null)
                    roleViewModel = _mapper.Map<RolesViewModel>(role);
                else
                    ModelState.AddModelError(string.Empty, RoleNotFound);
            }
            return PartialView("_RenderRole", roleViewModel);
        }

        [HttpPost, AjaxOnly]
        public async Task<IActionResult> CreateOrUpdate(RolesViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result;
                if (viewModel.Id != null)
                {
                    var role = await _roleManager.FindByIdAsync(viewModel.Id.ToString());
                    role.UpdateTime = DateTime.Now;
                    result = await _roleManager.UpdateAsync(_mapper.Map(viewModel, role));
                }
                else
                {
                    viewModel.InsertTime = DateTime.Now;
                    result = await _roleManager.CreateAsync(_mapper.Map<Role>(viewModel));
                }

                if (result.Succeeded)
                    TempData["notification"] = OperationSuccess;
                else
                    ModelState.AddErrorsFromResult(result);
            }
            return PartialView("_RenderRole", viewModel);
        }

        [HttpGet, AjaxOnly, DisplayName("حذف"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> Delete(string roleId)
        {
            if (!roleId.HasValue())
                ModelState.AddModelError(string.Empty, RoleNotFound);
            else
            {
                var role = await _roleManager.FindByIdAsync(roleId.ToString());
                if (role == null)
                    ModelState.AddModelError(string.Empty, RoleNotFound);
                else
                    return PartialView("_DeleteConfirmation", role);
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("Delete"), AjaxOnly]
        public async Task<IActionResult> DeleteConfirmed(Role model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id.ToString());
            if (role == null)
                ModelState.AddModelError(string.Empty, RoleNotFound);
            else
            {
                role.RemoveTime = DateTime.Now;
                var result = await _roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    TempData["notification"] = DeleteSuccess;
                    return PartialView("_DeleteConfirmation", role);
                }
                else
                    ModelState.AddErrorsFromResult(result);
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("DeleteGroup"), AjaxOnly, DisplayName("حذف گروهی"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> DeleteGroupConfirmed(string[] btSelectItem)
        {
            if (btSelectItem.Count() == 0)
                ModelState.AddModelError(string.Empty, "هیچ نقشی برای حذف انتخاب نشده است.");
            else
            {
                var splited = new string[btSelectItem.Length];
                foreach (var item in btSelectItem)
                    splited = item.Split(',');

                foreach (var splite in splited)
                {
                    var role = await _roleManager.FindByIdAsync(splite.ToString());
                    role.RemoveTime = DateTime.Now;
                    await _roleManager.UpdateAsync(role);
                }
                return Ok(DeleteGroupSuccess);
            }
            return BadRequest(RoleNotFound);
        }
    }
}
