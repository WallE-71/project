using System.Collections.Generic;
using ShoppingStore.Application.ViewModels.DynamicAccess;

namespace ShoppingStore.Infrastructure.Contracts
{
    public interface IMvcActionsDiscoveryService
    {
        ICollection<ControllerViewModel> GetAllSecuredControllerActionsWithPolicy(string policyName);
    }
}
