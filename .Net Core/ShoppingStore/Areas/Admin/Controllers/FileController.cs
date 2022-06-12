using System;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using ImageMagick;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Common.Attributes;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Application.ViewModels.File;
using ShoppingStore.Application.ViewModels.Product;
using ShoppingStore.Application.ViewModels.DynamicAccess;

namespace ShoppingStore.Areas.Admin.Controllers
{
    [DisplayName("مدیریت فایل ها")]
    public class FileController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uw;
        private readonly IWebHostEnvironment _env;
        private const string FileNotFound = "فایل درخواستی یافت نشد.";
        public FileController(IUnitOfWork uw, IMapper mapper, IWebHostEnvironment env)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
            _env = env;
            _env.CheckArgumentIsNull(nameof(_env));
            _mapper = mapper;
            _mapper.CheckArgumentIsNull(nameof(_mapper));
        }

        [HttpGet, DisplayName("مشاهده"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetFiles(string search, string order, int offset, int limit, string sort)
        {
            List<FileViewModel> viewModels;
            int total = _uw.BaseService<FileStore>().CountEntities();

            if (!search.HasValue())
                search = "";

            if (limit == 0)
                limit = total;

            if (sort == "تصویر")
            {
                if (order == "asc")
                    viewModels = await _uw.AdminFiles.GetPaginateFilesAsync(offset, limit, "ImageName", search);
                else
                    viewModels = await _uw.AdminFiles.GetPaginateFilesAsync(offset, limit, "ImageName desc", search);
            }
            else if (sort == "فایل/سند")
            {
                if (order == "asc")
                    viewModels = await _uw.AdminFiles.GetPaginateFilesAsync(offset, limit, "FileName", search);
                else
                    viewModels = await _uw.AdminFiles.GetPaginateFilesAsync(offset, limit, "FileName desc", search);
            }
            else
                viewModels = await _uw.AdminFiles.GetPaginateFilesAsync(offset, limit, "ImageName", search);

            if (search != "")
                total = viewModels.Count();

            return Json(new { total = total, rows = viewModels });
        }

        [HttpGet]
        public IActionResult Upload(string productId)
        {
            return PartialView("UploadMultiImage", new ProductViewModel(productId));
        }

        [HttpPost, Route("Upload")]
        public async Task<IActionResult> UploadPost(IEnumerable<IFormFile> files, string productId)
        {
            try
            {
                //var findDuplicateImage = _uw.BaseRepository<FileStore>().FindByConditionAsync(f => f.ProductId == productId).Result.FirstOrDefault();
                //if(findDuplicateImage != null) return PartialView("UploadMultiImage", new ProductViewModel(productId));
                var uploadsRootFolder = Path.Combine(_env.WebRootPath, "assets/images/gallery");
                var uploadInAngularFolder = Path.Combine(_env.WebRootPath + "\\..\\..\\..\\Angular\\src\\assets", "gallery");

                var nameImages = new List<string>();
                foreach (var item in files)
                {
                    if (item != null)
                    {
                        var fileExtension = Path.GetExtension(item.FileName);
                        var imageName = String.Concat($"product-{productId}-{StringExtensions.GenerateId(10)}", fileExtension);
                        var path = Path.Combine(uploadsRootFolder, imageName);
                        var pathAngular = Path.Combine(uploadInAngularFolder, imageName);
                        using (var memory = new MemoryStream())
                        {
                            await item.CopyToAsync(memory);
                            using (var Image = new MagickImage(memory.ToArray()))
                            {
                                Image.Resize(Image.Width / 2, Image.Height / 2);
                                Image.Quality = 50;
                                Image.Write(path);
                                Image.Write(pathAngular);
                            }
                        }
                        nameImages.Add(imageName);
                        //CompressImage(path);
                    }
                }
                if (nameImages.Count != 0)
                    await _uw.AdminFiles.InsertImagesAsync(productId, null, null, null, nameImages);
                return PartialView("UploadMultiImage", new ProductViewModel(new JsonResult("success"), productId));
            }
            catch (Exception e)
            {
                return PartialView("UploadMultiImage", new ProductViewModel(productId));
            }
        }

        private void CompressImage(string path)
        {
            var Image = new FileInfo(path);
            var optimizer = new ImageOptimizer();
            optimizer.Compress(Image); //error
            Image.Refresh();
        }

        [HttpGet, AjaxOnly, DisplayName("حذف"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> Delete(string fileId)
        {
            if (!fileId.HasValue())
                ModelState.AddModelError(string.Empty, FileNotFound);
            else
            {
                var fileStore = await _uw.BaseService<FileStore>().FindByIdAsync(fileId);
                if (fileStore == null)
                    ModelState.AddModelError(string.Empty, FileNotFound);
                else
                    return PartialView("_DeleteConfirmation", fileStore);
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("Delete"), AjaxOnly]
        public async Task<IActionResult> DeleteConfirmed(FileStore model)
        {
            if (!model.ImageName.HasValue())
                ModelState.AddModelError(string.Empty, FileNotFound);
            else
            {
                var fileStore = await _uw.BaseService<FileStore>().FindByIdAsync(model.ImageName);
                if (fileStore == null)
                    ModelState.AddModelError(string.Empty, FileNotFound);
                else
                {
                    fileStore.RemoveTime = DateTime.Now;
                    _uw.BaseService<FileStore>().Update(fileStore);
                    await _uw.BaseService<FileStore>().Commit();
                    TempData["notification"] = DeleteSuccess;
                    return PartialView("_DeleteConfirmation", fileStore);
                }
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("DeleteGroup"), AjaxOnly, DisplayName("حذف گروهی"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> DeleteGroupConfirmed(string[] btSelectItem)
        {
            if (btSelectItem.Count() == 0)
                ModelState.AddModelError(string.Empty, "هیچ فایلی برای حذف انتخاب نشده است.");
            else
            {
                var splited = new string[btSelectItem.Length];
                foreach (var item in btSelectItem)
                    splited = item.Split(',');

                foreach (var splite in splited)
                {
                    var file = await _uw.BaseService<FileStore>().FindByIdAsync(int.Parse(splite));
                    file.RemoveTime = DateTime.Now;
                    _uw.BaseService<FileStore>().Update(file);
                }
                await _uw.BaseService<FileStore>().Commit();
                return Ok(DeleteGroupSuccess);
            }
            return BadRequest(FileNotFound);
        }
    }
}
