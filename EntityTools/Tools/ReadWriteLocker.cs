using System;
using System.Threading;

namespace EntityTools.Tools
{
    public class RWLocker : IDisposable
    {
        /// <summary>
        /// Обертка над примитовом синхронизации ReaderWriterLockSlim
        /// https://habr.com/ru/post/459514/#ReaderWriterLockSlim
        /// </summary>
        public struct WriteLockToken : IDisposable
        {
            private readonly ReaderWriterLockSlim @lock;
            public WriteLockToken(ReaderWriterLockSlim @lock)
            {
                this.@lock = @lock;
                @lock.EnterWriteLock();
            }
            public void Dispose() => @lock.ExitWriteLock();
        }

        public struct ReadLockToken : IDisposable
        {
            private readonly ReaderWriterLockSlim @lock;
            public ReadLockToken(ReaderWriterLockSlim @lock)
            {
                this.@lock = @lock;
                @lock.EnterReadLock();
            }
            public void Dispose() => @lock.ExitReadLock();
        }

        private readonly ReaderWriterLockSlim @lock = new ReaderWriterLockSlim();

        public ReadLockToken ReadLock() => new ReadLockToken(@lock);
        public WriteLockToken WriteLock() => new WriteLockToken(@lock);

        public void Dispose() => @lock.Dispose();
    }
}
