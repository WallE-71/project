using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ShoppingStore.Domain.Common;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Application.ViewModels.Product;
using ShoppingStore.Application.ViewModels.Api.Home;

namespace ShoppingStore.Application.Services
{
    public class SiteProducts : ISiteProducts
    {
        private readonly IDataBaseContext _context;
        private readonly IProductService _productService;
        public SiteProducts(IDataBaseContext context, IProductService productService)
        {
            _context = context;
            _productService = productService;
            //_productService = new ProductService(_context);
        }

        public async Task<List<ProductInCategoriesDto>> GetProductInCategoryAsync(string categoryName, string filter, int checkedDiscount, string typeDiscount)
        {
            var NameOfCategories = "";
            var date = DateTime.Now.Date;
            var viewModels = new List<ProductInCategoriesDto>();

            var products = await (from p in (from p in _context.Products.Include(s => s.ProductCategories)
                                             select (new
                                             {
                                                 p.Id,
                                                 p.Name,
                                                 p.Price,
                                                 p.States,
                                                 p.ProductCategories
                                             })).AsNoTracking()
                                  join e in _context.ProductCategories on p.Id equals e.ProductId into aa
                                  from productCategories in aa.DefaultIfEmpty()

                                  join d in _context.Discounts on p.Id equals d.ProductId into bb
                                  from discount in bb.DefaultIfEmpty()
                                  where (p.States == ProductState.Ready
                                    && p.ProductCategories.Select(pc => pc.Category.Name).Contains(categoryName))
                                    //&& (typeDiscount == 1 ? true && (discount.EndDate <= discount.StartDate.Value.AddDays(7)) :
                                    && (checkedDiscount == 1 ? true &&
                                            (typeDiscount == "کد تخفیف" ? true && (discount.DiscountCode != null) :
                                             typeDiscount == "تخفیف ویژه" ? true && (discount.DiscountDay != null && DateTime.Now >= discount.StartDate) :
                                             typeDiscount == "فصلی" ? true && (discount.EndDate.Value.Date <= discount.StartDate.Value.Date.AddMonths(3) && discount.EndDate.Value.Date > discount.StartDate.Value.Date.AddMonths(1)) :
                                             typeDiscount == "ماهانه" ? true && (discount.EndDate.Value.Date <= discount.StartDate.Value.Date.AddMonths(1) && discount.EndDate.Value.Date > discount.StartDate.Value.Date.AddDays(7)) :
                                             typeDiscount == "هفتگی" ? true && (discount.EndDate.Value.Date <= discount.StartDate.Value.Date.AddDays(7)) :
                                             typeDiscount == "روزانه" ? true && (discount.StartDate <= date && discount.EndDate <= date.EndOfDay()) :
                                             typeDiscount == "") :
                                        checkedDiscount == 2 || filter != null)
                                  select (new ProductInCategoriesDto
                                  {
                                      Id = p.Id,
                                      Name = p.Name,
                                      Price = p.Price,
                                      PercentDiscount = discount.Percent,
                                      DisplayDiscount = discount.GetDiscountName(),
                                      NameOfCategories = productCategories.Category != null ? productCategories.Category.Name : "",
                                  })).OrderBy(filter).Skip(0).Take(10).AsNoTracking().ToListAsync();

            var productGroup = products.GroupBy(g => g.Id).Select(g => new { ProductId = g.Key, ProductGroup = g });
            foreach (var item in productGroup)
            {
                NameOfCategories = "";
                foreach (var a in item.ProductGroup.Select(a => a.NameOfCategories).Distinct())
                {
                    if (NameOfCategories == "")
                        NameOfCategories = a;
                    else
                        NameOfCategories = NameOfCategories + " - " + a;
                }

                var product = new ProductInCategoriesDto()
                {
                    Id = item.ProductId,
                    Name = item.ProductGroup.First().Name,
                    Price = item.ProductGroup.First().Price,
                    NameOfCategories = NameOfCategories
                };
                viewModels.Add(product);
            }
            foreach (var viewModel in viewModels)
            {
                await _productService.ProductStateManagerAsync(viewModel.Id);
                var discount = await _productService.DiscountManagerAsync(viewModel.Id);
                viewModel.ActionDiscount = discount.ToString().En2Fa();
                viewModel.ImageName = await _productService.GetProductImage(viewModel.Id);
            }
            return viewModels;
        }

