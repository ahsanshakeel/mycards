using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyCard.Core
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Commit all changes made in a container.
        /// </summary>
        ///<remarks>
        /// If the entity have fixed properties and any optimistic concurrency problem exists,  
        /// then an exception is thrown
        ///</remarks>
        void Commit();

        /// <summary>
        /// Commit all changes made in  a container.
        /// </summary>
        ///<remarks>
        /// If the entity have fixed properties and any optimistic concurrency problem exists,
        /// then 'client changes' are refreshed - Client wins
        ///</remarks>
        void CommitAndRefreshChanges();

        /// <summary>
        /// Rollback changes are not stored in the database at 
        /// this moment. See references of UnitOfWork pattern
        /// </summary>
        void RollbackChanges();

        /// <summary>
        /// Commit all changes made in a container Async.
        /// </summary>
        ///<remarks>
        /// If the entity have fixed properties and any optimistic concurrency problem exists,  
        /// then an exception is thrown
        ///</remarks>
        Task CommitAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Commit all changes made in  a container Async.
        /// </summary>
        ///<remarks>
        /// If the entity have fixed properties and any optimistic concurrency problem exists,
        /// then 'client changes' are refreshed - Client wins
        ///</remarks>
        Task CommitAndRefreshChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
