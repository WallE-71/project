using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Application.ViewModels.Product;

namespace ShoppingStore.Application.Services
{
    public class AdminProducts : IAdminProducts
    {
        private readonly IDataBaseContext _context;
        private readonly IProductService _productService;
        public AdminProducts(IDataBaseContext context, IProductService productService)
        {
            _context = context;
            _productService = productService;
            //_productService = new ProductService(_context);

        }

        public async Task<List<ProductViewModel>> GetPaginateProductAsync(int offset, int limit, string orderBy, string searchText, bool? IsDemo, bool? isSeller, long sellerId = 0)
        {
            var nameOfSeller = "";
            var displayDiscount = "";
            var nameOfCategories = "";
            var viewModels = new List<ProductViewModel>();
            var convertPresent = Convert.ToBoolean(IsDemo);
            var convertSeller = Convert.ToBoolean(isSeller);
            var getDateTimesForSearch = searchText.GetDateTimeForSearch();

            var products = await (from p in ((from p in _context.Products.Include(p => p.Visits).Include(p => p.Comments)
                                              where ((sellerId == 0 || convertSeller == false || p.SellerId == sellerId && convertSeller == true)
                                                  && (sellerId == 0 ? true : convertSeller == false ? p.States == ProductState.Ready && p.SellerId == null : p.SellerId == sellerId && convertSeller == true)
                                                  || p.RemoveTime == null && p.Name.Contains(searchText) && (p.InsertTime >= getDateTimesForSearch.First() && p.InsertTime <= getDateTimesForSearch.Last()))
                                                  && (IsDemo == null || (convertPresent ? p.IsComplete && p.InsertTime <= DateTime.Now : !p.IsComplete))
                                              select (new
                                              {
                                                  p.Id,
                                                  p.Name,
                                                  p.Price,
                                                  p.Stock,
                                                  p.States,
                                                  p.Barcode,
                                                  p.SellerId,
                                                  p.InsertTime,
                                                  p.IsPrefered,
                                                  p.NumberOfSale,
                                                  p.ExpirationDate,
                                                  NumberOfComments = p.Comments.Count(),
                                                  NumberOfVisit = p.Visits.Select(v => v.NumberOfVisit).Sum()
                                              })).OrderBy(orderBy).Skip(offset).Take(limit))
                                  join pc in _context.ProductCategories on p.Id equals pc.ProductId into aa
                                  from productCategory in aa.DefaultIfEmpty()
                                  join c in _context.Categories on productCategory.CategoryId equals c.Id into bb
                                  from category in bb.DefaultIfEmpty()

                                  join s in _context.Sellers on p.SellerId equals s.Id into gg
                                  from seller in gg.DefaultIfEmpty()
                                  join d in _context.Discounts on p.Id equals d.ProductId into jj
                                  from discount in jj.DefaultIfEmpty()
                                  select (new ProductViewModel
                                  {
                                      Id = p.Id,
                                      Name = p.Name,
                                      Price = p.Price,
                                      Stock = p.Stock,
                                      Barcode = p.Barcode,
                                      SellerId = p.SellerId,
                                      InsertTime = p.InsertTime,
                                      IsPrefered = p.IsPrefered,
                                      NumberOfSale = p.NumberOfSale,
                                      NumberOfVisit = p.NumberOfVisit,
                                      ExpirationDate = p.ExpirationDate,
                                      PercentDiscount = discount.Percent,
                                      NumberOfComments = p.NumberOfComments,
                                      DisplayPrice = p.Price.ToString().En2Fa(),
                                      DisplayStates = p.States.GetProductState(),
                                      DisplayDiscount = discount.GetDiscountName(),
                                      NameOfSeller = seller != null ? seller.Name : "",
                                      NameOfCategories = category != null ? category.Name : "",
                                      PersianInsertTime = p.InsertTime.ConvertMiladiToShamsi("yyyy/MM/dd"),
                                      ShortName = p.Name.Length > 20 ? p.Name.Substring(0, 20) + "..." : p.Name,
                                      PersianExpirationDate = p.ExpirationDate.ConvertMiladiToShamsi("yyyy/MM/dd"),
                                  })).AsNoTracking().ToListAsync();

            var productGroup = products.GroupBy(g => g.Id).Select(g => new { ProductId = g.Key, ProductGroup = g });
            foreach (var item in productGroup)
            {
                nameOfCategories = "";
                foreach (var a in item.ProductGroup.Select(a => a.NameOfCategories).Distinct())
                {
                    if (nameOfCategories == "")
                        nameOfCategories = a;
                    else
                        nameOfCategories = nameOfCategories + " - " + a;
                }

                nameOfSeller = "";
                foreach (var a in item.ProductGroup.Select(a => a.NameOfSeller).Distinct())
                {
                    if (nameOfSeller == "")
                        nameOfSeller = a;
                    else
                        nameOfSeller = nameOfSeller + " - " + a;
                }

                displayDiscount = "";
                foreach (var a in item.ProductGroup.Select(a => a.DisplayDiscount).Distinct())
                {
                    if (displayDiscount == "")
                        displayDiscount = a;
                    else
                        displayDiscount = displayDiscount + " - " + a;
                }

                var product = new ProductViewModel()
                {
                    Id = item.ProductId,
                    NameOfSeller = nameOfSeller,
                    DisplayDiscount = displayDiscount,
                    NameOfCategories = nameOfCategories,
                    Name = item.ProductGroup.First().Name,
                    Stock = item.ProductGroup.First().Stock,
                    Barcode = item.ProductGroup.First().Barcode,
                    SellerId = item.ProductGroup.First().SellerId,
                    ShortName = item.ProductGroup.First().ShortName,
                    ImageName = item.ProductGroup.First().ImageName,
                    IsPrefered = item.ProductGroup.First().IsPrefered,
                    DisplayPrice = item.ProductGroup.First().DisplayPrice,
                    NumberOfSale = item.ProductGroup.First().NumberOfSale,
                    DisplayStates = item.ProductGroup.First().DisplayStates,
                    NumberOfVisit = item.ProductGroup.First().NumberOfVisit,
                    PercentDiscount = item.ProductGroup.First().PercentDiscount,
                    NumberOfComments = item.ProductGroup.First().NumberOfComments,
                    PersianInsertTime = item.ProductGroup.First().InsertTime == null ? "-" : item.ProductGroup.First().InsertTime > DateTime.Now ? DateTimeExtensions.ConvertMiladiToShamsi(item.ProductGroup.First().InsertTime, "yyyy/MM/dd") + " " + "(در دست تولید)" : DateTimeExtensions.ConvertMiladiToShamsi(item.ProductGroup.First().InsertTime, "yyyy/MM/dd"),
                    PersianExpirationDate = item.ProductGroup.First().ExpirationDate == null ? "-" : item.ProductGroup.First().ExpirationDate < DateTime.Now ? DateTimeExtensions.ConvertMiladiToShamsi(item.ProductGroup.First().ExpirationDate, "yyyy/MM/dd") + " " + "(منقضی شده)" : DateTimeExtensions.ConvertMiladiToShamsi(item.ProductGroup.First().ExpirationDate, "yyyy/MM/dd")
                };
                viewModels.Add(product);
            }

            foreach (var viewModel in viewModels)
            {
                viewModel.Row = ++offset;
                await _productService.ProductStateManagerAsync(viewModel.Id);
                var discount = await _productService.DiscountManagerAsync(viewModel.Id);
                viewModel.ActionDiscount = discount.ToString().En2Fa();
                viewModel.ImageName = await _productService.GetProductImage(viewModel.Id);
            }
            return viewModels;
        }

