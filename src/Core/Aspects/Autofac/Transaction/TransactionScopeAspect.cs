using Castle.DynamicProxy;
using Core.Utilities.Interceptors;
using Core.Utilities.Results;
using System.Transactions;

namespace Core.Aspects.Autofac.Transaction
{
    public class TransactionScopeAspect : MethodInterception
    {
        public override void Intercept(IInvocation invocation)
        {
            using (TransactionScope transactionScope = new TransactionScope())
            {
                try
                {
                    invocation.Proceed();
                    transactionScope.Complete();
                }
                catch (System.Exception ex)
                {
                    transactionScope.Dispose();

                    invocation.ReturnValue = new ErrorResult(message: $"{ex.Message} - {ex.InnerException?.Message ?? ""}");
                }
            }
        }
    }
}
