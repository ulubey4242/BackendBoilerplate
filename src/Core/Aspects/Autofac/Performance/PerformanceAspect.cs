using Castle.DynamicProxy;
using Core.CrossCuttingConcerns.Logging.Log4Net;
using Core.Utilities.Interceptors;
using Core.Utilities.IoC;
using Core.Utilities.Messages;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;

namespace Core.Aspects.Autofac.Performance
{
    public class PerformanceAspect:MethodInterception
    {
        private int _interval;
        private Stopwatch _stopwatch;
        private LoggerServiceBase _loggerServiceBase;

        public PerformanceAspect(int interval=5, Type logger=null)
        {
            if (logger != null)
            {
                if (logger.BaseType != typeof(LoggerServiceBase))
                    throw new System.Exception(AspectMessages.WrongLoggerType);

                _loggerServiceBase = (LoggerServiceBase)Activator.CreateInstance(logger);
            }

            _interval = interval;
            _stopwatch = ServiceTool.ServiceProvider.GetService<Stopwatch>();
        }

        protected override void OnBefore(IInvocation invocation)
        {
            _stopwatch.Start();
        }

        protected override void OnAfter(IInvocation invocation)
        {
            if (_stopwatch.Elapsed.TotalSeconds>_interval)
            {
                var logMessage = $"Performance : {invocation.Method.DeclaringType.FullName}.{invocation.Method.Name}-->{_stopwatch.Elapsed.TotalSeconds}";

                Debug.WriteLine(logMessage);

                if (_loggerServiceBase != null)
                    _loggerServiceBase.Warn(logMessage);
            }

            _stopwatch.Reset();
        }
    }
}
