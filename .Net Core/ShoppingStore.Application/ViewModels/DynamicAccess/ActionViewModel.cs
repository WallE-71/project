using System;
using System.Collections.Generic;

namespace ShoppingStore.Application.ViewModels.DynamicAccess
{
    public class ActionViewModel
    {
        public string ActionDisplayName { get; set; }
        public string ActionId => $"{ControllerId}:{ActionName}";
        public string ActionName { get; set; }
        public string ControllerId { get; set; }
        public bool IsSecuredAction { get; set; }

        public IList<Attribute> ActionAttributes { get; set; }
    }
}
