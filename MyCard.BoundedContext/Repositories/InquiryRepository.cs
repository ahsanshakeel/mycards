using MyCard.Core;
using MyCard.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCard.BoundedContext.Repositories
{
    public class InquiryRepository : Repository<Inquiry>, IInquiryRepository
    {
        public InquiryRepository(IMyCardUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
