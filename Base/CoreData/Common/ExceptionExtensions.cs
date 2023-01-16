using System;
using System.Runtime.ExceptionServices;

namespace CoreData.Common
{
    public static class ExceptionExtensions
    {
        public static void ReThrow(this Exception ex)
        {
            ExceptionDispatchInfo.Capture(ex.InnerException ?? ex).Throw();
            //Never reaches this line but compiler needs a return. 
            throw ex;
        }
    }
}