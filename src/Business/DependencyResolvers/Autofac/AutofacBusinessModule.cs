using Autofac;
using Autofac.Extras.DynamicProxy;
using Business.Abstract;
using Business.Concrete;
using Business.Utilities.Security.Jwt;
using Castle.DynamicProxy;
using Core.CrossCuttingConcerns.Logging.Log4Db.Abstract;
using Core.Utilities.Interceptors;
using Core.Utilities.IoC;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Microsoft.Extensions.DependencyInjection;

namespace Business.DependencyResolvers.Autofac
{
    public class AutofacBusinessModule : Module, ICoreModule
    {
        public void Load(IServiceCollection services)
        {
            services.AddSingleton(typeof(IUserService), typeof(UserManager));
            services.AddSingleton(typeof(IUserDal), typeof(EfUserDal));

            services.AddSingleton(typeof(ITokenHelper), typeof(JwtHelper));
            services.AddSingleton(typeof(IAuthService), typeof(AuthManager));

            services.AddSingleton(typeof(ISystemLogDal), typeof(EfSystemLogDal));
            services.AddSingleton(typeof(ILog4DbService), typeof(Log4DbManager));

            services.AddSingleton(typeof(IOperationClaimService), typeof(OperationClaimManager));
            services.AddSingleton(typeof(IOperationClaimDal), typeof(EfOperationClaimDal));

            services.AddSingleton(typeof(IUserOperationClaimService), typeof(UserOperationClaimManager));
            services.AddSingleton(typeof(IUserOperationClaimDal), typeof(EfUserOperationClaimDal));

            services.AddSingleton(typeof(IProductService), typeof(ProductManager));
            services.AddSingleton(typeof(IProductDal), typeof(EfProductDal));
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AuthManager>().As<IAuthService>();
            builder.RegisterType<JwtHelper>().As<ITokenHelper>();

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces()
                .EnableInterfaceInterceptors(new ProxyGenerationOptions()
                {
                    Selector = new AspectInterceptorSelector()
                })
                .SingleInstance();

        }
    }
}
