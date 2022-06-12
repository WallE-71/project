using System.Linq;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using ShoppingStore.Infrastructure.Contracts;
using ShoppingStore.Application.ViewModels.DynamicAccess;

namespace ShoppingStore.Infrastructure
{
    public class SecurityTrimmingService : ISecurityTrimmingService
    {
        private readonly IHttpContextAccessor _http;
        private readonly IMvcActionsDiscoveryService _mvcActions;
        public SecurityTrimmingService(IHttpContextAccessor httpContextAccessor, IMvcActionsDiscoveryService mvcActionsDiscoveryService)
        {
            _http = httpContextAccessor;
            _mvcActions = mvcActionsDiscoveryService;
        }

        public bool CanCurrentUserAccess(string area, string controller, string action)
        {
            return _http.HttpContext != null && CanUserAccess(_http.HttpContext.User, area, controller, action);
        }

        public bool CanUserAccess(ClaimsPrincipal user, string area, string controller, string action)
        {
            var currentClaimValue = $"{area}:{controller}:{action}";
            var securedControllerActions = _mvcActions.GetAllSecuredControllerActionsWithPolicy(ConstantPolicies.DynamicPermission);

            if (!securedControllerActions.SelectMany(x => x.MvcActions).Any(x => x.ActionId == currentClaimValue))
                throw new KeyNotFoundException($@"The `secured` area={area}/controller={controller}/action={action} with `ConstantPolicies.DynamicPermission` policy not found. Please check you have entered the area/controller/action names correctly and also it's decorated with the correct security policy.");

            if (!user.Identity.IsAuthenticated)
                return false;

            return user.HasClaim(claim => claim.Type == ConstantPolicies.DynamicPermissionClaimType &&
                                          claim.Value == currentClaimValue);
        }
    }
}
