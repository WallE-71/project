using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Domain.Common.Api;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Domain.Common.Api.Attributes;
using ShoppingStore.Application.ViewModels.Api.Seller;

namespace ShoppingStore.Web.Areas.Api.Controllers.v1
{
    [DisplayName("SellerApi"), ApiResultFilter, ApiController, ApiVersion("1"), Route("api/v{version:apiVersion}/[controller]")]
    public class SellerApiController : ControllerBase
    {
        private readonly IUnitOfWork _uw;
        private readonly IWebHostEnvironment _env;
        public SellerApiController(IUnitOfWork uw, IWebHostEnvironment env)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
            _env = env;
            _env.CheckArgumentIsNull(nameof(_env));
        }

        [HttpPost]
        public virtual async Task<ApiResult<string>> RequestRegister(string imageFile, string name, string surName, string nationalId,
            string phoneNumber, string email, string webSite, string brand, int activityType, string store, string telNumber, string address,
            string postalCode, string establishmentDate, string sampleProduct, string description, string scanNationalIdCart, string scanDocument)
        {
            if (name.HasValue() && surName.HasValue() && phoneNumber.HasValue() && activityType != 0)
            {
                var dto = new SellerDto();
                var sellerId = await _uw.InsertRequestSeller.RequestRegister(dto);
                if (imageFile.HasValue())
                    await InsertImage(imageFile, sellerId, null);
                if (scanNationalIdCart.HasValue())
                    await InsertImage(scanNationalIdCart, sellerId, null);
                if (scanDocument.HasValue())
                    await InsertImage(scanDocument, sellerId, null);
                return Ok(email);
            }
            return Ok();
        }

        [HttpGet]
        public async Task<ApiResult<string>> ReciveAnswerAsync(string email)
        {
            if (email.HasValue())
            {
                var sellers = await _uw.BaseService<Seller>().FindByConditionAsync(s => s.Email == email);
                if (sellers.Count() == 0)
                    return Ok();
                else
                {
                    if (sellers.FirstOrDefault().IsComplete)
                        return Ok("success");
                    else
                        return Ok();
                }
            }
            return Ok();
        }

        [NonAction]
        public async Task InsertImage(string file, int sellerId, string imageName)
        {
            var checkRootFolder = Path.Combine(_env.WebRootPath, "images/seller");
            if (!Directory.Exists(checkRootFolder))
                Directory.CreateDirectory(checkRootFolder);
            var checkAngularFolder = Path.Combine(_env.WebRootPath + "/../../Angular/src/assets/images/", "sellerImages");
            if (!Directory.Exists(checkAngularFolder))
                Directory.CreateDirectory(checkAngularFolder);

            if (imageName.HasValue())
            {
                FileExtensions.DeleteFile($"{_env.WebRootPath}/images/seller/{imageName}");
                FileExtensions.DeleteFile($"{_env.WebRootPath}/../../Angular/src/assets/images/sellerImages/{imageName}"); ;
            }
            var image = $"seller-{StringExtensions.GenerateId(10)}.jpg";
            file.UploadFileBase64($"{_env.WebRootPath}/images/seller/{image}");
            file.UploadFileBase64($"{_env.WebRootPath}/../../Angular/src/assets/images/sellerImages/{image}");

            var images = new List<string>();
            images.Add(image);
            await _uw.AdminFiles.InsertImagesAsync(null, sellerId, null, null, images);
        }
    }
}
