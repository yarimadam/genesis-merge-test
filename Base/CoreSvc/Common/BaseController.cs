using System.Collections.Generic;
using CoreData.Common;
using CoreType.Types;
using Microsoft.AspNetCore.Mvc;

namespace CoreSvc.Common
{
    public abstract class BaseController : Controller
    {
        public IList<string> ResourceCodes;

        public SessionContext Session => SessionAccessor.GetSession();
    }
}