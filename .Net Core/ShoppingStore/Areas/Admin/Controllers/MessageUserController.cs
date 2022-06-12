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
using ShoppingStore.Application.ViewModels.MessageUser;
using ShoppingStore.Application.ViewModels.DynamicAccess;

namespace ShoppingStore.Areas.Admin.Controllers
{
    [DisplayName("مدیریت پیام ها")]
    public class MessageUserController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uw;
        private const string MessageNotFound = "پیام یافت نشد.";
        public MessageUserController(IMapper mapper, IUnitOfWork uw)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
            _mapper = mapper;
            _mapper.CheckArgumentIsNull(nameof(_mapper));
        }

        [HttpGet, DisplayName("مشاهده"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public IActionResult Index(int userId)
        {
            return View(nameof(Index), new MessageUsersViewModel { UserId = userId });
        }

        [HttpGet]
        public async Task<IActionResult> GetMessages(string search, string order, int offset, int limit, string sort, int userId, bool? isRegisterCode)
        {
            List<MessageUsersViewModel> viewModels;
            int total = _uw.BaseService<MessageUser>().CountEntities();

            if (!search.HasValue())
                search = "";

            if (limit == 0)
                limit = total;

            if (userId == 0)
                userId = 0;

            if (sort == "email")
            {
                if (order == "asc")
                    viewModels = await _uw.AdminMessageUsers.GetPaginateMessagesAsync(offset, limit, "Email", search, 0, null);
                else
                    viewModels = await _uw.AdminMessageUsers.GetPaginateMessagesAsync(offset, limit, "Email desc", search, 0, null);
            }
            else if (sort == "message")
            {
                if (order == "asc")
                    viewModels = await _uw.AdminMessageUsers.GetPaginateMessagesAsync(offset, limit, "MessageTypes", search, 0, null);
                else
                    viewModels = await _uw.AdminMessageUsers.GetPaginateMessagesAsync(offset, limit, "MessageTypes desc", search, 0, null);
            }
            else if (sort == "persianInsertTime")
            {
                if (order == "asc")
                    viewModels = await _uw.AdminMessageUsers.GetPaginateMessagesAsync(offset, limit, "InsertTime", search, 0, null);
                else
                    viewModels = await _uw.AdminMessageUsers.GetPaginateMessagesAsync(offset, limit, "InsertTime desc", search, 0, null);
            }
            else if (sort == "answer")
            {
                if (order == "asc")
                    viewModels = await _uw.AdminMessageUsers.GetPaginateMessagesAsync(offset, limit, "Answer", search, 0, null);
                else
                    viewModels = await _uw.AdminMessageUsers.GetPaginateMessagesAsync(offset, limit, "Answer desc", search, 0, null);
            }
            else if (sort == "isComplete")
            {
                if (order == "asc")
                    viewModels = await _uw.AdminMessageUsers.GetPaginateMessagesAsync(offset, limit, "IsComplete", search, 0, null);
                else
                    viewModels = await _uw.AdminMessageUsers.GetPaginateMessagesAsync(offset, limit, "IsComplete desc", search, 0, null);
            }
            else
                viewModels = await _uw.AdminMessageUsers.GetPaginateMessagesAsync(offset, limit, "InsertTime desc", search, 0, null);

            if (search != "")
                total = viewModels.Count();

            return Json(new { total = total, rows = viewModels });
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmOrInConfirmMessage(string messageId)
        {
            if (messageId.HasValue())
            {
                var message = await _uw.BaseService<MessageUser>().FindByIdAsync(messageId);
                if (message != null)
                {
                    if (message.IsComplete)
                        message.IsComplete = false;
                    else
                        message.IsComplete = true;

                    _uw.BaseService<MessageUser>().Update(message);
                    await _uw.BaseService<MessageUser>().Commit();
                    return Json("Success");
                }
            }
            return Json($"عضوی با شناسه '{messageId}' یافت نشد !!!");
        }

        [HttpGet, DisplayName("پاسخ به پیام دریافتی"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> AnswerMessage(int messageId)
        {
            if (messageId == 0)
                ModelState.AddModelError(string.Empty, MessageNotFound);
            else
            {
                var message = await _uw.BaseService<MessageUser>().FindByIdAsync(messageId);
                if (message == null)
                    ModelState.AddModelError(string.Empty, MessageNotFound);
                else
                    return PartialView("_AnswerMessage", _mapper.Map<MessageUsersViewModel>(message));
            }
            return PartialView("_AnswerMessage");
        }

        [HttpPost]
        public async Task<IActionResult> AnswerMessage(MessageUsersViewModel viewModel)
        {
            if (viewModel.Id != 0)
            {
                var message = await _uw.BaseService<MessageUser>().FindByIdAsync(viewModel.Id);
                if (message != null)
                {
                    message.Answer = viewModel.Answer;
                    message.UpdateTime = DateTime.Now;
                    _uw.BaseService<MessageUser>().Update(message);
                    await _uw.BaseService<MessageUser>().Commit();
                    TempData["notification"] = EditSuccess;
                }
                else
                    ModelState.AddModelError(string.Empty, MessageNotFound);
            }
            return PartialView("_AnswerMessage", viewModel);
        }

        [HttpGet, DisplayName("حذف"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> Delete(string messageId)
        {
            if (!messageId.HasValue())
                ModelState.AddModelError(string.Empty, MessageNotFound);
            else
            {
                var message = await _uw.BaseService<MessageUser>().FindByIdAsync(messageId);
                if (message == null)
                    ModelState.AddModelError(string.Empty, MessageNotFound);
                else
                    return PartialView("_DeleteConfirmation", message);
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(MessageUser model)
        {
            if (model.Id == 0)
                ModelState.AddModelError(string.Empty, MessageNotFound);
            else
            {
                var message = await _uw.BaseService<MessageUser>().FindByIdAsync(model.Id);
                if (message == null)
                    ModelState.AddModelError(string.Empty, MessageNotFound);
                else
                {
                    message.RemoveTime = DateTime.Now;
                    _uw.BaseService<MessageUser>().Update(message);
                    await _uw.BaseService<MessageUser>().Commit();
                    TempData["notification"] = DeleteSuccess;
                    return PartialView("_DeleteConfirmation", message);
                }
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("DeleteGroup"), DisplayName("حذف گروهی"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> DeleteGroupConfirmed(string[] btSelectItem)
        {
            if (btSelectItem.Count() == 0)
                ModelState.AddModelError(string.Empty, "هیچ پیامی برای حذف انتخاب نشده است.");
            else
            {
                var splited = new string[btSelectItem.Length];
                foreach (var item in btSelectItem)
                    splited = item.Split(',');

                foreach (var splite in splited)
                {
                    var message = await _uw.BaseService<MessageUser>().FindByIdAsync(int.Parse(splite));
                    message.RemoveTime = DateTime.Now;
                    _uw.BaseService<MessageUser>().Update(message);
                }
                await _uw.BaseService<MessageUser>().Commit();
                return Ok(DeleteGroupSuccess);
            }
            return BadRequest(MessageNotFound);
        }
    }
}
