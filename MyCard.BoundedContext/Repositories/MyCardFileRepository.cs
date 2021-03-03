using MyCard.Core;
using MyCard.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCard.BoundedContext.Repositories
{
    public class MyCardFileRepository : Repository<MyCardFile>, IMyCardFileRepository
    {
        public MyCardFileRepository(IMyCardUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