        public async Task<ProductViewModel> GetProductByIdAsync(string productId, string userId)
        {
            int id = 0;
            var NameOfCategories = "";
            Int32.TryParse(userId, out id);
            ProductViewModel viewModel = null;
            var productInfos = await (from p in _context.Products.Where(n => n.Id == productId).Include(v => v.Bookmarks).Include(v => v.Visits).Include(l => l.Comments).ThenInclude(u => u.Likes)
                                      where (p.SellerId != null && p.RemoveTime == null && p.States == ProductState.Ready)
                                      join e in _context.ProductCategories on p.Id equals e.ProductId into aa
                                      from productCategories in aa.DefaultIfEmpty()
                                      join c in _context.Categories on productCategories.CategoryId equals c.Id into bb
                                      from category in bb.DefaultIfEmpty()

                                      join b in _context.Brands on p.BrandId equals b.Id into dd
                                      from brand in dd.DefaultIfEmpty()

                                      join d in _context.Discounts on p.Id equals d.ProductId into ee
                                      from discount in ee.DefaultIfEmpty()
                                      select (new ProductViewModel
                                      {
                                          Id = p.Id,
                                          Name = p.Name,
                                          Price = p.Price,
                                          Description = p.Description,
                                          PercentDiscount = discount.Percent,
                                          DisplayDiscount = discount.GetDiscountName(),
                                          NameOfBrand = brand != null ? brand.Name : "",
                                          IdOfCategories = category != null ? category.Id : 0,
                                          NameOfCategories = category != null ? category.Name : "",
                                          NumberOfVisit = p.Visits.Select(v => v.NumberOfVisit).Sum(),
                                          NumberOfComments = p.Comments.Where(c => c.IsComplete == true).Count(),
                                          InsertTime = p.InsertTime == null ? new DateTime(01, 01, 01) : p.InsertTime,
                                          PersianExpirationDate = p.ExpirationDate.ConvertMiladiToShamsi("yyyy/MM/dd"),
                                          NumberOfLike = p.Comments.Select(c => c.Likes.Select(l => l.IsLiked == true)).Count(),
                                          ExpirationDate = p.ExpirationDate == null ? new DateTime(01, 01, 01) : p.ExpirationDate,
                                          NumberOfDisLike = p.Comments.Select(c => c.Likes.Select(l => l.IsLiked == false)).Count(),
                                          IsBookmarked = p.Bookmarks.Any(b => b.UserId == (userId != null ? id : 0) && b.ProductId == productId && b.RemoveTime == null),
                                          PersianInsertTime = p.InsertTime == null ? "-" : p.InsertTime.ConvertMiladiToShamsi("yyyy/MM/dd"),
                                      })).AsNoTracking().ToListAsync();

            if (productInfos.Count != 0)
            {
                var productGroup = productInfos.GroupBy(g => g.Id).Select(g => new { Id = g.Key, ProductGroup = g });
                foreach (var a in productGroup.First().ProductGroup.Select(a => a.NameOfCategories).Distinct())
                {
                    if (NameOfCategories == "")
                        NameOfCategories = a;
                    else
                        NameOfCategories = NameOfCategories + " - " + a;
                }

                viewModel = new ProductViewModel()
                {
                    NameOfCategories = NameOfCategories,
                    Id = productGroup.First().ProductGroup.First().Id,
                    Name = productGroup.First().ProductGroup.First().Name,
                    Price = productGroup.First().ProductGroup.First().Price,
                    Description = productGroup.First().ProductGroup.First().Description,
                    NameOfBrand = productGroup.First().ProductGroup.First().NameOfBrand,
                    IsBookmarked = productGroup.First().ProductGroup.First().IsBookmarked,
                    NumberOfLike = productGroup.First().ProductGroup.First().NumberOfLike,
                    NumberOfVisit = productGroup.First().ProductGroup.First().NumberOfVisit,
                    NumberOfDisLike = productGroup.First().ProductGroup.First().NumberOfDisLike,
                    NumberOfComments = productGroup.First().ProductGroup.First().NumberOfComments,
                    CategoryIdsList = productGroup.First().ProductGroup.Select(a => a.IdOfCategories).ToList(),
                    PercentDiscount = productGroup.First().ProductGroup.Select(a => a.PercentDiscount).First(),
                    DisplayDiscount = productGroup.First().ProductGroup.Select(a => a.DisplayDiscount).First(),
                    PersianInsertTime = productGroup.First().ProductGroup.First().InsertTime.ConvertMiladiToShamsi("yyyy/MM/dd"),
                    PersianExpirationDate = productGroup.First().ProductGroup.First().ExpirationDate.ConvertMiladiToShamsi("yyyy/MM/dd"),
                };

                if (viewModel.Description.HasValue())
                {
                    var description = viewModel.Description.Replace("<p>", "").Replace("</p>", "");
                    viewModel.Description = description;
                    viewModel.ShortName = description.Length >= 100 ? description.Substring(0, 100) + "..." : description;
                }
                await _productService.ProductStateManagerAsync(viewModel.Id);
                var discount = await _productService.DiscountManagerAsync(viewModel.Id);
                viewModel.ActionDiscount = discount.ToString().En2Fa();
                viewModel.ImageName = await _productService.GetProductImage(viewModel.Id);
                viewModel.ImageFiles = await _productService.GetProductImagesAsync(viewModel.Id, 0, 5);
            }
            return viewModel;
        }


