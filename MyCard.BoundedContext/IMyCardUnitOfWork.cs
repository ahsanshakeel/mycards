using MyCard.Core;
using MyCard.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCard.BoundedContext
{
    public interface IMyCardUnitOfWork: IQueryableUnitOfWork, IUnitOfWork, IDisposable, ISql
    {
        IDbSet<Company> Companies { get; }
        IDbSet<User> Users { get; }
        IDbSet<LicenseType> LicenseTypes { get; }
        IDbSet<PurchaseOrder> PurchaseOrders { get; }
        IDbSet<PayTab_ResponseModel> payTab_ResponseModels { get; }
        IDbSet<ContactShareActivity> ContactShareActivities { get; }
        IDbSet<Contact> Contacts { get; }
        IDbSet<Inquiry> Inquiries { get; }
        IDbSet<EmailTemplate> EmailTemplates { get; }
        IDbSet<MyCardFile> MyCardFiles { get; }
        IDbSet<City> Cities { get; }
        IDbSet<Industry> Industries { get; }
        IDbSet<LicenseDescription> LicenseDescriptions { get; }
        IDbSet<PasswordHistory> PasswordHistories { get; }
    }
}
