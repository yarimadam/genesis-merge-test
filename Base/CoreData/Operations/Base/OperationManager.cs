using System;
using CoreData.Infrastructure;

namespace CoreData.Operations
{
    public class OperationManager
    {
        private readonly UnitOfWork _unitOfWork;

        public OperationManager(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public T Get<T>() where T : IOperationBase
        {
            return (T) Activator.CreateInstance(typeof(T), _unitOfWork);
        }
    }
}