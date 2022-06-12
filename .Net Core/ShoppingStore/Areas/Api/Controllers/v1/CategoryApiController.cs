using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ShoppingStore.Domain.Common.Api;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Domain.Common.Api.Attributes;
using ShoppingStore.Application.ViewModels.Category;

namespace ShoppingStore.Web.Areas.Api.Controllers.v1
{
    [DisplayName("CategoryApi"), ApiResultFilter, ApiVersion("1"), Route("api/v{version:apiVersion}/[controller]")]
    public class CategoryApiController : ControllerBase
    {
        private readonly IUnitOfWork _uw;
        public CategoryApiController(IUnitOfWork uw)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
        }

        [HttpGet]
        public virtual async Task<ApiResult<List<TreeViewCategory>>> ShowCategories()
        {
            return Ok(await _uw.AdminCategories.GetAllCategoriesAsync());
        }

        [HttpGet("SubCategories")]
        public virtual async Task<ApiResult<List<TreeViewCategory>>> Get(string parentName)
        {
            return Ok(await _uw.SiteCategories.GetSubCategoriesByName(parentName));
        }
    }
}
