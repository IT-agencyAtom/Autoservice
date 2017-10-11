using System;
using System.Threading;

namespace Autoservice.DAL.Common.Implementation
{
    /// <summary>
    /// Base class for disposable objects.
    /// (based in Autofac.Util.Disposable)
    /// </summary>
    public class Disposable : IDisposable
    {
        private const int DisposedFlag = 1;
        private int _isDisposed;

        /// <summary>
        /// Returns true if the current instance has been disposed; otherwise false;
        /// 
        /// </summary>
        protected bool IsDisposed
        {
            get
            {
                Thread.MemoryBarrier();
                return _isDisposed == DisposedFlag;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// 
        /// </summary>
        //[SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "Dispose is implemented correctly, FxCop just doesn't see it.")]
        public void Dispose()
        {
            if (Interlocked.Exchange(ref _isDisposed, 1) == 1)
                return;
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// 
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
        }
    }
}