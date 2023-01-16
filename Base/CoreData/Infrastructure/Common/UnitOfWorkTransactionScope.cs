using System;
using System.Threading;
using System.Threading.Tasks;
using CoreData.Common;
using CoreData.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CoreData.Infrastructure
{
    public delegate void TransactionEventHandler(object sender, EventArgs e);

    public class UnitOfWorkTransactionScope : IDisposable, IAsyncDisposable
    {
        private readonly DbContext _context;
        private IDbContextTransaction _transaction;
        public ErrorBehaviour Behaviour;
        public event TransactionEventHandler Committed;
        public event TransactionEventHandler RolledBack;

        public UnitOfWorkTransactionScope(DbContext context)
        {
            _context = context;
        }

        public UnitOfWorkTransactionScope BeginTransaction(ErrorBehaviour behaviour = ErrorBehaviour.Rollback)
        {
            Behaviour = behaviour;
            _transaction = _context.Database.BeginTransaction();
            return this;
        }

        public async Task<UnitOfWorkTransactionScope> BeginTransactionAsync(ErrorBehaviour behaviour = ErrorBehaviour.Rollback)
        {
            Behaviour = behaviour;
            _transaction = await _context.Database.BeginTransactionAsync();
            return this;
        }

        public void Commit()
        {
            CommitAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task CommitAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                try
                {
                    await _context.SaveChangesAsync(cancellationToken);
                }
                catch (Exception e)
                {
                    if (Behaviour != ErrorBehaviour.CommitAnyway)
                        e.ReThrow();
                }

                await _transaction.CommitAsync(cancellationToken);
            }
            catch (Exception e)
            {
                if (Behaviour != ErrorBehaviour.DoNothing)
                    await RollbackAsync(cancellationToken);
                e.ReThrow();
            }
            finally
            {
                _context.DetachAllEntities();
                OnCommit(EventArgs.Empty);
            }
        }

        protected virtual void OnCommit(EventArgs e)
        {
            Committed?.Invoke(this, e);
        }

        public void Rollback()
        {
            RollbackAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                await _transaction.RollbackAsync(cancellationToken);
            }
            catch (Exception e)
            {
                e.ReThrow();
            }
            finally
            {
                _context.DetachAllEntities();
                OnRollback(EventArgs.Empty);
            }
        }

        protected virtual void OnRollback(EventArgs e)
        {
            RolledBack?.Invoke(this, e);
        }

        public void Dispose()
        {
            _transaction?.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            if (_transaction != null)
                await _transaction.DisposeAsync();
        }
    }
}