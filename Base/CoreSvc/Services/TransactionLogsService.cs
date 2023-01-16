using CoreData.Repositories;
using CoreType.DBModels;

namespace CoreSvc.Services
{
    public class TransactionLogsService : GenericService<TransactionLogs, int, TransactionLogsRepository>
    {
    }
}