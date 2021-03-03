// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRegistry.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MyCard.Web.DependencyResolution {
    using MyCard.BoundedContext;
    using MyCard.BoundedContext.Repositories;
    using MyCard.Domain;
    using StructureMap;

	
	public class DefaultRegistry : Registry {
		#region Constructors and Destructors

		public DefaultRegistry() {
			Scan(
				scan =>
				{
					scan.TheCallingAssembly();
					scan.WithDefaultConventions();
					scan.With(new ControllerConvention());
				});
			For<ICompanyRepository>().Use<CompanyRepository>();
            For<IUserRepository>().Use<UserRepository>();
            For<ILicenseTypeRepository>().Use<LicenseTypeRepository>();
            For<IPurchaseOrderRepository>().Use<PurchaseOrderRepository>();
            For<IPaytabRepository>().Use<PaytabRepository>();
            For<IContactShareActivityRepository>().Use<ContactShareActivityRepository>();
            For<IInquiryRepository>().Use<InquiryRepository>();
            For<IEmailTemplateRepository>().Use<EmailTemplateRepository>();
            For<IMyCardFileRepository>().Use<MyCardFileRepository>();
            For<ICityRepository>().Use<CityRepository>();
            For<IIndustryRepository>().Use<IndustryRepository>();
            For<ILicenseDescriptionRepository>().Use<LicenseDescriptionRepository>();
            For<IPasswordHistoryRepository>().Use<PasswordHistoryRepository>();
            For<IMyCardUnitOfWork>().Use<MyCardUnitOfWork>();            
        }

		#endregion
	}
}