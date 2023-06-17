using Castle.DynamicProxy;
using Core.CrossCuttingConcerns.Validation;
using Core.Utilities.Interceptors;
using Core.Utilities.Messages;
using Core.Utilities.Results;
using FluentValidation;
using System;
using System.Linq;

namespace Core.Aspects.Autofac.Validation
{
    public class ValidationAspect:MethodInterception
    {
        private Type _validatorType;
        public ValidationAspect(Type validatorType)
        {
            if (!typeof(IValidator).IsAssignableFrom(validatorType))
            {
                throw new System.Exception(AspectMessages.WrongValidationType);
            }

            _validatorType = validatorType;
        }

        public override void Intercept(IInvocation invocation)
        {
            try
            {
                var validator = (IValidator)Activator.CreateInstance(_validatorType);
                var entityType = _validatorType.BaseType.GetGenericArguments()[0];
                var entities = invocation.Arguments
                    .Where(t => t.GetType() == entityType)
                    .Select(x => new ValidationContext<object>(x))
                    .ToList();

                foreach (ValidationContext<object> entity in entities)
                {
                    ValidationTool<object>.Validate(validator, entity);
                }

                invocation.Proceed();
            }
            catch (System.Exception ex)
            {
                if (AllowThrow)
                    throw;

                invocation.ReturnValue = new ErrorResult(message: ex.Message);
            }
        }

    }
}