        public async Task<List<ProductInCategoriesDto>> GetRelatedProductAsync(int number, List<int> categoryIdList, string productId)
        {
            var categoryIds = new List<string>();
            foreach (var str in categoryIdList)
                categoryIds.Add(str.ToString());

            var randomRow = 0;
            var recentRandomRow = 0;
            var whereExpression = "ProductId!=@0 and (";
            categoryIds.Insert(0, productId);
            for (int i = 0; i < categoryIds.Count() - 1; i++)
                whereExpression = whereExpression + @"CategoryId==@" + (i + 1) + (i + 1 != categoryIds.Count - 1 ? " or " : ")");

            var viewModels = new List<ProductInCategoriesDto>();
            int productCount = (from p in _context.Products.Where(p => p.States == ProductState.Ready)
                                join b in _context.ProductCategories.Where(whereExpression, categoryIds.ToArray())
                                   on p.Id equals b.ProductId
                                select p).Count();

            for (int i = 0; i < number && i < productCount; i++)
            {
                randomRow = CustomMethods.RandomNumber(1, productCount + 1);
                while (recentRandomRow == randomRow)
                    randomRow = CustomMethods.RandomNumber(1, productCount + 1);

                var product = await (from p in _context.Products
                                     .Include(p => p.Visits).Include(p => p.Comments).ThenInclude(c => c.Likes)
                                     .Where(p => p.States == ProductState.Ready)
                                     select new ProductInCategoriesDto
                                     {
                                         Id = p.Id,
                                         Name = p.Name,
                                         Price = p.Price,
                                         NumberOfComments = p.Comments.Count(),
                                         NumberOfVisit = p.Visits.Select(v => v.NumberOfVisit).Sum(),
                                         NumberOfLike = p.Comments.Select(c => c.Likes.Select(l => l.IsLiked == true)).Count(),
                                         NumberOfDisLike = p.Comments.Select(c => c.Likes.Select(l => l.IsLiked == false)).Count(),
                                     }).Skip(randomRow - 1).Take(1).FirstOrDefaultAsync();

                if (product != null)
                    viewModels.Add(product);
                recentRandomRow = randomRow;
            }
            foreach (var viewModel in viewModels)
            {
                await _productService.ProductStateManagerAsync(viewModel.Id);
                var discount = await _productService.DiscountManagerAsync(viewModel.Id);
                viewModel.ActionDiscount = discount.ToString().En2Fa();
                viewModel.ImageName = await _productService.GetProductImage(viewModel.Id);
            }
            return viewModels;
        }

