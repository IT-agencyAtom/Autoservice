using System.Data.Entity;
using System.Reflection;
using Autofac;
using Autofac.Extras.CommonServiceLocator;
using Mehdime.Entity;
using Microsoft.Practices.ServiceLocation;
using Autoservice.DAL.Common.Context;
using Autoservice.DAL.Common.Implementation;
using Autoservice.DAL.Common.Interfaces;
using Autoservice.DAL.Repositories;
using Autoservice.DAL.Repositories.Interfaces;
using Autoservice.DAL.Services;

namespace Autoservice.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        ///     Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            var builder = new ContainerBuilder();
            ConfigureContainer(builder);

            var autofacLocator = new AutofacServiceLocator(builder.Build());

            ServiceLocator.SetLocatorProvider(() => autofacLocator);
        }

        public MainManager Main => ServiceLocator.Current.GetInstance<MainManager>();

        protected void ConfigureContainer(ContainerBuilder builder)
        {
            Assembly assembly = typeof(App).Assembly;
            builder.RegisterAssemblyTypes(assembly)
                .Where(item => item.Name.EndsWith("Manager") && item.IsAbstract == false)
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<DbContextScopeFactory>().As<IDbContextScopeFactory>().SingleInstance();
            builder.RegisterType<AmbientDbContextLocator>().As<IAmbientDbContextLocator>().SingleInstance();
            builder.RegisterType<DbWorker>().As<IDbWorker>().SingleInstance();
#if DEBUG
            builder.RegisterType<LocalDbContextFactory>().As<IDbContextFactory>().SingleInstance();
#else
            builder.RegisterType<ProductionDbContextFactory>().As<IDbContextFactory>().SingleInstance();
#endif
            // Repository registration
            RegisterRepositories<AutoServiceDBContext>(builder);

            // Service registration
            builder.RegisterAssemblyTypes(typeof(GeneralService).Assembly)
                .Where(t => t.Name.EndsWith("Service")).AsImplementedInterfaces().InstancePerDependency();
        }

        public void RegisterRepositories<TContext>(ContainerBuilder builder)
                where TContext : DbContext, IAutoServiceDBContext
        {
            builder.RegisterType<ClientRepository<TContext>>().As<IClientRepository>().InstancePerDependency();
            builder.RegisterType<ActivityRepository<TContext>>().As<IActivityRepository>().InstancePerDependency();
            builder.RegisterType<CarRepository<TContext>>().As<ICarRepository>().InstancePerDependency();
            builder.RegisterType<MasterRepository<TContext>>().As<IMasterRepository>().InstancePerDependency();
            builder.RegisterType<OrderRepository<TContext>>().As<IOrderRepository>().InstancePerDependency();
            builder.RegisterType<SparePartRepository<TContext>>().As<ISparePartRepository>().InstancePerDependency();
            builder.RegisterType<WorkRepository<TContext>>().As<IWorkRepository>().InstancePerDependency();
            builder.RegisterType<UserRepository<TContext>>().As<IUserRepository>().InstancePerDependency();
            builder.RegisterType<OrderWorkRepository<TContext>>().As<IOrderWorkRepository>().InstancePerDependency();
        }

        public static void Cleanup()
        {
            // Clear the ViewModels
        }
    }
}