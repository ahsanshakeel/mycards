using MyCard.Core;
using MyCard.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCard.BoundedContext.Repositories
{
  
    public class PaytabRepository : Repository<PayTab_ResponseModel>, IPaytabRepository
    {
        public PaytabRepository(IMyCardUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}
