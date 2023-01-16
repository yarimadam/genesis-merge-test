using System;
using Microsoft.AspNetCore.Mvc.Routing;

namespace CoreSvc.Filters
{
    public class DefaultRouteAttribute : Attribute, IRouteTemplateProvider
    {
        public string Template => "[controller]/[action]";
        public int? Order => 2;
        public string Name { get; set; }
    }
}