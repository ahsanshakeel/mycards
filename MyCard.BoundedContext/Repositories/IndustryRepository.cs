using MyCard.Core;
using MyCard.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCard.BoundedContext.Repositories
{
    public class IndustryRepository : Repository<Industry>, IIndustryRepository
    {
        public IndustryRepository(IMyCardUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
