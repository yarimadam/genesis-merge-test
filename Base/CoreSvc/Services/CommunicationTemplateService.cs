using System;
using CoreSvc.Services;
using CoreType.DBModels;
using CoreData.Repositories;
using CoreData.Validators;

namespace CoreSvc.Services
{
    public class CommunicationTemplateService : GenericService<CommunicationTemplate, int, CommunicationTemplateValidator, CommunicationTemplateRepository>
    {
    }
}