using MySql.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyCard.Domain;

namespace MyCard.BoundedContext
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class MyCardUnitOfWork: BaseUnitOfWork, IMyCardUnitOfWork
    {
        public MyCardUnitOfWork() : base("name=mycard") { }

        private IDbSet<Company> _companies;
        public IDbSet<Company> Companies
        {
            get
            {
                if (this._companies == null)
                    this._companies = (IDbSet<Company>)this.Set<Company>();
                return this._companies;
            }
        }

        public IDbSet<User> _users;
        public IDbSet<User> Users
        {
            get
            {
                if (this._users == null)
                    this._users = (IDbSet<User>)this.Set<User>();
                return this._users;
            }
        }

        public IDbSet<LicenseType> _licenseTypes;
        public IDbSet<LicenseType> LicenseTypes
        {
            get
            {
                if (this._licenseTypes == null)
                    this._licenseTypes = (IDbSet<LicenseType>)this.Set<LicenseType>();
                return this._licenseTypes;
            }
        }

        public IDbSet<PurchaseOrder> _purchaseOrders;
        public IDbSet<PurchaseOrder> PurchaseOrders
        {
            get
            {
                if (this._purchaseOrders == null)
                    this._purchaseOrders = (IDbSet<PurchaseOrder>)this.Set<PurchaseOrder>();
                return this._purchaseOrders;
            }
        }

        public IDbSet<ContactShareActivity> _contactShareActivities;
        public IDbSet<ContactShareActivity> ContactShareActivities
        {
            get
            {
                if (this._contactShareActivities == null)
                    this._contactShareActivities = (IDbSet<ContactShareActivity>)this.Set<ContactShareActivity>();
                return this._contactShareActivities;
            }
        }

        public IDbSet<Inquiry> _inquiries;
        public IDbSet<Inquiry> Inquiries
        {
            get
            {
                if (this._inquiries == null)
                    this._inquiries = (IDbSet<Inquiry>)this.Set<Inquiry>();
                return this._inquiries;
            }
        }

        public IDbSet<Contact> _contacs;
        public IDbSet<Contact> Contacts
        {
            get
            {
                if (this._contacs == null)
                    this._contacs = (IDbSet<Contact>)this.Set<Contact>();
                return this._contacs;
            }
        }
        public IDbSet<EmailTemplate> _emailTemplates;
        public IDbSet<EmailTemplate> EmailTemplates
        {
            get
            {
                if (this._emailTemplates == null)
                    this._emailTemplates = (IDbSet<EmailTemplate>)this.Set<EmailTemplate>();
                return this._emailTemplates;
            }
        }
        public IDbSet<MyCardFile> _myCardFiles;
        public IDbSet<MyCardFile> MyCardFiles
        {
            get
            {
                if (this._myCardFiles == null)
                    this._myCardFiles = (IDbSet<MyCardFile>)this.Set<MyCardFile>();
                return this._myCardFiles;
            }
        }
        public IDbSet<City> _cities;
        public IDbSet<City> Cities
        {
            get
            {
                if (this._cities == null)
                    this._cities = (IDbSet<City>)this.Set<City>();
                return this._cities;
            }
        }
        public IDbSet<Industry> _industries;
        public IDbSet<Industry> Industries
        {
            get
            {
                if (this._industries == null)
                    this._industries = (IDbSet<Industry>)this.Set<Industry>();
                return this._industries;
            }
        }
        public IDbSet<LicenseDescription> _licenseDescriptions;
        public IDbSet<LicenseDescription> LicenseDescriptions
        {
            get
            {
                if (this._licenseDescriptions == null)
                    this._licenseDescriptions = (IDbSet<LicenseDescription>)this.Set<LicenseDescription>();
                return this._licenseDescriptions;
            }
        }

        public IDbSet<PasswordHistory> _passwordHistories;
        public IDbSet<PasswordHistory> PasswordHistories
        {
            get
            {
                if (this._passwordHistories == null)
                    this._passwordHistories = (IDbSet<PasswordHistory>)this.Set<PasswordHistory>();
                return this._passwordHistories;
            }
        }
        public IDbSet<PayTab_ResponseModel> _payTab_ResponseModels;
        public IDbSet<PayTab_ResponseModel> payTab_ResponseModels
        {
            get 
            {
                if (this._payTab_ResponseModels == null)
                    this._payTab_ResponseModels = (IDbSet<PayTab_ResponseModel>)this.Set<PayTab_ResponseModel>();
                return this._payTab_ResponseModels;
            }
        }
    }
}
