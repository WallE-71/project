using System;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using AutoMapper;
using ImageMagick;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Domain.Common.Attributes;
using ShoppingStore.Application.ViewModels.Slider;
using ShoppingStore.Application.ViewModels.DynamicAccess;
using StringExtensions = ShoppingStore.Domain.Common.Extensions.StringExtensions;
using DateTimeExtensions = ShoppingStore.Domain.Common.Extensions.DateTimeExtensions;

namespace ShoppingStore.Areas.Admin.Controllers
{
    [DisplayName("مدیریت بنر و اسلایدرها")]
    public class SliderController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uw;
        private readonly IWebHostEnvironment _env;
        private const string SliderNotFound = "اسلایدر درخواستی یافت نشد.";
        public SliderController(IMapper mapper, IUnitOfWork uw, IWebHostEnvironment env)
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
        public async Task<IActionResult> GetSliders(string search, string order, int offset, int limit, string sort)
        {
            var viewModels = new List<SliderViewModel>();
            int total = _uw.BaseService<Slider>().CountEntities();
            if (!search.HasValue())
                search = "";

            if (limit == 0)
                limit = total;

            if (sort == "imageLocation")
            {
                if (order == "asc")
                    viewModels = await _uw.AdminSliders.GetPaginateSlidersAsync(offset, limit, "ImageLocation", search);
                else
                    viewModels = await _uw.AdminSliders.GetPaginateSlidersAsync(offset, limit, "ImageLocation desc", search);
            }
            else
                viewModels = await _uw.AdminSliders.GetPaginateSlidersAsync(offset, limit, "Id", search);

            if (search != "")
                total = viewModels.Count();

            foreach (var item in viewModels)
            {
                if (item.TypeOfSlider == TypeOfSlider.Static)
                    item.ImageFile = await _uw.AdminFiles.FindImageAsync(null, null, item.Id, null);
                else if (item.TypeOfSlider == TypeOfSlider.Ads)
                    item.ImageAdFile = await _uw.AdminFiles.FindImageAsync(null, null, item.Id, null);
                else if (item.TypeOfSlider != 0)
                    item.ImageFiles = await _uw.AdminFiles.GetImagesAsync(null, null, item.Id);
            }

            return Json(new { total = total, rows = viewModels });
        }

