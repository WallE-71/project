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
using ShoppingStore.Domain.Common.Attributes;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Application.ViewModels.Order;
using ShoppingStore.Application.ViewModels.DynamicAccess;

namespace ShoppingStore.Areas.Admin.Controllers
{
    [DisplayName("مدیریت سفارشات")]
    public class OrderController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uw;
        private const string OrderNotFound = "سفارش درخواستی یافت نشد.";
        public OrderController(IMapper mapper, IUnitOfWork uw)
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
        public async Task<IActionResult> GetOrders(string search, string order, int offset, int limit, string sort)
        {
            List<OrderViewModel> viewModels;
            int total = _uw.BaseService<Order>().CountEntities();

            if (!search.HasValue())
                search = "";

            if (limit == 0)
                limit = total;

            if (sort == "productName")
            {
                if (order == "asc")
                    viewModels = await _uw.AdminOrders.GetPaginateOrdersAsync(offset, limit, "ProductName", search);
                else
                    viewModels = await _uw.AdminOrders.GetPaginateOrdersAsync(offset, limit, "ProductName desc", search);
            }
            else if (sort == "requestPayId")
            {
                if (order == "asc")
                    viewModels = await _uw.AdminOrders.GetPaginateOrdersAsync(offset, limit, "RequestPayId", search);
                else
                    viewModels = await _uw.AdminOrders.GetPaginateOrdersAsync(offset, limit, "RequestPayId desc", search);
            }
            else if (sort == "amountPaid")
            {
                if (order == "asc")
                    viewModels = await _uw.AdminOrders.GetPaginateOrdersAsync(offset, limit, "AmountPaid", search);
                else
                    viewModels = await _uw.AdminOrders.GetPaginateOrdersAsync(offset, limit, "AmountPaid desc", search);
            }
            else if (sort == "quantity")
            {
                if (order == "asc")
                    viewModels = await _uw.AdminOrders.GetPaginateOrdersAsync(offset, limit, "Quantity", search);
                else
                    viewModels = await _uw.AdminOrders.GetPaginateOrdersAsync(offset, limit, "Quantity desc", search);
            }
            else if (sort == "persianInsertTime")
            {
                if (order == "asc")
                    viewModels = await _uw.AdminOrders.GetPaginateOrdersAsync(offset, limit, "InsertTime", search);
                else
                    viewModels = await _uw.AdminOrders.GetPaginateOrdersAsync(offset, limit, "InsertTime desc", search);
            }
            else if (sort == "orderState")
            {
                if (order == "asc")
                    viewModels = await _uw.AdminOrders.GetPaginateOrdersAsync(offset, limit, "OrderState", search);
                else
                    viewModels = await _uw.AdminOrders.GetPaginateOrdersAsync(offset, limit, "OrderState desc", search);
            }
            else
                viewModels = await _uw.AdminOrders.GetPaginateOrdersAsync(offset, limit, "RequestPayId", search);

            if (search != "")
                total = viewModels.Count();

            return Json(new { total = total, rows = viewModels });
        }

        [HttpGet, AjaxOnly]
        public async Task<IActionResult> RenderOrderState(int orderId)
        {
            var viewModel = new OrderViewModel();
            if (orderId != 0)
            {
                var order = await _uw.BaseService<Order>().FindByIdAsync(orderId);
                if (order != null)
                {
                    viewModel.Id = order.Id;
                    viewModel.OrderState = order.States;
                }
                else
                    ModelState.AddModelError(string.Empty, OrderNotFound);
            }
            return PartialView("_RenderOrderState", viewModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeOrderState(OrderViewModel viewModel)
        {
            if (viewModel.Id == 0)
                ModelState.AddModelError(string.Empty, $"عضوی با شناسه '{viewModel.Id}' یافت نشد !!!");
            else
            {
                var order = await _uw.BaseService<Order>().FindByIdAsync(viewModel.Id);
                if (order != null)
                {
                    order.UpdateTime = DateTime.Now;
                    order.States = viewModel.OrderState;

                    //order.States = orderState == "0" ? OrderState.Processing : orderState == "1" ? OrderState.Confirmed :
                    //    orderState == "2" ? OrderState.Transmission : OrderState.Delivered;

                    _uw.BaseService<Order>().Update(order);
                    await _uw.BaseService<Order>().Commit();
                    //return Json("Success");
                    TempData["notification"] = OperationSuccess;
                }
            }
            return PartialView("_RenderOrderState", viewModel);
        }

        [HttpGet, AjaxOnly, DisplayName("حذف"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> Delete(int orderId)
        {
            if (orderId == 0)
                ModelState.AddModelError(string.Empty, OrderNotFound);
            else
            {
                var order = await _uw.BaseService<Order>().FindByIdAsync(orderId);
                if (order == null)
                    ModelState.AddModelError(string.Empty, OrderNotFound);
                else
                    return PartialView("_DeleteConfirmation", order);
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("Delete"), AjaxOnly]
        public async Task<IActionResult> DeleteConfirmed(Order model)
        {
            if (model.Id == 0)
                ModelState.AddModelError(string.Empty, OrderNotFound);
            else
            {
                var order = await _uw.BaseService<Order>().FindByIdAsync(model.Id);
                if (order == null)
                    ModelState.AddModelError(string.Empty, OrderNotFound);
                else
                {
                    order.RemoveTime = DateTime.Now;
                    _uw.BaseService<Order>().Update(order);
                    await _uw.BaseService<Order>().Commit();
                    TempData["notification"] = DeleteSuccess;
                    return PartialView("_DeleteConfirmation", order);
                }
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("DeleteGroup"), AjaxOnly, DisplayName("حذف گروهی"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> DeleteGroupConfirmed(string[] btSelectItem)
        {
            if (btSelectItem.Count() == 0)
                ModelState.AddModelError(string.Empty, "هیچ سفارشی برای حذف انتخاب نشده است.");
            else
            {
                var splited = new string[btSelectItem.Length];
                foreach (var item in btSelectItem)
                    splited = item.Split(',');

                foreach (var splite in splited)
                {
                    var order = await _uw.BaseService<Order>().FindByIdAsync(int.Parse(splite));
                    order.RemoveTime = DateTime.Now;
                    _uw.BaseService<Order>().Update(order);
                }
                await _uw.BaseService<Order>().Commit();
                return Ok(DeleteGroupSuccess);
            }
            return BadRequest(OrderNotFound);
        }
    }
}
