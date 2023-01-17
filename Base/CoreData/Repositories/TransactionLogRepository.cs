using System;
using CoreType.DBModels;
using CoreData.Common;
using CoreData.DBContexts;
using CoreData.Validators;

namespace CoreData.Repositories
{
    public class TransactionLogRepository : GenericGenesisRepository<TransactionLog, int, TransactionLogValidator>
    {
        public TransactionLogRepository()
        {
        }

        public TransactionLogRepository(GenesisContextBase context) : base(context)
        {
        }
    }
}