        public async Task<int> GetNumberOfVisitAsync(DateTime EndDateTimeMiladi, DateTime StartDateTimeMiladi)
        {
            var numberVisits = await _context.Products.Include(p => p.Visits)
                                            .Where(p => p.InsertTime < EndDateTimeMiladi && StartDateTimeMiladi <= p.InsertTime)
                                            .Select(p => p.Visits.Sum(v => v.NumberOfVisit)).ToListAsync();

            return numberVisits.AsEnumerable().Sum();
        }

        public async Task<List<ProductViewModel>> ProductDetailFeatures(string productId)
        {
            var nameOfBrand = "";
            var nameOfColor = "";
            var nameOfMaterial = "";
            var nameOfGuarantee = "";
            var viewModels = new List<ProductViewModel>();

            var products = await (from p in ((from p in _context.Products
                                              where (p.Id == productId)
                                              select (new
                                              {
                                                  p.Id,
                                                  p.Name,
                                                  p.Size,
                                                  p.Stock,
                                                  p.Price,
                                                  p.Weight,
                                                  p.MadeIn,
                                                  p.States,
                                                  p.BrandId,
                                                  p.InsertTime,
                                                  p.Description,
                                                  p.GuaranteeId,
                                                  p.ExpirationDate,
                                              })).AsNoTracking())
                                  join pm in _context.ProductMaterials on p.Id equals pm.ProductId into bb
                                  from productMaterial in bb.DefaultIfEmpty()
                                  join pc in _context.ProductColors on p.Id equals pc.ProductId into dd
                                  from productColor in dd.DefaultIfEmpty()

                                  join g in _context.Guarantees on p.GuaranteeId equals g.Id into ff
                                  from guarantee in ff.DefaultIfEmpty()
                                  join b in _context.Brands on p.BrandId equals b.Id into gg
                                  from brand in gg.DefaultIfEmpty()
                                  select (new ProductViewModel
                                  {
                                      Id = p.Id,
                                      Name = p.Name,
                                      Size = p.Size,
                                      Stock = p.Stock,
                                      Price = p.Price,
                                      Weight = p.Weight,
                                      MadeIn = p.MadeIn,
                                      States = p.States,
                                      InsertTime = p.InsertTime,
                                      Description = p.Description,
                                      ExpirationDate = p.ExpirationDate,
                                      DisplayStates = p.States.GetProductState(),
                                      NameOfBrand = brand != null ? brand.Name : "",
                                      NameOfGuarantee = guarantee != null ? guarantee.Name : "",
                                      NameOfColor = productColor != null ? productColor.Color.Name : "",
                                      NameOfMaterial = productMaterial != null ? productMaterial.Material.Name : "",
                                  })).AsNoTracking().ToListAsync();

            var productGroup = products.GroupBy(g => g.Id).Select(g => new { ProductId = g.Key, ProductGroup = g });
            foreach (var item in productGroup)
            {
                nameOfColor = "";
                foreach (var a in item.ProductGroup.Select(a => a.NameOfColor).Distinct())
                {
                    if (nameOfColor == "")
                        nameOfColor = a;
                    else
                        nameOfColor = nameOfColor + " , " + a;
                }

                nameOfMaterial = "";
                foreach (var a in item.ProductGroup.Select(a => a.NameOfMaterial).Distinct())
                {
                    if (nameOfMaterial == "")
                        nameOfMaterial = a;
                    else
                        nameOfMaterial = nameOfMaterial + " , " + a;
                }

                nameOfBrand = "";
                foreach (var a in item.ProductGroup.Select(a => a.NameOfBrand).Distinct())
                {
                    if (nameOfBrand == "")
                        nameOfBrand = a;
                    else
                        nameOfBrand = nameOfBrand + " - " + a;
                }

                nameOfGuarantee = "";
                foreach (var a in item.ProductGroup.Select(a => a.NameOfGuarantee).Distinct())
                {
                    if (nameOfGuarantee == "")
                        nameOfGuarantee = a;
                    else
                        nameOfGuarantee = nameOfGuarantee + " - " + a;
                }

                var product = new ProductViewModel()
                {
                    Id = item.ProductId,
                    NameOfBrand = nameOfBrand,
                    NameOfColor = nameOfColor,
                    NameOfMaterial = nameOfMaterial,
                    NameOfGuarantee = nameOfGuarantee,
                    Name = item.ProductGroup.First().Name,
                    Size = item.ProductGroup.First().Size,
                    Stock = item.ProductGroup.First().Stock,
                    Price = item.ProductGroup.First().Price,
                    Weight = item.ProductGroup.First().Weight,
                    MadeIn = item.ProductGroup.First().MadeIn,
                    States = item.ProductGroup.First().States,
                    ImageName = item.ProductGroup.First().ImageName,
                    InsertTime = item.ProductGroup.First().InsertTime,
                    Description = item.ProductGroup.First().Description,
                    DisplayStates = item.ProductGroup.First().DisplayStates,
                    ExpirationDate = item.ProductGroup.First().ExpirationDate,
                    PercentDiscount = item.ProductGroup.First().PercentDiscount,
                    PersianInsertTime = item.ProductGroup.First().InsertTime == null ? "-" : item.ProductGroup.First().InsertTime > DateTime.Now ? DateTimeExtensions.ConvertMiladiToShamsi(item.ProductGroup.First().InsertTime, "yyyy/MM/dd") + " " + "(در دست تولید)" : DateTimeExtensions.ConvertMiladiToShamsi(item.ProductGroup.First().InsertTime, "yyyy/MM/dd"),
                    PersianExpirationDate = item.ProductGroup.First().ExpirationDate == null ? "-" : item.ProductGroup.First().ExpirationDate < DateTime.Now ? DateTimeExtensions.ConvertMiladiToShamsi(item.ProductGroup.First().ExpirationDate, "yyyy/MM/dd") + " " + "(منقضی شده)" : DateTimeExtensions.ConvertMiladiToShamsi(item.ProductGroup.First().ExpirationDate, "yyyy/MM/dd")
                };
                viewModels.Add(product);
            }
            foreach (var viewModel in viewModels)
            {
                await _productService.ProductStateManagerAsync(viewModel.Id);
                var discount = await _productService.DiscountManagerAsync(viewModel.Id);
                viewModel.ActionDiscount = discount.ToString().En2Fa();
                viewModel.ImageName = await _productService.GetProductImage(viewModel.Id);
                if (viewModel.Description.HasValue())
                    viewModel.Description = viewModel.Description.Replace("<p>", "").Replace("</p>", "");
            }
            return viewModels;
        }

