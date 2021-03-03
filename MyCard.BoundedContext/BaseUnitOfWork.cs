using MyCard.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyCard.BoundedContext
{
    public abstract class BaseUnitOfWork : DbContext, IQueryableUnitOfWork, IUnitOfWork, IDisposable, ISql
    {
        public BaseUnitOfWork(string connectionString) : base(connectionString) { }

        public void ApplyCurrentValues<TEntity>(TEntity original, TEntity current) where TEntity : class
        {
            this.Entry<TEntity>(original).CurrentValues.SetValues((object)current);
        }

        public void Attach<TEntity>(TEntity item) where TEntity : class
        {
            this.Entry<TEntity>(item).State = EntityState.Unchanged;
        }

        public void Commit()
        {
            try
            {
                this.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                throw this.GetDBValidationExptions(ex);
            }
        }

        public void CommitAndRefreshChanges()
        {
            bool saveFailed = false;

            do
            {
                try
                {
                    base.SaveChanges();

                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;

                    ex.Entries.ToList()
                              .ForEach(entry =>
                              {
                                  entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                              });

                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var entry in this.ChangeTracker.Entries())
                    {
                        if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                        {
                            entry.State = EntityState.Detached;
                        }
                    }

                    throw GetDBValidationExptions(dbEx);

                }
            } while (saveFailed);
        }

        public async Task CommitAndRefreshChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            bool saveFailed = false;

            do
            {
                try
                {
                    await base.SaveChangesAsync(cancellationToken);

                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;

                    ex.Entries.ToList()
                              .ForEach(entry =>
                              {
                                  entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                              });

                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var entry in this.ChangeTracker.Entries())
                    {
                        if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                        {
                            entry.State = EntityState.Detached;
                        }
                    }

                    throw GetDBValidationExptions(dbEx);
                }

            } while (saveFailed);
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                await this.SaveChangesAsync(cancellationToken);
            }
            catch (DbEntityValidationException ex)
            {
                throw this.GetDBValidationExptions(ex);
            }
        }

        public IQueryable<TEntity> CreateSet<TEntity>() where TEntity : class, new()
        {
            return (IDbSet<TEntity>)this.Set<TEntity>();
        }

        public int ExecuteCommand(string sqlCommand, params object[] parameters)
        {
            return this.Database.ExecuteSqlCommand(sqlCommand, parameters);
        }

        public IEnumerable<TEntity> ExecuteQuery<TEntity>(string sqlQuery, params object[] parameters)
        {
            return (IEnumerable<TEntity>)this.Database.SqlQuery<TEntity>(sqlQuery, parameters);
        }

        public void RollbackChanges()
        {
            this.ChangeTracker.Entries().ToList().ForEach((entry => entry.State = EntityState.Unchanged));
        }

        public void SetModified<TEntity>(TEntity item) where TEntity : class
        {
            this.Entry<TEntity>(item).State = EntityState.Modified;
        }
        private Exception GetDBValidationExptions(DbEntityValidationException dbEx)
        {
            string message = string.Empty;
            foreach (DbEntityValidationResult validationResult in dbEx.EntityValidationErrors)
            {
                foreach (DbValidationError dbValidationError in (IEnumerable<DbValidationError>)validationResult.ValidationErrors)
                    message = message + string.Format("Property: {0} Error: {1}", (object)dbValidationError.PropertyName, (object)dbValidationError.ErrorMessage);
            }
            return new Exception("ValidationError", new Exception(message));
        }
    }
}