        public async Task<List<ProductInCategoriesDto>> SpecialDiscountProductsAsync(string day)
        {
            var products = await (from p in _context.Products
                                  join d in _context.Discounts on p.Id equals d.ProductId into aa
                                  from discount in aa.DefaultIfEmpty()
                                  where (p.States == ProductState.Ready &&
                                    (discount != null && discount.EndDate != null && discount.DiscountDay == day))
                                  select (new ProductInCategoriesDto
                                  {
                                      Id = p.Id,
                                      Name = p.Name,
                                      Price = p.Price,
                                      PercentDiscount = discount.Percent,
                                      NumberOfComments = p.Comments.Count(),
                                      DisplayDiscount = discount.GetDiscountName(),
                                      NumberOfVisit = p.Visits.Select(v => v.NumberOfVisit).Sum(),
                                      InsertTime = p.InsertTime == null ? new DateTime(01, 01, 01) : p.InsertTime,
                                  })).OrderByDescending(p => p.PercentDiscount).AsNoTracking().ToListAsync();

            foreach (var product in products)
            {
                await _productService.ProductStateManagerAsync(product.Id);
                var discount = await _productService.DiscountManagerAsync(product.Id);
                product.ActionDiscount = discount.ToString().En2Fa();
                product.ImageName = await _productService.GetProductImage(product.Id);
            }
            return products;
        }

        public async Task<List<ProductInCategoriesDto>> GetPreferedProductsNoDiscountAsync()
        {
            var viewModels = await (from p in _context.Products
                                    join d in _context.Discounts on p.Id equals d.ProductId into aa
                                    from discount in aa.DefaultIfEmpty()
                                    join s in _context.Sellers on p.SellerId equals s.Id into bb
                                    from seller in bb.DefaultIfEmpty()
                                    join r in _context.Ratings on seller.Id equals r.SellerId into cc
                                    from rating in cc.DefaultIfEmpty()
                                    where (p.States == ProductState.Ready && seller != null && p.IsPrefered == true && discount == null)
                                    select new ProductInCategoriesDto
                                    {
                                        Id = p.Id,
                                        Name = p.Name,
                                        Price = p.Price,
                                        Description = p.Description,
                                        BestOfSellers = rating.UserVote != 0 ? rating.UserVote : 0
                                    }).OrderByDescending(p => p.BestOfSellers).Skip(0).Take(5).ToListAsync();

            foreach (var viewModel in viewModels)
            {
                await _productService.ProductStateManagerAsync(viewModel.Id);
                viewModel.ImageName = await _productService.GetProductImage(viewModel.Id);
            }
            return viewModels;
        }

        public async Task<List<ProductInCategoriesDto>> GetPreferedProductsAsync()
        {
            var viewModels = await (from p in _context.Products
                                    join d in _context.Discounts on p.Id equals d.ProductId into aa
                                    from discount in aa.DefaultIfEmpty()
                                    where (p.States == ProductState.Ready && p.IsPrefered == true && discount != null && discount.DiscountDay == null)
                                    select new ProductInCategoriesDto
                                    {
                                        Id = p.Id,
                                        Name = p.Name,
                                        Price = p.Price,
                                        Description = p.Description,
                                        PercentDiscount = discount.Percent,
                                    }).Skip(0).Take(5).ToListAsync();

            foreach (var viewModel in viewModels)
            {
                await _productService.ProductStateManagerAsync(viewModel.Id);
                var discount = await _productService.DiscountManagerAsync(viewModel.Id);
                viewModel.ActionDiscount = discount.ToString().En2Fa();
                viewModel.ImageName = await _productService.GetProductImage(viewModel.Id);
            }
            return viewModels;
        }

        public async Task<List<ProductInCategoriesDto>> MostPopularProductsAsync()
        {
            //int? maxNumberOfSale = 0;
            //var _product = _context.Products.Where(p => p.States == ProductState.Ready && p.NumberOfSale != null).ToList();
            //if (_product.Count != 0)
            //    maxNumberOfSale = _product.OrderByDescending(p => p.NumberOfSale).FirstOrDefault().NumberOfSale;

            var products = await (from p in _context.Products
                                  where (p.States == ProductState.Ready && p.NumberOfSale != null)
                                  //(p.NumberOfSale > maxNumberOfSale))
                                  select (new ProductInCategoriesDto
                                  {
                                      Id = p.Id,
                                      Name = p.Name,
                                      Price = p.Price,
                                      NumberOfSale = p.NumberOfSale
                                  })).OrderByDescending(p => p.NumberOfSale).AsNoTracking().ToListAsync();

            foreach (var product in products)
            {
                await _productService.ProductStateManagerAsync(product.Id);
                product.ImageName = await _productService.GetProductImage(product.Id);
            }
            return products;
        }

