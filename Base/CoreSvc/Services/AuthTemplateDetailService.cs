using System;
using CoreSvc.Services;
using CoreType.DBModels;
using CoreData.Repositories;
using CoreData.Validators;

namespace CoreSvc.Services
{
    public class AuthTemplateDetailService : GenericService<AuthTemplateDetail, int, AuthTemplateDetailValidator, AuthTemplateDetailRepository>
    {
    }
}