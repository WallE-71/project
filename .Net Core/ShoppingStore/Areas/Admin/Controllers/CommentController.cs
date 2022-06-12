using System;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Application.ViewModels.Comments;
using ShoppingStore.Application.ViewModels.DynamicAccess;

namespace ShoppingStore.Areas.Admin.Controllers
{
    [DisplayName("مدیریت دیدگاه ها")]
    public class CommentController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uw;
        private const string CommentNotFound = "دیدگاه یافت نشد.";
        public CommentController(IMapper mapper, IUnitOfWork uw)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
            _mapper = mapper;
            _mapper.CheckArgumentIsNull(nameof(_mapper));
        }

        [HttpGet, DisplayName("مشاهده"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public IActionResult Index(string productId, bool? isConfirm)
        {
            return View(nameof(Index), new CommentViewModel { ProductId = productId, IsComplete = isConfirm });
        }

        [HttpGet]
        public async Task<IActionResult> GetComments(string search, string order, int offset, int limit, string sort, string prodcutId, bool? isConfirm)
        {
            List<CommentViewModel> comments;
            int total = _uw.BaseService<Comment>().CountEntities();

            if (!search.HasValue())
                search = "";

            if (limit == 0)
                limit = total;

            if (!prodcutId.HasValue())
                prodcutId = "";

            if (sort == "نام")
            {
                if (order == "asc")
                    comments = await _uw.AdminComments.GetPaginateCommentsAsync(offset, limit, "Name", search, prodcutId, isConfirm);
                else
                    comments = await _uw.AdminComments.GetPaginateCommentsAsync(offset, limit, "Name desc", search, prodcutId, isConfirm);
            }
            else if (sort == "ایمیل")
            {
                if (order == "asc")
                    comments = await _uw.AdminComments.GetPaginateCommentsAsync(offset, limit, "Email", search, prodcutId, isConfirm);
                else
                    comments = await _uw.AdminComments.GetPaginateCommentsAsync(offset, limit, "Email desc", search, prodcutId, isConfirm);
            }
            else if (sort == "تاریخ ارسال")
            {
                if (order == "asc")
                    comments = await _uw.AdminComments.GetPaginateCommentsAsync(offset, limit, "InsertTime", search, prodcutId, isConfirm);
                else
                    comments = await _uw.AdminComments.GetPaginateCommentsAsync(offset, limit, "InsertTime desc", search, prodcutId, isConfirm);
            }
            else
                comments = await _uw.AdminComments.GetPaginateCommentsAsync(offset, limit, "InsertTime desc", search, prodcutId, isConfirm);

            if (search != "")
                total = comments.Count();

            return Json(new { total = total, rows = comments });
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmOrInConfirm(int commentId)
        {
            if (commentId != 0)
            {
                var comment = await _uw.BaseService<Comment>().FindByIdAsync(commentId);
                if (comment != null)
                {
                    if (comment.IsComplete)
                        comment.IsComplete = false;
                    else
                        comment.IsComplete = true;

                    _uw.BaseService<Comment>().Update(comment);
                    await _uw.BaseService<Comment>().Commit();
                    return Json("Success");
                }
            }
            return Json($"عضوی با شناسه '{commentId}' یافت نشد !!!");
        }

        [HttpGet, DisplayName("حذف"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> Delete(int commentId)
        {
            if (commentId == 0)
                ModelState.AddModelError(string.Empty, CommentNotFound);
            else
            {
                var comment = await _uw.BaseService<Comment>().FindByIdAsync(commentId);
                if (comment == null)
                    ModelState.AddModelError(string.Empty, CommentNotFound);
                else
                    return PartialView("_DeleteConfirmation", comment);
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Comment model)
        {
            if (model.Id == 0)
                ModelState.AddModelError(string.Empty, CommentNotFound);
            else
            {
                var comment = await _uw.BaseService<Comment>().FindByIdAsync(model.Id);
                if (comment == null)
                    ModelState.AddModelError(string.Empty, CommentNotFound);
                else
                {
                    _uw.BaseService<Comment>().Delete(comment);
                    await _uw.BaseService<Comment>().Commit();
                    TempData["notification"] = DeleteSuccess;
                    return PartialView("_DeleteConfirmation", comment);
                }
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("DeleteGroup"), DisplayName("حذف گروهی"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> DeleteGroupConfirmed(string[] btSelectItem)
        {
            if (btSelectItem.Count() == 0)
                ModelState.AddModelError(string.Empty, "هیچ دیدگاهی برای حذف انتخاب نشده است.");
            else
            {
                var splited = new string[btSelectItem.Length];
                foreach (var item in btSelectItem)
                    splited = item.Split(',');

                foreach (var splite in splited)
                {
                    var comment = await _uw.BaseService<Comment>().FindByIdAsync(int.Parse(splite));
                    comment.RemoveTime = DateTime.Now;
                    _uw.BaseService<Comment>().Update(comment);
                }
                await _uw.BaseService<Comment>().Commit();
                return Ok(DeleteGroupSuccess);
            }
            return BadRequest(CommentNotFound);
        }
    }
}