        public async Task<List<ProductInCategoriesDto>> MostDiscountProductAsync()
        {
            var products = await (from p in _context.Products.Include(l => l.Comments).ThenInclude(c => c.Likes)
                                  join d in _context.Discounts on p.Id equals d.ProductId into aa
                                  from discount in aa.DefaultIfEmpty()
                                  where (p.States == ProductState.Ready && (discount != null && discount.EndDate != null))
                                  select (new ProductInCategoriesDto
                                  {
                                      Id = p.Id,
                                      Name = p.Name,
                                      Price = p.Price,
                                      PercentDiscount = discount.Percent
                                  })).OrderByDescending(p => p.PercentDiscount).AsNoTracking().ToListAsync();

            foreach (var product in products)
            {
                await _productService.ProductStateManagerAsync(product.Id);
                var discount = await _productService.DiscountManagerAsync(product.Id);
                product.ActionDiscount = discount.ToString().En2Fa();
                product.ImageName = await _productService.GetProductImage(product.Id);
            }
            return products;
        }

        public async Task<List<ProductInCategoriesDto>> MostViewedProductAsync()
        {
            var nameOfCategories = "";
            var startMiladiDate = new DateTime();
            var viewModels = new List<ProductInCategoriesDto>();

            var numOfWeek = DateTimeExtensions.ConvertMiladiToShamsi(DateTime.Now, "dddd").GetNumOfWeek();
            startMiladiDate = DateTime.Now.AddDays((-6) * numOfWeek).Date + new TimeSpan(0, 0, 0);

            var products = await (from p in ((from p in _context.Products.Include(p => p.Visits)
                                              where (p.States == ProductState.Ready && startMiladiDate <= p.InsertTime)
                                              select (new
                                              {
                                                  p.Id,
                                                  p.Price,
                                                  NumberOfVisit = p.Visits.Select(v => v.NumberOfVisit).Sum(),
                                                  ProductName = p.Name.Length > 60 ? p.Name.Substring(0, 60) + "..." : p.Name,
                                                  CreateDateTime = p.InsertTime == null ? new DateTime(01, 01, 01) : p.InsertTime,
                                              })).Where(p => p.NumberOfVisit > 0)
                                                    .OrderByDescending(p => p.NumberOfVisit).Skip(0).Take(5))
                                  join e in _context.ProductCategories on p.Id equals e.ProductId into bc
                                  from bct in bc.DefaultIfEmpty()
                                  join c in _context.Categories on bct.CategoryId equals c.Id into cg
                                  from cog in cg.DefaultIfEmpty()
                                  select (new
                                  {
                                      p.Id,
                                      p.Price,
                                      p.ProductName,
                                      p.NumberOfVisit,
                                      p.CreateDateTime,
                                      Name = cog != null ? cog.Name : "",
                                  })).AsNoTracking().ToListAsync();

            var productsGroup = products.GroupBy(g => g.Id).Select(g => new { ProductId = g.Key, ProductGroup = g });
            foreach (var item in productsGroup)
            {
                nameOfCategories = "";
                foreach (var a in item.ProductGroup.Select(a => a.Name).Distinct())
                {
                    if (nameOfCategories == "")
                        nameOfCategories = a;
                    else
                        nameOfCategories = nameOfCategories + " - " + a;
                }

                var viewModel = new ProductInCategoriesDto()
                {
                    Id = item.ProductId,
                    Price = item.ProductGroup.First().Price,
                    Name = item.ProductGroup.First().ProductName,
                    InsertTime = item.ProductGroup.First().CreateDateTime,
                    NumberOfVisit = item.ProductGroup.First().NumberOfVisit,
                    NameOfCategories = nameOfCategories
                };
                viewModels.Add(viewModel);
            }
            foreach (var viewModel in viewModels)
            {
                await _productService.ProductStateManagerAsync(viewModel.Id);
                var discount = await _productService.DiscountManagerAsync(viewModel.Id);
                viewModel.ActionDiscount = discount.ToString().En2Fa();
                viewModel.ImageName = await _productService.GetProductImage(viewModel.Id);
            }
            return viewModels;
        }

