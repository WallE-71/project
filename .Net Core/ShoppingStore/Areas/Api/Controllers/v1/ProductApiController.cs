using System;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Domain.Common.Api;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Domain.Common.Api.Attributes;
using ShoppingStore.Application.ViewModels.Api.Home;

namespace ShoppingStore.Web.Areas.Api.Controllers.v1
{
    [DisplayName("ProductApi"), ApiResultFilter, ApiVersion("1"), Route("api/v{version:apiVersion}/[controller]")]
    public class ProductApiController : ControllerBase
    {
        private readonly IUnitOfWork _uw;
        private readonly IHttpContextAccessor _accessor;
        private readonly IProductService _productService;
        public ProductApiController(
            IUnitOfWork uw,
            IHttpContextAccessor accessor,
            IProductService productService)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
            _accessor = accessor;
            _accessor.CheckArgumentIsNull(nameof(_accessor));
            _productService = productService;
            _productService.CheckArgumentIsNull(nameof(_productService));
        }

        [HttpGet("GetProductInCategory")]
        public virtual async Task<ApiResult<object>> GetProductInCategory(string categoryName)
        {
            var discountType = await _productService.GetMaxDiscountTypeInCategory();
            var products = await _uw.SiteProducts.GetProductInCategoryAsync(categoryName, "Name", 0, discountType);
            return Ok(new { products = products, discountType = discountType });
        }

        [HttpGet("GetProductInCategoryByFilter")]
        public virtual async Task<ApiResult<List<ProductInCategoriesDto>>> GetProductInCategoryByFilter(string categoryName, int filterId, int filterByCheched)
        {
            var discountType = await _productService.GetMaxDiscountTypeInCategory();
            var productsInCategories = new List<ProductInCategoriesDto>();
            if (filterByCheched != 0)
                productsInCategories = await _uw.SiteProducts.GetProductInCategoryAsync(categoryName, "Name", filterByCheched, discountType);
            else
            {
                var filter = filterId == 1 ? "PercentDiscount desc" : filterId == 2 ? "Price desc" : filterId == 3 ? "Price" : "Name";
                productsInCategories = await _uw.SiteProducts.GetProductInCategoryAsync(categoryName, filter, 0, discountType);
            }
            return Ok(productsInCategories);
        }

        [HttpGet("Index")]
        public virtual async Task<IActionResult> Index()
        {
            var mostTalkProduct = await _uw.SiteProducts.MostTalkProductAsync();
            var mostViewedProduct = await _uw.SiteProducts.MostViewedProductAsync();
            var preferedProducts = await _uw.SiteProducts.GetPreferedProductsAsync();
            var mostPopularProducts = await _uw.SiteProducts.MostPopularProductsAsync();
            var mostDiscountProducts = await _uw.SiteProducts.MostDiscountProductAsync();
            var specialDiscountProducts = await _uw.SiteProducts.SpecialDiscountProductsAsync("سهشنبه");
            var preferedProductsNoDiscount = await _uw.SiteProducts.GetPreferedProductsNoDiscountAsync();
            return Ok(new { mostTalkProduct, mostViewedProduct, preferedProducts, mostPopularProducts, mostDiscountProducts, specialDiscountProducts, preferedProductsNoDiscount });
        }

        [HttpGet("ProductDetails")]
        public async Task<IActionResult> ProductDetails(string productId, string browserId, string userId)
        {
            if (!productId.HasValue())
                return Ok();
            else
            {
                var existProducts = await _uw.BaseService<Product>().FindByIdAsync(productId);
                if (existProducts == null)
                    return Ok();
                else
                {
                    Guid _BrowserId;
                    Guid.TryParse(browserId, out _BrowserId);
                    var IpAddress = _accessor.HttpContext?.Connection?.RemoteIpAddress.ToString();
                    await _uw.SiteProducts.InsertVisitOfUserAsync(productId, _BrowserId, IpAddress);

                    var seller = await _uw.SiteSellers.GetSellerForProductAsync(productId);
                    var productComments = await _uw.SiteComments.GetProductCommentsAsync(productId);
                    var preferedProductsNoDiscount = await _uw.SiteProducts.GetPreferedProductsNoDiscountAsync();
                    var product = await _uw.SiteProducts.GetProductByIdAsync(productId, userId != null ? userId : null);
                    var productsRelated = await _uw.SiteProducts.GetRelatedProductAsync(8, product.CategoryIdsList, productId);
                    return Ok(new { seller, productComments, preferedProductsNoDiscount, product, productsRelated });
                }
            }
        }

        [HttpGet("Bookmark")]
        public async Task<IActionResult> BookmarkProduct(string productId, string userId)
        {
            if (productId.HasValue() && userId != "null" && userId != null)
            {
                int id;
                Int32.TryParse(userId, out id);
                var bookmark = _uw.BaseService<Bookmark>().FindByConditionAsync(b => b.UserId == id && b.ProductId == productId).Result.FirstOrDefault();
                if (bookmark == null)
                {
                    bookmark = new Bookmark { ProductId = productId, UserId = id, InsertTime = DateTime.Now };
                    await _uw.BaseService<Bookmark>().CreateAsync(bookmark);
                    await _uw.BaseService<Bookmark>().Commit();
                    return Ok(true);
                }
                else
                {
                    var result = false;
                    if (bookmark.RemoveTime == null)
                        bookmark.RemoveTime = DateTime.Now;
                    else
                    {
                        bookmark.RemoveTime = null;
                        bookmark.InsertTime = DateTime.Now;
                        result = true;
                    }
                    _uw.BaseService<Bookmark>().Update(bookmark);
                    await _uw.BaseService<Bookmark>().Commit();
                    return Ok(result);
                }
            }
            else
                return Ok();
        }


        [HttpGet("Search")]
        public ApiResult<string> Search(string searchText)
        {
            try
            {
                return Ok(_uw.BaseService<Product>().FindByConditionAsync(p => (p.Name.Contains(searchText) == true || p.Description.Contains(searchText))
                                                                     && p.States == ProductState.Ready).Result.FirstOrDefault().Id);
            }
            catch
            {
                return NotFound("محصول درخواستی یافت نشد!");
            }
        }
    }
}
