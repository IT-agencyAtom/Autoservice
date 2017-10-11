using Mehdime.Entity;
using Autoservice.DAL.Common.Interfaces;

namespace Autoservice.DAL.Common.Implementation
{
    /// <summary>
    /// Стандартная реализация работы с данными
    /// </summary>
    public class DbWorker : Disposable, IDbWorker
    {
        public DbWorker(IDbContextScopeFactory dbContextScopeFactory)
        {
            _dbContextScopeFactory = dbContextScopeFactory;
        }

        private readonly IDbContextScopeFactory _dbContextScopeFactory;

        /// <summary>
        /// Начать работу с базой данных, т.е. создать execution scope
        /// в режиме read/write
        /// 
        /// Типовое применение:
        /// 
        /// using(var scope = BeginWork())
        /// {
        ///     // Some work in repositories
        ///     scope.SaveChanges();
        /// }
        /// </summary>
        /// <returns></returns>
        public IDbContextScope BeginWork()
        {
            return _dbContextScopeFactory.Create();
        }

        /// <summary>
        /// Начать работу с базой данных, т.е. создать execution scope
        /// в режиме read only
        /// 
        /// Типовое применение:
        /// 
        /// using(BeginReadOnlyWork())
        /// {
        ///     // Some work in repositories without write operations
        ///     // e.g. var users = userRepository.GetAll();
        /// }
        /// </summary>
        /// <returns></returns>
        public IDbContextReadOnlyScope BeginReadOnlyWork()
        {
            return _dbContextScopeFactory.CreateReadOnly();
        }
    }
}