        public async Task<List<ProductViewModel>> FindProductByIdAsync(string productId)
        {
            var nameOfBrand = "";
            var nameOfColor = "";
            var nameOfMaterial = "";
            var nameOfGuarantee = "";
            var nameOfCategories = "";
            var viewModels = new List<ProductViewModel>();

            var products = await (from p in ((from p in _context.Products
                                              where (p.Id == productId)
                                              select (new
                                              {
                                                  p.Id,
                                                  p.Name,
                                                  p.Size,
                                                  p.Stock,
                                                  p.Price,
                                                  p.Weight,
                                                  p.MadeIn,
                                                  p.States,
                                                  p.BrandId,
                                                  p.InsertTime,
                                                  p.Description,
                                                  p.GuaranteeId,
                                                  p.ExpirationDate,
                                              })).AsNoTracking())
                                  join c in _context.ProductCategories on p.Id equals c.ProductId into aa
                                  from productCategory in aa.DefaultIfEmpty()

                                  join pm in _context.ProductMaterials on p.Id equals pm.ProductId into bb
                                  from productMaterial in bb.DefaultIfEmpty()
                                  join m in _context.Materials on productMaterial.MaterialId equals m.Id into cc
                                  from material in cc.DefaultIfEmpty()

                                  join pc in _context.ProductColors on p.Id equals pc.ProductId into dd
                                  from productColor in dd.DefaultIfEmpty()
                                  join co in _context.Colors on productColor.ColorId equals co.Id into ee
                                  from color in ee.DefaultIfEmpty()

                                  join g in _context.Guarantees on p.GuaranteeId equals g.Id into ff
                                  from guarantee in ff.DefaultIfEmpty()

                                  join b in _context.Brands on p.BrandId equals b.Id into gg
                                  from brand in gg.DefaultIfEmpty()
                                  select (new ProductViewModel
                                  {
                                      Id = p.Id,
                                      Name = p.Name,
                                      Size = p.Size,
                                      Stock = p.Stock,
                                      Price = p.Price,
                                      Weight = p.Weight,
                                      MadeIn = p.MadeIn,
                                      States = p.States,
                                      InsertTime = p.InsertTime,
                                      Description = p.Description,
                                      ExpirationDate = p.ExpirationDate,
                                      NameOfBrand = brand != null ? brand.Name : "",
                                      NameOfColor = color != null ? color.Name : "",
                                      NameOfMaterial = material != null ? material.Name : "",
                                      NameOfGuarantee = guarantee != null ? guarantee.Name : "",
                                      IdOfCategories = productCategory != null ? productCategory.CategoryId : 0,
                                      NameOfCategories = productCategory != null ? productCategory.Category.Name : "",
                                  })).AsNoTracking().ToListAsync();

            var productGroup = products.GroupBy(g => g.Id).Select(g => new { ProductId = g.Key, ProductGroup = g });
            foreach (var item in productGroup)
            {
                nameOfColor = "";
                foreach (var a in item.ProductGroup.Select(a => a.NameOfColor).Distinct())
                {
                    if (nameOfColor == "")
                        nameOfColor = a;
                    else
                        nameOfColor = nameOfColor + "," + a;
                }

                nameOfMaterial = "";
                foreach (var a in item.ProductGroup.Select(a => a.NameOfMaterial).Distinct())
                {
                    if (nameOfMaterial == "")
                        nameOfMaterial = a;
                    else
                        nameOfMaterial = nameOfMaterial + "," + a;
                }

                nameOfBrand = "";
                foreach (var a in item.ProductGroup.Select(a => a.NameOfBrand).Distinct())
                {
                    if (nameOfBrand == "")
                        nameOfBrand = a;
                    else
                        nameOfBrand = nameOfBrand + " - " + a;
                }

                nameOfGuarantee = "";
                foreach (var a in item.ProductGroup.Select(a => a.NameOfGuarantee).Distinct())
                {
                    if (nameOfGuarantee == "")
                        nameOfGuarantee = a;
                    else
                        nameOfGuarantee = nameOfGuarantee + " - " + a;
                }

                nameOfCategories = "";
                foreach (var a in item.ProductGroup.Select(a => a.NameOfCategories).Distinct())
                {
                    if (nameOfCategories == "")
                        nameOfCategories = a;
                    else
                        nameOfCategories = nameOfCategories + " - " + a;
                }

                var product = new ProductViewModel()
                {
                    Id = item.ProductId,
                    NameOfBrand = nameOfBrand,
                    NameOfColor = nameOfColor,
                    NameOfMaterial = nameOfMaterial,
                    NameOfGuarantee = nameOfGuarantee,
                    NameOfCategories = nameOfCategories,
                    Name = item.ProductGroup.First().Name,
                    Size = item.ProductGroup.First().Size,
                    Stock = item.ProductGroup.First().Stock,
                    Price = item.ProductGroup.First().Price,
                    Weight = item.ProductGroup.First().Weight,
                    MadeIn = item.ProductGroup.First().MadeIn,
                    States = item.ProductGroup.First().States,
                    ImageName = item.ProductGroup.First().ImageName,
                    InsertTime = item.ProductGroup.First().InsertTime,
                    Description = item.ProductGroup.First().Description,
                    ExpirationDate = item.ProductGroup.First().ExpirationDate,
                    IdOfCategories = item.ProductGroup.First().IdOfCategories,
                    PercentDiscount = item.ProductGroup.First().PercentDiscount,
                    PersianInsertTime = item.ProductGroup.First().InsertTime == null ? "-" : item.ProductGroup.First().InsertTime > DateTime.Now ? DateTimeExtensions.ConvertMiladiToShamsi(item.ProductGroup.First().InsertTime, "yyyy/MM/dd") + " " + "(در دست تولید)" : DateTimeExtensions.ConvertMiladiToShamsi(item.ProductGroup.First().InsertTime, "yyyy/MM/dd"),
                    PersianExpirationDate = item.ProductGroup.First().ExpirationDate == null ? "-" : item.ProductGroup.First().ExpirationDate < DateTime.Now ? DateTimeExtensions.ConvertMiladiToShamsi(item.ProductGroup.First().ExpirationDate, "yyyy/MM/dd") + " " + "(منقضی شده)" : DateTimeExtensions.ConvertMiladiToShamsi(item.ProductGroup.First().ExpirationDate, "yyyy/MM/dd")
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

        public async Task<int> InsertProductGuarantee(string guaranteeName)
        {
            int id;
            var guarantee = await _context.Guarantees.FirstOrDefaultAsync(b => b.Name == guaranteeName);
            if (guarantee != null)
            {
                id = guarantee.Id;
                guarantee.Name = guaranteeName;
                guarantee.InsertTime = DateTime.Now;
                _context.Guarantees.Update(guarantee);
            }
            else
            {
                var guarantees = _context.Guarantees.Where(b => b.RemoveTime == null).ToList();
                if (guarantees.Count() == 0)
                    id = 1;
                else
                    id = guarantees.OrderByDescending(c => c.Id).First().Id + 1;
                await _context.Guarantees.AddAsync(new Guarantee { Name = guaranteeName, Id = id, InsertTime = DateTime.Now });
            }
            await _context.SaveChangesAsync();
            return id;
        }


        public List<string> GetBrands() => _context.Brands.Select(b => b.Name).ToList();
        public List<string> GetColors() => _context.Colors.Select(t => t.Name).ToList();
        public List<string> GetMaterials() => _context.Materials.Select(t => t.Name).ToList();
        public List<string> GetGuarantees() => _context.Guarantees.Select(g => g.Name).ToList();


        public int CountProductSale() => (int)_context.Products.Where(p => p.RemoveTime == null).Sum(p => p.NumberOfSale);
        public int CountProductPresentation() => _context.Products.Where(p => p.States == ProductState.Ready && p.Stock != 0 && p.InsertTime <= DateTime.Now).Count();
    }
}
