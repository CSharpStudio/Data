using System;
using System.Collections.Generic;
using System.Text;

namespace Css.ComponentModel
{
    /// <summary>
    /// Disposable Pattern.
    /// </summary>
    public class DisposableBase : IDisposable
    {
        bool disposed;

        protected bool Disposed
        {
            get
            {
                lock (this)
                {
                    return disposed;
                }
            }
        }

        #region IDisposable Members
        
        public void Dispose()
        {
            lock (this)
            {
                if (disposed == false)
                {
                    Cleanup();
                    disposed = true;

                    GC.SuppressFinalize(this);
                }
            }
        }

        #endregion

        protected virtual void Cleanup()
        {
            // override to provide cleanup
        }
        
        ~DisposableBase()
        {
            Cleanup();
        }
    }

    public class Disposable : DisposableBase
    {
        public static readonly Disposable Empty = new Disposable();

        public static IDisposable Create(Action cleanup)
        {
            return new Disposable { _cleanup = cleanup };
        }
        Action _cleanup;

        protected override void Cleanup()
        {
            base.Cleanup();
            _cleanup?.Invoke();
        }
    }
}
