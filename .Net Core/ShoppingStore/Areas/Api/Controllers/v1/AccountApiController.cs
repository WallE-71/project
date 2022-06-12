using System;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using ShoppingStore.Domain.Common.Api;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Infrastructure.Contract;
using ShoppingStore.Domain.Entities.Identity;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Infrastructure.Contracts;
using ShoppingStore.Domain.Common.Api.Attributes;
using ShoppingStore.Application.ViewModels.Api.SignIn;

namespace ShoppingStore.Web.Areas.Api.Controllers.v1
{
    [DisplayName("AccountApi"), ApiResultFilter, ApiVersion("1"), Route("api/v{version:apiVersion}/[controller]")]
    public class AccountApiController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uw;
        private readonly IJwtService _jwtService;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _accessor;
        private readonly ISignInOption<User> _signInOption;
        private readonly IApplicationUserManager _userManager;
        private readonly IApplicationRoleManager _roleManager;
        private readonly ILogger<AccountApiController> _logger;
        public AccountApiController(
            IMapper mapper,
            IUnitOfWork uw,
            IJwtService jwtService,
            IEmailSender emailSender,
            IHttpContextAccessor accessor,
            ISignInOption<User> signInOption,
            IApplicationUserManager userManager,
            IApplicationRoleManager roleManager,
            ILogger<AccountApiController> logger)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
            _mapper = mapper;
            _mapper.CheckArgumentIsNull(nameof(_mapper));
            _logger = logger;
            _logger.CheckArgumentIsNull(nameof(_logger));
            _accessor = accessor;
            _accessor.CheckArgumentIsNull(nameof(_accessor));
            _jwtService = jwtService;
            _jwtService.CheckArgumentIsNull(nameof(_jwtService));
            _userManager = userManager;
            _userManager.CheckArgumentIsNull(nameof(_userManager));
            _roleManager = roleManager;
            _roleManager.CheckArgumentIsNull(nameof(_roleManager));
            _emailSender = emailSender;
            _emailSender.CheckArgumentIsNull(nameof(_emailSender));
            _signInOption = signInOption;
            _signInOption.CheckArgumentIsNull(nameof(_signInOption));
        }

        [HttpPost("SendCode")]
        public async Task<ApiResult<string>> SendCode(string email)
        {
            if (!email.HasValue() && !email.IsValidEmail())
                return BadRequest();

            var code = StringExtensions.GenerateId(5);
            var Message = "<p style='direction:rtl; font-size:14px; font-family:tahoma'>کد اعتبارسنجی شما :" + code + "</p>";
            await _emailSender.SendEmailAsync(email, "کد اعتبارسنجی", Message);// need: google -> Allow less secure apps: ON    // link => https://myaccount.google.com/lesssecureapps?pli=1&rapt=AEjHL4Omcm86-NTHT7zOzqXU4d5yoQk4FYQLORKtApUfpDvz4ybLOtJz8nthJeMNi8FwDIcZv6AB_QPuZN8E4V4viZDE-Z-OdA

            return Ok(code);
        }

        [HttpPost]
        public async Task<ApiResult<UserDto>> RegisterOrSignIn(string phoneNumber, string browserId)
        {
            if (!phoneNumber.HasValue() && !phoneNumber.IsValidPhoneNumber())
                return Ok();

            var guid = new Guid();
            if (browserId == "null")
            {
                var value = Guid.NewGuid().ToString();
                Guid.TryParse(value, out guid);
            }

            var findCustomer = await _uw.BaseService<User>().FindByConditionAsync(u => u.PhoneNumber == phoneNumber);
            if (findCustomer != null) // SignIn, Login
            {
                if (findCustomer.FirstOrDefault().IsActive)
                {
                    var result = await _signInOption.PhoneNumberSignInAsync(findCustomer.FirstOrDefault(), phoneNumber, true);
                    if (result.Succeeded)
                    {
                        var token = _jwtService.GenerateTokenAsync(findCustomer.FirstOrDefault());
                        if (token != null)
                            return new UserDto { token = token, browserId = browserId != "null" ? browserId : guid.ToString() };
                        else return null;
                    }
                    else if (result.IsLockedOut)
                        return Ok("حساب کاربری شما به مدت 20 دقیقه به دلیل تلاش های ناموفق قفل شد.");
                    else
                    {
                        _logger.LogWarning($"The user attempts to login with the IP address({_accessor.HttpContext?.Connection?.RemoteIpAddress.ToString()}) and phoneNumber ({phoneNumber}).");
                        return Ok("شماره همراه شما صحیح نمی باشد.");
                    }
                }
                else
                    return Ok("حساب کاربری شما غیرفعال است.");
            }
            else // SignUp, Register
            {
                var customer = new User { PhoneNumber = phoneNumber, UserName = phoneNumber, Email = phoneNumber + "@snapmarket.ir", FirstName = "", LastName = "", Address = "", InsertTime = DateTime.Now, IsActive = true };
                IdentityResult result = await _userManager.CreateAsync(customer, phoneNumber);
                if (result.Succeeded)
                {
                    var role = await _roleManager.FindByNameAsync("مشتری");
                    if (role == null)
                        await _roleManager.CreateAsync(new Role("مشتری"));

                    result = await _userManager.AddToRoleAsync(customer, "مشتری");
                    if (result.Succeeded)
                    {
                        customer.EmailConfirmed = true;
                        customer.LockoutEnabled = false;
                        customer.TwoFactorEnabled = true;
                        customer.PhoneNumberConfirmed = true;
                        await _userManager.UpdateAsync(customer);
                        var token = _jwtService.GenerateTokenAsync(await _userManager.FindByIdAsync(customer.Id.ToString()));
                        if (token != null)
                            return new UserDto { token = token, browserId = guid.ToString() };
                    }
                }
                foreach (var item in result.Errors)
                    return Ok(item.Description);
            }
            return Ok();
        }

        [HttpPut]
        public async Task<ApiResult<object>> EditInfo(string phoneNumber, string email, string firstName, string lastName, string address)
        {
            IdentityResult result = null;
            var users = await _uw.BaseService<User>().FindByConditionAsync(u => u.PhoneNumber == phoneNumber);
            if (users.Count() != 0)
            {
                users.FirstOrDefault().Email = email;
                users.FirstOrDefault().Address = address;
                users.FirstOrDefault().LastName = lastName;
                users.FirstOrDefault().FirstName = firstName;
                users.FirstOrDefault().PhoneNumber = phoneNumber;
                result = await _userManager.UpdateAsync(users.FirstOrDefault());
            }
            else
                return Ok();

            if (result.Succeeded)
            {
                var newToken = _jwtService.GenerateTokenAsync(users.FirstOrDefault());
                if (newToken != null)
                    return new UserDto
                    {
                        token = newToken,
                        browserId = null
                    };
            }
            return Ok();
        }

        [HttpPost("CreditCart")]
        public async Task<object> CreditCart(string phoneNumber, int credit, string nationalId, string bankCode, bool getCart)
        {
            return Ok(await _uw.ManageCreditCartUser.CreditCartAsync(phoneNumber, credit, nationalId, bankCode, getCart));
        }
    }
}
