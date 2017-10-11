using System;
using NLog;
using Autoservice.DAL.Common.Implementation;
using Autoservice.DAL.Common.Interfaces;

namespace Autoservice.DAL.Services
{
    /// <summary>
    /// Base absract class of application service
    /// </summary>
    public abstract class ServiceBase : Disposable
    {
        protected Logger Log;

        protected ServiceBase(IDbWorker dbWorker)
        {
            Db = dbWorker;
            Log = LogManager.GetLogger(GetType().FullName);
        }

        public IDbWorker Db { get; private set; }

        protected virtual void DoWork(Action action)
        {
            try
            {
                using (var scope = Db.BeginWork())
                {
                    action();
                    scope.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Operation failed");
            }
        }

        protected virtual void DoReadOnlyWork(Action action)
        {
            try
            {
                using (Db.BeginReadOnlyWork())
                {
                    action();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Operation failed");
            }
        }
    }
}
