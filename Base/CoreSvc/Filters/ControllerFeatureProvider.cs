using System.Linq;
using System.Reflection;
using Serilog;

namespace CoreSvc.Filters
{
    public class ControllerFeatureProvider : Microsoft.AspNetCore.Mvc.Controllers.ControllerFeatureProvider
    {
        private static readonly string AssemblyFullName = typeof(ControllerFeatureProvider).Assembly.FullName;

        protected override bool IsController(TypeInfo typeInfo)
        {
            var isController = base.IsController(typeInfo);

            if (isController)
            {
                if (typeInfo.Assembly.FullName == AssemblyFullName
                    && !Constants.LISTING_ALLOWED_CONTROLLER_METHODS.Contains(typeInfo))
                {
                    Log.Debug($"{typeInfo.Name} named controller is disabled. If you want to enable it, add its type to LISTING_ALLOWED_CONTROLLER_METHODS.");
                    return false;
                }
            }

            return isController;
        }
    }
}