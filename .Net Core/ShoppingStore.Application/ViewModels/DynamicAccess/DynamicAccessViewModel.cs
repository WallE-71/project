using System.Collections.Generic;
using ShoppingStore.Domain.Entities.Identity;

namespace ShoppingStore.Application.ViewModels.DynamicAccess
{
    public class DynamicAccessIndexViewModel
    {
        //public DynamicAccessIndexViewModel(User userIncludeUserClaims, ICollection<ControllerViewModel> securedControllerActions)
        //{
        //    UserIncludeUserClaims = userIncludeUserClaims;
        //    SecuredControllerActions = securedControllerActions;
        //}

        public string ActionIds { set; get; }
        public int UserId { set; get; }

        public User UserIncludeUserClaims { set; get; }
        public ICollection<ControllerViewModel> SecuredControllerActions { set; get; }
    }
}
