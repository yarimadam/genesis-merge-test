using CoreData.Repositories;
using CoreType.DBModels;
using CoreType.Types;

// ReSharper disable UnusedMember.Global

namespace CoreData.Common
{
    public class UIHelperMethods
    {
        private static readonly UserRepository _userRepository = new UserRepository();
        private static readonly ParameterRepository _parameterRepo = new ParameterRepository();
        private static readonly TenantRepository _tenantRepository = new TenantRepository();
        private static readonly CoreCompanyRepository _companyRepository = new CoreCompanyRepository();

        private readonly SessionContext Session;

        public UIHelperMethods(SessionContext session)
        {
            Session = session;
        }

        public CoreUsers GetUser(int userId)
        {
            return _userRepository.GetById(userId);
        }

        public string GetParameter(string keyCode, string lang)
        {
            return _parameterRepo.GetLocalizedParam(keyCode, lang);
        }

        public string GetParameter(string keyCode, string value, string lang)
        {
            return _parameterRepo.GetLocalizedParam(keyCode, value, lang);
        }

        public CoreCompany GetCompany(int companyId)
        {
            return _companyRepository.GetById(companyId);
        }

        public Tenant GetTenant(int tenantId)
        {
            return _tenantRepository.GetById(tenantId);
        }
    }
}