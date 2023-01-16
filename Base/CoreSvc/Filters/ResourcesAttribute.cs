using System.Collections.Generic;
using System.Linq;
using CoreSvc.Common;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CoreSvc.Filters
{
    public class ResourcesAttribute : ActionFilterAttribute
    {
        private readonly IList<string> _resourceCodes;

        public ResourcesAttribute(string resourceCode, params string[] resourceCodes)
        {
            _resourceCodes = new[] { resourceCode }.Concat(resourceCodes).ToList();
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.Controller is BaseController controller)
            {
                controller.ResourceCodes = _resourceCodes;
            }

            base.OnActionExecuting(context);
        }
    }
}