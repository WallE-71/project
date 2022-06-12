using System;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Application.ViewModels.Seller;
using ShoppingStore.Application.ViewModels.Product;
using ShoppingStore.Application.ViewModels.DynamicAccess;

namespace ShoppingStore.Areas.Admin.Controllers
{
    [DisplayName("مدیریت فروشندگان")]
    public class SellerController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uw;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _context;
        private const string ProductNotFound = "محصول یافت نشد.";
        private const string SellerNotFound = "فروشنده یافت نشد.";
        public SellerController(
            IMapper mapper,
            IUnitOfWork uw,
            IWebHostEnvironment env,
            IHttpContextAccessor context)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
            _env = env;
            _env.CheckArgumentIsNull(nameof(_env));
            _mapper = mapper;
            _mapper.CheckArgumentIsNull(nameof(_mapper));
            _context = context;
            _context.CheckArgumentIsNull(nameof(_context));
        }

        [HttpGet, DisplayName("مشاهده"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetSellers(string search, string order, int offset, int limit, string sort)
        {
            List<SellerViewModel> viewModels;
            int total = _uw.BaseService<Seller>().CountEntities();

            if (!search.HasValue())
                search = "";

            if (limit == 0)
                limit = total;

            if (sort == "brand")
            {
                if (order == "asc")
                    viewModels = await _uw.AdminSellers.GetPaginateSellersAsync(offset, limit, "Brand", search);
                else
                    viewModels = await _uw.AdminSellers.GetPaginateSellersAsync(offset, limit, "Brand desc", search);
            }
            else if (sort == "userVote")
            {
                if (order == "asc")
                    viewModels = await _uw.AdminSellers.GetPaginateSellersAsync(offset, limit, "UserVote", search);
                else
                    viewModels = await _uw.AdminSellers.GetPaginateSellersAsync(offset, limit, "UserVote desc", search);
            }
            else if (sort == "persianRegisterDate")
            {
                if (order == "asc")
                    viewModels = await _uw.AdminSellers.GetPaginateSellersAsync(offset, limit, "RegisterDate", search);
                else
                    viewModels = await _uw.AdminSellers.GetPaginateSellersAsync(offset, limit, "RegisterDate desc", search);
            }
            else
                viewModels = await _uw.AdminSellers.GetPaginateSellersAsync(offset, limit, "RegisterDate desc", search);

            if (search != "")
                total = viewModels.Count();

            return Json(new { total = total, rows = viewModels });
        }

        [HttpGet, DisplayName("درج و ویرایش"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> CreateOrUpdate(int sellerId)
        {
            var viewModel = new SellerViewModel();
            if (sellerId == 0)
                ModelState.AddModelError(string.Empty, SellerNotFound);
            else
            {
                var seller = await _uw.BaseService<Seller>().FindByIdAsync(sellerId);
                if (seller != null)
                {
                    viewModel = _mapper.Map<SellerViewModel>(seller);
                    var store = _uw.BaseService<Store>().FindByConditionAsync(s => s.SellerId == seller.Id).Result.FirstOrDefault();
                    if (store != null)
                    {
                        viewModel.Store = store.Name;
                        viewModel.Address = store.Address;
                        viewModel.TelNumber = store.TelNumber;
                        viewModel.PostalCode = store.PostalCode;
                        viewModel.SampleProduct = store.Description;
                        viewModel.PersianEstablishmentDate = store.EstablishmentDate.ConvertMiladiToShamsi("yyyy/MM/dd");
                    }
                    if (seller.InsertTime > DateTime.Now)
                    {
                        viewModel.PersianInsertTime = seller.InsertTime.ConvertMiladiToShamsi("yyyy/MM/dd");
                        viewModel.PersianInsertTime = seller.InsertTime.Value.TimeOfDay.ToString();
                    }

                    if (seller.InsertTime > DateTime.Now)
                        viewModel.PersianInsertTime = seller.InsertTime.ConvertMiladiToShamsi("yyyy/MM/dd");
                }
                else
                    ModelState.AddModelError(string.Empty, SellerNotFound);
            }
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate(SellerViewModel viewModel)
        {
            if (viewModel.Id != 0)
            {
                ModelState.Remove("SellerImageFile");
                ModelState.Remove("NationalIdImageFile");
                ModelState.Remove("DocumentImageFile");
            }
            if (ModelState.IsValid)
            {
                var IsSuccess = false;
                if (viewModel.Id != 0)
                {
                    var seller = await _uw.BaseService<Seller>().FindByIdAsync(viewModel.Id);
                    if (seller == null)
                        ModelState.AddModelError(string.Empty, SellerNotFound);
                    else
                    {
                        viewModel.UpdateTime = DateTime.Now;
                        _uw.BaseService<Seller>().Update(_mapper.Map(viewModel, seller));
                        await _uw.BaseService<Seller>().Commit();

                        var store = await _uw.BaseService<Store>().FindByConditionAsync(s => s.Name == viewModel.Store && s.PostalCode == viewModel.PostalCode);
                        if (store.Count() != 0)
                        {
                            store.FirstOrDefault().Name = viewModel.Store;
                            store.FirstOrDefault().Address = viewModel.Address;
                            store.FirstOrDefault().TelNumber = viewModel.TelNumber;
                            store.FirstOrDefault().PostalCode = viewModel.PostalCode;
                            store.FirstOrDefault().Description = viewModel.SampleProduct;
                            store.FirstOrDefault().EstablishmentDate = viewModel.PersianEstablishmentDate.ConvertShamsiToMiladi();
                            store.FirstOrDefault().UpdateTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now);
                            _uw.BaseService<Store>().Update(store.FirstOrDefault());
                        }
                        else if (viewModel.Store.HasValue())
                        {
                            var newStore = new Store
                            {
                                Id = viewModel.Id,
                                Name = viewModel.Store,
                                SellerId = viewModel.Id,
                                Address = viewModel.Address,
                                TelNumber = viewModel.TelNumber,
                                PostalCode = viewModel.PostalCode,
                                Description = viewModel.SampleProduct,
                                EstablishmentDate = viewModel.PersianEstablishmentDate.ConvertShamsiToMiladi(),
                                InsertTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now)
                            };
                            await _uw.BaseService<Store>().CreateAsync(newStore);
                        }
                        await _uw.BaseService<Store>().Commit();
                        ViewBag.Alert = EditSuccess;
                        IsSuccess = true;
                    }
                }
                else
                {
                    var seller = new Seller();
                    var sellers = await _uw.BaseService<Seller>().FindByConditionAsync(null, p => p.OrderByDescending(p => p.Id));
                    if (sellers.Count() != 0)
                        viewModel.Id = sellers.FirstOrDefault().Id + 1;
                    else
                        viewModel.Id = 1;
                    viewModel.InsertTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now);
                    await _uw.BaseService<Seller>().CreateAsync(_mapper.Map<Seller>(viewModel));
                    await _uw.BaseService<Seller>().Commit();

                    if (viewModel.Store.HasValue())
                    {
                        var newStore = new Store
                        {
                            Id = viewModel.Id,
                            Name = viewModel.Store,
                            SellerId = viewModel.Id,
                            Address = viewModel.Address,
                            TelNumber = viewModel.TelNumber,
                            PostalCode = viewModel.PostalCode,
                            Description = viewModel.SampleProduct,
                            EstablishmentDate = viewModel.PersianEstablishmentDate.ConvertShamsiToMiladi(),
                            InsertTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now)
                        };
                        await _uw.BaseService<Store>().CreateAsync(newStore);
                        await _uw.BaseService<Store>().Commit();
                    }
                    IsSuccess = true;
                }
                var lastImages = await _uw.AdminFiles.GetImagesAsync(null, viewModel.Id, null);
                if (IsSuccess && viewModel.SellerImageFile != null)
                    await ImageManager(null, viewModel.SellerImageFile, viewModel.Id, lastImages);
                if (IsSuccess && viewModel.NationalIdImageFile != null)
                    await ImageManager(null, viewModel.NationalIdImageFile, viewModel.Id, lastImages);
                if (IsSuccess && viewModel.DocumentImageFile.HasValue())
                    await ImageManager(viewModel.DocumentImageFile, null, viewModel.Id, lastImages);

                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        [HttpPost, ActionName("AddGroup"), DisplayName("افزودن محصولات"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> AddProducts(int sellerId, string productId, string[] sellerProductIds)
        {
            var product = new Product();
            var products = new List<Product>();
            var seller = await _uw.BaseService<Seller>().FindByIdAsync(sellerId);
            if (seller == null)
                return Ok(SellerNotFound);

            var splited = new string[sellerProductIds.Length];
            foreach (var item in sellerProductIds)
                splited = item.Split(',');

            if (sellerProductIds[0] != "undefined")
            {
                foreach (var item in splited)
                {
                    product = await _uw.BaseService<Product>().FindByIdAsync(item);
                    product.SellerId = sellerId;
                    products.Add(product);
                    _uw.BaseService<Product>().UpdateRange(products);
                }
            }
            else if (productId.HasValue())
            {
                product = await _uw.BaseService<Product>().FindByIdAsync(productId);
                product.SellerId = sellerId;
                _uw.BaseService<Product>().Update(product);
            }
            await _uw.BaseService<Product>().Commit();
            ViewBag.Success = "به فروشگاه اضافه گردید!";
            return Ok("Success");
        }

        [HttpGet, DisplayName("حذف"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> Delete(int sellerId)
        {
            if (sellerId == 0)
                ModelState.AddModelError(string.Empty, SellerNotFound);
            else
            {
                var seller = await _uw.BaseService<Seller>().FindByIdAsync(sellerId);
                if (seller == null)
                    ModelState.AddModelError(string.Empty, SellerNotFound);
                else
                    return PartialView("_DeleteConfirmation", seller);
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Seller model)
        {
            if (model.Id == 0)
                ModelState.AddModelError(string.Empty, SellerNotFound);
            else
            {
                var seller = await _uw.BaseService<Seller>().FindByIdAsync(model.Id);
                if (seller == null)
                    ModelState.AddModelError(string.Empty, SellerNotFound);
                else
                {
                    seller.RemoveTime = DateTime.Now;
                    _uw.BaseService<Seller>().Update(seller);
                    await _uw.BaseService<Seller>().Commit();
                    TempData["notification"] = DeleteSuccess;
                    return PartialView("_DeleteConfirmation", seller);
                }
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("DeleteGroup"), DisplayName("حذف گروهی"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> DeleteGroupConfirmed(string[] btSelectItem)
        {
            if (btSelectItem.Count() == 0)
                ModelState.AddModelError(string.Empty, "هیچ فروشنده ای برای حذف انتخاب نشده است.");
            else
            {
                var splited = new string[btSelectItem.Length];
                foreach (var item in btSelectItem)
                    splited = item.Split(',');

                foreach (var splite in splited)
                {
                    var role = await _uw.BaseService<Seller>().FindByIdAsync(splite.ToString());
                    role.RemoveTime = DateTime.Now;
                    _uw.BaseService<Seller>().Update(role);
                    await _uw.BaseService<Seller>().Commit();
                }
                return Ok(DeleteGroupSuccess);
            }
            return BadRequest(SellerNotFound);
        }

        [HttpGet]
        public async Task<IActionResult> IsRegisterOrNotRegister(int sellerId)
        {
            if (sellerId != 0)
            {
                var seller = await _uw.BaseService<Seller>().FindByIdAsync(sellerId);
                if (seller != null)
                {
                    if (seller.IsComplete)
                    {
                        seller.IsComplete = false;
                        seller.RegisterDate = null;
                    }
                    else
                    {
                        seller.IsComplete = true;
                        seller.RegisterDate = DateTime.Now;
                    }
                    seller.UpdateTime = DateTime.Now;
                    _uw.BaseService<Seller>().Update(seller);
                    await _uw.BaseService<Seller>().Commit();
                    return Json("Success");
                }
            }
            return Json($"فروشنده ای با شناسه '{sellerId}' یافت نشد !!!");
        }

        public async Task ImageManager(string file, IFormFile imgFile, int sellerId, List<string> lastImages)
        {
            if (lastImages != null)
            {
                foreach (var img in lastImages)
                {
                    FileExtensions.DeleteFile($"{_env.WebRootPath}/assets/images/seller/{img}");
                    FileExtensions.DeleteFile($"{_env.WebRootPath}/../../../Angular/src/assets/images/sellerImages/{img}");
                }
                var fileStores = await _uw.BaseService<FileStore>().FindByConditionAsync(f => f.SellerId == sellerId);
                if (fileStores.Count() != 0)
                    _uw.BaseService<FileStore>().DeleteRange(fileStores);
            }

            var image = $"seller-{StringExtensions.GenerateId(10)}.jpg";
            if (imgFile != null)
            {
                await imgFile.UploadFileAsync($"{_env.WebRootPath}/assets/images/seller/{image}");
                await imgFile.UploadFileAsync($"{_env.WebRootPath}/../../../Angular/src/assets/images/sellerImages/{image}");
            }
            else if (file.HasValue())
            {
                file.UploadFileBase64($"{_env.WebRootPath}/assets/images/seller/{image}");
                file.UploadFileBase64($"{_env.WebRootPath}/../../../Angular/src/assets/images/sellerImages/{image}");
            }
            var images = new List<string>();
            images.Add(image);
            await _uw.AdminFiles.InsertImagesAsync(null, sellerId, null, null, images);
        }

        [HttpGet, DisplayName("مشاهده محصولات"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public IActionResult ProductsOfSeller(int sellerId, bool? isSeller)
        {
            return View(nameof(ProductsOfSeller), new ProductViewModel { SellerId = sellerId, IsSeller = isSeller });
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsForSeller(string search, string order, int offset, int limit, string sort, bool? isSeller, long sellerId)
        {
            List<ProductViewModel> viewModels;
            int total = _uw.BaseService<Product>().CountEntities();

            if (!search.HasValue())
                search = "";

            if (limit == 0)
                limit = total;

            if (sort == "name")
            {
                if (order == "asc")
                    viewModels = await _uw.AdminProducts.GetPaginateProductAsync(offset, limit, "Name", search, null, isSeller, sellerId);
                else
                    viewModels = await _uw.AdminProducts.GetPaginateProductAsync(offset, limit, "Name desc", search, null, isSeller, sellerId);
            }
            else if (sort == "price")
            {
                if (order == "asc")
                    viewModels = await _uw.AdminProducts.GetPaginateProductAsync(offset, limit, "Price", search, null, isSeller, sellerId);
                else
                    viewModels = await _uw.AdminProducts.GetPaginateProductAsync(offset, limit, "Price desc", search, null, isSeller, sellerId);
            }
            else if (sort == "stock")
            {
                if (order == "asc")
                    viewModels = await _uw.AdminProducts.GetPaginateProductAsync(offset, limit, "Stock", search, null, isSeller, sellerId);
                else
                    viewModels = await _uw.AdminProducts.GetPaginateProductAsync(offset, limit, "Stock desc", search, null, isSeller, sellerId);
            }
            else if (sort == "persianInsertTime")
            {
                if (order == "asc")
                    viewModels = await _uw.AdminProducts.GetPaginateProductAsync(offset, limit, "InsertTime", search, null, isSeller, sellerId);
                else
                    viewModels = await _uw.AdminProducts.GetPaginateProductAsync(offset, limit, "InsertTime desc", search, null, isSeller, sellerId);
            }
            else
                viewModels = await _uw.AdminProducts.GetPaginateProductAsync(offset, limit, "InsertTime desc", search, null, isSeller, sellerId);

            if (search != "")
                total = viewModels.Count();

            return Json(new { total = total, rows = viewModels });
        }

        [HttpGet, DisplayName("حذف")]
        public async Task<IActionResult> DeleteOfStore(int sellerId, string productId)
        {
            if (sellerId == 0)
                ModelState.AddModelError(string.Empty, SellerNotFound);
            else
            {
                var product = await _uw.BaseService<Product>().FindByIdAsync(productId);
                if (product == null)
                    ModelState.AddModelError(string.Empty, ProductNotFound);
                else
                    return PartialView("_DeleteConfirmation", product);
            }
            return PartialView("_DeleteOfStoreConfirmation");
        }

        [HttpPost, ActionName("DeleteOfStore")]
        public async Task<IActionResult> DeleteOfStoreConfirmed(int sellerId, string productId)
        {
            if (sellerId == 0)
                ModelState.AddModelError(string.Empty, ProductNotFound);
            else
            {
                var product = await _uw.BaseService<Product>().FindByIdAsync(productId);
                if (product == null)
                    ModelState.AddModelError(string.Empty, ProductNotFound);
                else
                {
                    product.SellerId = null;
                    product.UpdateTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now);
                    _uw.BaseService<Product>().Update(product);
                    await _uw.BaseService<Product>().Commit();
                    TempData["notification"] = DeleteSuccess;
                    return PartialView("_DeleteOfStoreConfirmation", product);
                }
            }
            return PartialView("_DeleteOfStoreConfirmation");
        }

        [HttpPost, ActionName("DeleteGroupProduct")]
        public async Task<IActionResult> DeleteGroupProduct(int sellerId, string[] btSelectItem)
        {
            if (btSelectItem.Count() == 0)
                ModelState.AddModelError(string.Empty, "هیچ محصولی ای برای حذف انتخاب نشده است.");
            else
            {
                var ids = new string[btSelectItem.Length];
                foreach (var item in btSelectItem)
                    ids = item.Split(',');

                var seller = await _uw.BaseService<Seller>().FindByConditionAsync(s => s.Id == sellerId);
                if (seller != null)
                {
                    foreach (var id in ids)
                    {
                        var product = await _uw.BaseService<Product>().FindByIdAsync(id);
                        product.SellerId = null;
                        _uw.BaseService<Product>().Update(product);
                        await _uw.BaseService<Product>().Commit();
                    }
                }
                return Ok(DeleteGroupSuccess);
            }
            return BadRequest(ProductNotFound);
        }
    }
}