        public async Task<List<ProductInCategoriesDto>> MostTalkProductAsync()
        {
            var products = await (from p in _context.Products.Include(l => l.Comments).ThenInclude(c => c.Likes)
                                  where (p.States == ProductState.Ready)
                                  select (new ProductInCategoriesDto
                                  {
                                      Id = p.Id,
                                      Name = p.Name,
                                      Price = p.Price,
                                      NumberOfComments = p.Comments.Count(),
                                      NumberOfLike = p.Comments.Select(c => c.Likes.Select(l => l.IsLiked == true)).Count(),
                                      NumberOfDisLike = p.Comments.Select(c => c.Likes.Select(l => l.IsLiked == false)).Count(),
                                  }))
                                  .Where(p => p.NumberOfComments != 0)
                                  .OrderByDescending(p => p.NumberOfComments).AsNoTracking().ToListAsync();

            foreach (var product in products)
            {
                await _productService.ProductStateManagerAsync(product.Id);
                var discount = await _productService.DiscountManagerAsync(product.Id);
                product.ActionDiscount = discount.ToString().En2Fa();
                product.ImageName = await _productService.GetProductImage(product.Id);
            }
            return products;
        }


        public async Task InsertVisitOfUserAsync(string productId, Guid? browserId, string ipAddress)
        {
            var visit = _context.Visits.Where(v => v.ProductId == productId && v.IpAddress == ipAddress).FirstOrDefault();
            if (visit != null && visit.LastVisitDateTime.Date != DateTime.Now.Date)
            {
                visit.BrowserId = browserId;
                visit.LastVisitDateTime = DateTime.Now;
                visit.NumberOfVisit = visit.NumberOfVisit + 1;
                _context.Visits.Update(visit);
                await _context.SaveChangesAsync();
            }
            else if (visit == null)
            {
                visit = new Visit { IpAddress = ipAddress, BrowserId = browserId, LastVisitDateTime = DateTime.Now, ProductId = productId, NumberOfVisit = 1 };
                await _context.Visits.AddAsync(visit);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<string> GetWeeklyProductDiscountAsync(string url)
        {
            var content = "";
            var endMiladiDate = DateTime.Now;
            var numOfWeek = DateTimeExtensions.ConvertMiladiToShamsi(DateTime.Now, "dddd").GetNumOfWeek();
            var startMiladiDate = DateTime.Now.AddDays((-1) * numOfWeek).Date + new TimeSpan(0, 0, 0);

            var viewModels = await (from p in _context.Products.Include(v => v.Visits).Include(l => l.Comments).ThenInclude(c => c.Likes)
                                    where (p.InsertTime <= endMiladiDate && startMiladiDate <= p.InsertTime)
                                    select (new ProductInCategoriesDto
                                    {
                                        Id = p.Id,
                                        Price = p.Price,
                                        Name = p.Name.Length > 50 ? p.Name.Substring(0, 50) + "..." : p.Name,
                                    })).OrderByDescending(o => o.InsertTime).AsNoTracking().ToListAsync();

            foreach (var item in viewModels)
            {
                await _productService.ProductStateManagerAsync(item.Id);
                var discount = await _productService.DiscountManagerAsync(item.Id);
                item.ActionDiscount = discount.ToString().En2Fa();
                item.ImageName = await _productService.GetProductImage(item.Id);
                content = content +
                    $" <div style='direction:rtl;font-family:tahoma;text-align:center'>" +
                    $" <div class='row align-items-center'>" +
                    $" <div class='col-12 col-lg-6'>" +
                    $" <div class='post-thumbnail'>" +
                    $" <img src='{url + "/productImage/" + item.ImageName}' alt='{item.ImageName}'> " +
                    $" </div>" +
                    $" </div> <div class='col-12 col-lg-6'>" +
                    $" <div class='post-content mt-0'>" +
                    $" <h4 style='color:#878484;'>{item.Name}</h4>" +
                    $" <p> <a href='{url}'>[ادامه مطلب]</a> </p>" +
                    $" </div> </div> </div> </div><hr/>";
            }
            return content;
        }

        //public string SearchInProducts(string searchText) => _context.Products.SingleOrDefault(p => (p.Name.Contains(searchText) == true || p.Description.Contains(searchText)) && p.States == ProductState.Ready).Id;
    }
}