        [HttpGet, DisplayName("درج و ویرایش"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> CreateOrUpdate(int sliderId)
        {
            ViewBag.ImageName = null;
            ViewBag.ImageAdName = null;
            var viewModel = new SliderViewModel();
            if (sliderId != 0)
            {
                ViewBag.SliderId = sliderId;
                var slider = await _uw.BaseService<Slider>().FindByIdAsync(sliderId);
                if (slider == null)
                    return BadRequest(SliderNotFound);

                viewModel = _mapper.Map<SliderViewModel>(slider);

                if (slider.TypeOfSlider == TypeOfSlider.Static)
                    ViewBag.ImageName = await _uw.AdminFiles.FindImageAsync(null, null, sliderId, null);
                else if (slider.TypeOfSlider == TypeOfSlider.Ads)
                    ViewBag.ImageAdName = await _uw.AdminFiles.FindImageAsync(null, null, sliderId, null);
                else if (slider.TypeOfSlider != 0)
                    ViewBag.Sliders = await _uw.AdminFiles.GetImagesAsync(null, null, sliderId);
            }
            else
            {
                ViewBag.SliderId = 0;
                var sliders = await _uw.BaseService<Slider>().FindByConditionAsync(null, p => p.OrderByDescending(p => p.Id));
                if (sliders.Count() != 0)
                    viewModel.Id = sliders.FirstOrDefault().Id + 1;
                else
                    viewModel.Id = 1;
            }
            return View(viewModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrUpdate(SliderViewModel viewModel)
        {
            ModelState.Remove("Name");
            if (ModelState.IsValid)
            {
                var IsSuccess = false;
                if (viewModel.Title.HasValue())
                    viewModel.Name = viewModel.Title;
                var slider = await _uw.BaseService<Slider>().FindByIdAsync(viewModel.Id);
                var advertising = _uw.BaseService<Advertising>().FindByConditionAsync(a => a.SliderId == viewModel.Id).Result.FirstOrDefault();
                if (viewModel.Id != 0 && slider != null)
                {
                    viewModel.UpdateTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now);
                    _uw.BaseService<Slider>().Update(slider);

                    if (viewModel.ImageAdFile.HasValue())
                    {
                        advertising.Url = viewModel.Url;
                        advertising.UpdateTime = viewModel.UpdateTime;
                        advertising.ImageLocation = viewModel.ImageLocation;
                        _uw.BaseService<Advertising>().Update(advertising);
                    }
                    await _uw.BaseService<Slider>().Commit();
                    IsSuccess = true;
                    TempData["notification"] = EditSuccess;
                }
                else
                {
                    if (viewModel.ImageFile.HasValue())
                        viewModel.TypeOfSlider = TypeOfSlider.Static;
                    else if (viewModel.ImageAdFile.HasValue())
                        viewModel.TypeOfSlider = TypeOfSlider.Ads;
                    else
                        viewModel.TypeOfSlider = TypeOfSlider.Dynamic;

                    viewModel.InsertTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now);
                    await _uw.BaseService<Slider>().CreateAsync(_mapper.Map<Slider>(viewModel));

                    if (viewModel.ImageAdFile.HasValue())
                    {
                        var advertisings = await _uw.BaseService<Advertising>().FindAllAsync();
                        if (advertisings.Count() != 0)
                            advertising.Id = advertisings.OrderByDescending(c => c.Id).First().Id + 1;
                        else
                            advertising.Id = 1;
                        advertising.Url = viewModel.Url;
                        advertising.SliderId = viewModel.Id;
                        advertising.InsertTime = viewModel.InsertTime;
                        advertising.ImageLocation = viewModel.ImageLocation;
                        await _uw.BaseService<Advertising>().CreateAsync(advertising);
                    }
                    await _uw.BaseService<Slider>().Commit();
                    IsSuccess = true;
                    TempData["notification"] = InsertSuccess;
                }
                var lastImage = await _uw.AdminFiles.FindImageAsync(null, null, viewModel.Id, null);
                if (IsSuccess && viewModel.ImageFile.HasValue())
                    await InsertImageAsync(viewModel.ImageFile, viewModel.Id, lastImage);
                if (IsSuccess && viewModel.ImageAdFile.HasValue())
                    await InsertImageAsync(viewModel.ImageAdFile, viewModel.Id, lastImage);
                if (IsSuccess && !viewModel.ImageFile.HasValue() && !viewModel.ImageAdFile.HasValue())
                    await InsertImagesUploaded(viewModel.Id);
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        [HttpPost, Route("UploadSlider")]
        public async Task<IActionResult> UploadImages(IEnumerable<IFormFile> files, int sliderId, int idOfSlider)
        {
            var nameImages = new List<string>();
            var uploadsRootFolder = Path.Combine(_env.WebRootPath, "assets/images/slider");
            var uploadInAngularFolder = Path.Combine(_env.WebRootPath + "\\..\\..\\..\\Angular\\src\\assets", "slider");

            foreach (var item in files)
            {
                if (item != null)
                {
                    var fileExtension = Path.GetExtension(item.FileName);
                    var imageName = String.Concat($"slider-{sliderId}-{idOfSlider}-{StringExtensions.GenerateId(10)}", fileExtension);
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
            return new JsonResult("success");
        }

        [HttpGet, AjaxOnly(), DisplayName("حذف"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> Delete(int sliderId)
        {
            if (sliderId == 0)
                ModelState.AddModelError(string.Empty, SliderNotFound);
            else
            {
                var slider = await _uw.BaseService<Slider>().FindByIdAsync(sliderId);
                if (slider == null)
                    ModelState.AddModelError(string.Empty, SliderNotFound);
                else
                    return PartialView("_DeleteConfirmation", slider);
            }
            return PartialView("_DeleteConfirmation");
        }

        [HttpPost, ActionName("Delete"), AjaxOnly]
        public async Task<IActionResult> DeleteConfirmed(Slider model)
        {
            if (model.Id == 0)
                ModelState.AddModelError(string.Empty, SliderNotFound);
            else
            {
                var slider = await _uw.BaseService<Slider>().FindByIdAsync(model.Id);
                if (slider == null)
                    ModelState.AddModelError(string.Empty, SliderNotFound);
                else
                {
                    slider.RemoveTime = DateTime.Now;
                    _uw.BaseService<Slider>().Update(slider);
                    await _uw.BaseService<Slider>().Commit();
                    TempData["notification"] = DeleteSuccess;
                    return PartialView("_DeleteConfirmation", slider);
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
                    var slider = await _uw.BaseService<Slider>().FindByIdAsync(int.Parse(splite));
                    slider.RemoveTime = DateTime.Now;
                    _uw.BaseService<Slider>().Update(slider);
                }
                await _uw.BaseService<Slider>().Commit();
                return Ok(DeleteGroupSuccess);
            }
            return BadRequest(SliderNotFound);
        }

        private async Task<List<string>> InsertImagesUploaded(int sliderId)
        {
            var nameImages = new List<string>();
            var uploadsRootFolder = Path.Combine(_env.WebRootPath, "assets/images/slider");
            var files = Directory.GetFiles(uploadsRootFolder);
            foreach (var file in files)
            {
                var info = new FileInfo(file);
                var name = info.Name;
                var spliteId = name.Contains($"slider-{sliderId}-");
                if (spliteId)
                    nameImages.Add(info.Name);
            }
            if (nameImages.Count != 0)
                await _uw.AdminFiles.InsertImagesAsync(null, null, sliderId, null, nameImages);
            else
                return nameImages;
            return null;
        }

        public async Task InsertImageAsync(string file, int sliderId, string imageName)
        {
            if (imageName.HasValue())
            {
                FileExtensions.DeleteFile($"{_env.WebRootPath}/assets/images/slider/{imageName}");
                FileExtensions.DeleteFile($"{_env.WebRootPath}/../../../Angular/src/assets/slider/{imageName}");
            }
            var image = $"slider-{StringExtensions.GenerateId(10)}.jpg";
            file.UploadFileBase64($"{_env.WebRootPath}/assets/images/slider/{image}");
            file.UploadFileBase64($"{_env.WebRootPath}/../../../Angular/src/assets/slider/{image}");

            var images = new List<string>();
            images.Add(image);
            await _uw.AdminFiles.InsertImagesAsync(null, null, sliderId, null, images);
        }
    }
}
