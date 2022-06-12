using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using ShoppingStore.Application.Contracts;

namespace ShoppingStore.Application.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly IDataBaseContext _context;
        private readonly IProductService _productService;
        public UnitOfWork(IMapper mapper, IWebHostEnvironment env, IDataBaseContext context, IProductService productService)
        {
            _env = env;
            _mapper = mapper;
            _context = context;
            _productService = productService;
        }

        public IBaseService<TEntity> BaseService<TEntity>() where TEntity : class
        {
            IBaseService<TEntity> repository = new BaseService<TEntity>(_context);
            return repository;
        }

        private IAdminBrands _getAdminBrands;
        public IAdminBrands AdminBrands
        {
            get
            {
                return _getAdminBrands = _getAdminBrands ?? new AdminBrands(_context);
            }
        }

        private IAdminCarts _getAdminCarts;
        public IAdminCarts AdminCarts
        {
            get
            {
                return _getAdminCarts = _getAdminCarts ?? new AdminCarts(_context);
            }
        }

        private ISiteCarts _getSiteCarts;
        public ISiteCarts SiteCarts
        {
            get
            {
                return _getSiteCarts = _getSiteCarts ?? new SiteCarts(_context);
            }
        }

        private IManageShoppingCart _getManageShoppingCart;
        public IManageShoppingCart ManageShoppingCart
        {
            get
            {
                return _getManageShoppingCart = _getManageShoppingCart ?? new ManageShoppingCart(_context);
            }
        }

        private IAdminCategories _getAdminCategories;
        public IAdminCategories AdminCategories
        {
            get
            {
                return _getAdminCategories = _getAdminCategories ?? new AdminCategories(_context);
            }
        }

        private ISiteCategories _getSiteCategories;
        public ISiteCategories SiteCategories
        {
            get
            {
                return _getSiteCategories = _getSiteCategories ?? new SiteCategories(_context, _getAdminCategories);
            }
        }

        private IAdminCities _getAdminCities;
        public IAdminCities AdminCities
        {
            get
            {
                return _getAdminCities = _getAdminCities ?? new AdminCities(_context);
            }
        }

        private IAdminColors _getAdminColors;
        public IAdminColors AdminColors
        {
            get
            {
                return _getAdminColors = _getAdminColors ?? new AdminColors(_context);
            }
        }

        private IAdminComments _getAdminComments;
        public IAdminComments AdminComments
        {
            get
            {
                return _getAdminComments = _getAdminComments ?? new AdminComments(_context);
            }
        }

        private ISiteComments _getSiteComments;
        public ISiteComments SiteComments
        {
            get
            {
                return _getSiteComments = _getSiteComments ?? new SiteComments(_context);
            }
        }

        private IInsertCommentUser _getInsertCommentUser;
        public IInsertCommentUser InsertCommentUser
        {
            get
            {
                return _getInsertCommentUser = _getInsertCommentUser ?? new InsertCommentUser(_mapper, _context);
            }
        }

        private IAdminCreditCarts _getAdminCreditCarts;
        public IAdminCreditCarts AdminCreditCarts
        {
            get
            {
                return _getAdminCreditCarts = _getAdminCreditCarts ?? new AdminCreditCarts(_context);
            }
        }

        private IManageCreditCartUser _getManageCreditCartUser;
        public IManageCreditCartUser ManageCreditCartUser
        {
            get
            {
                return _getManageCreditCartUser = _getManageCreditCartUser ?? new ManageCreditCartUser(_context);
            }
        }

        private IAdminFiles _getAdminFiles;
        public IAdminFiles AdminFiles
        {
            get
            {
                return _getAdminFiles = _getAdminFiles ?? new AdminFiles(_context);
            }
        }

        private IInsertLikeOrDislike _getInsertLikeOrDislike;
        public IInsertLikeOrDislike InsertLikeOrDislike
        {
            get
            {
                return _getInsertLikeOrDislike = _getInsertLikeOrDislike ?? new InsertLikeOrDislike(_context);
            }
        }

        private IAdminMaterials _getAdminMaterials;
        public IAdminMaterials AdminMaterials
        {
            get
            {
                return _getAdminMaterials = _getAdminMaterials ?? new AdminMaterials(_context);
            }
        }

        private IAdminMessageUsers _getAdminMessageUsers;
        public IAdminMessageUsers AdminMessageUsers
        {
            get
            {
                return _getAdminMessageUsers = _getAdminMessageUsers ?? new AdminMessageUsers(_context);
            }
        }

        private IInsertMassageOfUser _getInsertMassageOfUser;
        public IInsertMassageOfUser InsertMassageOfUser
        {
            get
            {
                return _getInsertMassageOfUser = _getInsertMassageOfUser ?? new InsertMassageOfUser(_context);
            }
        }

        private IAdminNewsletters _getAdminNewsletters;
        public IAdminNewsletters AdminNewsletters
        {
            get
            {
                return _getAdminNewsletters = _getAdminNewsletters ?? new AdminNewsletters(_context);
            }
        }

        private IInsertNewsletter _getInsertNewsletter;
        public IInsertNewsletter InsertNewsletter
        {
            get
            {
                return _getInsertNewsletter = _getInsertNewsletter ?? new InsertNewsletter(_context);
            }
        }

        private IAdminOrders _getAdminOrders;
        public IAdminOrders AdminOrders
        {
            get
            {
                return _getAdminOrders = _getAdminOrders ?? new AdminOrders(_context);
            }
        }

        private ISiteOrders _getSiteOrders;
        public ISiteOrders SiteOrders
        {
            get
            {
                return _getSiteOrders = _getSiteOrders ?? new SiteOrders(_context);
            }
        }

        private IAdminPayments _getAdminPayments;
        public IAdminPayments AdminPayments
        {
            get
            {
                return _getAdminPayments = _getAdminPayments ?? new AdminPayments(_context);
            }
        }
        private ISitePayments _getSitePayments;
        public ISitePayments SitePayments
        {
            get
            {
                return _getSitePayments = _getSitePayments ?? new SitePayments(_context);
            }
        }

        private IAdminProducts _getAdminProducts;
        public IAdminProducts AdminProducts
        {
            get
            {
                return _getAdminProducts = _getAdminProducts ?? new AdminProducts(_context, _productService);
            }
        }

        private ISiteProducts _getSiteProducts;
        public ISiteProducts SiteProducts
        {
            get
            {
                return _getSiteProducts = _getSiteProducts ?? new SiteProducts(_context, _productService);
            }
        }

        private IAdminProvinces _getAdminProvinces;
        public IAdminProvinces AdminProvinces
        {
            get
            {
                return _getAdminProvinces = _getAdminProvinces ?? new AdminProvinces(_context);
            }
        }

        private IInsertRateSaller _getInsertRateSaller;
        public IInsertRateSaller InsertRateSaller
        {
            get
            {
                return _getInsertRateSaller = _getInsertRateSaller ?? new InsertRateSaller(_context);
            }
        }

        private ISiteSellers _getSiteSellers;
        public ISiteSellers SiteSellers
        {
            get
            {
                return _getSiteSellers = _getSiteSellers ?? new SiteSellers(_context);
            }
        }

        private IAdminSellers _getAdminSellers;
        public IAdminSellers AdminSellers
        {
            get
            {
                return _getAdminSellers = _getAdminSellers ?? new AdminSellers(_context);
            }
        }

        private IInsertRequestSeller _getInsertRequestSeller;
        public IInsertRequestSeller InsertRequestSeller
        {
            get
            {
                return _getInsertRequestSeller = _getInsertRequestSeller ?? new InsertRequestSeller(_env, _context);
            }
        }

        private IAdminSliders _getAdminSliders;
        public IAdminSliders AdminSliders
        {
            get
            {
                return _getAdminSliders = _getAdminSliders ?? new AdminSliders(_context);
            }
        }

        private ISiteSliders _getSiteSliders;
        public ISiteSliders SiteSliders
        {
            get
            {
                return _getSiteSliders = _getSiteSliders ?? new SiteSliders(_context);
            }
        }

        private IAdminUsers _getAdminUsers;
        public IAdminUsers AdminUsers
        {
            get
            {
                return _getAdminUsers = _getAdminUsers ?? new AdminUsers(_context);
            }
        }
    }
}
