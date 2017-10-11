using System.Data.Entity;
using Mehdime.Entity;
using Autoservice.DAL.Common.Context;

namespace Autoservice.DAL.Common.Implementation
{
    /// <summary>
    ///     Базовый репозиторий модуля DAL
    ///     Контекст не зашит в базовую реализацию репозитория,
    ///     потому что, во-первых, полезно бывает использовать в разные контексты в юнитах
    ///     Самый простой пример - это тестирование, также вариант не загружать все метаданные сущностей
    ///     Во-вторых, это позволяет наследовать наборы DbSet в виде интерфейсов, чтобы в конечном приложении
    ///     внедрить нужный (реализующий все интерфейсы) контекст с помощью фабрики IDbContextFactory
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    public class RepositoryBase<T, TContext> : EntityRepositoryBase<T, TContext>
        where T : class, IEntity
        where TContext : DbContext, IAutoServiceDBContext
    {
        public RepositoryBase(IAmbientDbContextLocator locator)
            : base(locator)
        {
        }

        protected virtual IAutoServiceDBContext Context => DbContext;
    }
}
