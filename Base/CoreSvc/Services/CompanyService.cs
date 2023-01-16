using CoreData.Repositories;
using CoreData.Validators;
using CoreType.DBModels;
using Microsoft.EntityFrameworkCore;

namespace CoreSvc.Services
{
    public class CompanyService : GenericService<CoreCompany, int, CoreCompanyValidator, CoreCompanyRepository>
    {
        public CompanyService(DbContext context = null) : base(context)
        {
        }
    }
}