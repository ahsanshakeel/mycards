using  System;
using  System.Collections.Generic;
using  System.Linq.Expressions;
using  System.Web;
using  Umbraco.Core.Models;
using  Umbraco.Core.Models.PublishedContent;
using  Umbraco.Web;
using  Umbraco.ModelsBuilder;
using  Umbraco.ModelsBuilder.Umbraco;
[assembly: PureLiveAssembly]
[assembly:ModelsBuilderAssembly(PureLive = true, SourceHash = "fc6a6a2fff60175")]
[assembly:System.Reflection.AssemblyVersion("0.0.0.5")]


// FILE: models.generated.cs

//------------------------------------------------------------------------------
// <auto-generated>
//   This code was generated by a tool.
//
//    Umbraco.ModelsBuilder v3.0.7.99
//
//   Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------















namespace Umbraco.Web.PublishedContentModels
{
	/// <summary>Home Page</summary>
	[PublishedContentModel("homePage")]
	public partial class HomePage : PublishedContentModel
	{
#pragma warning disable 0109 // new is redundant
		public new const string ModelTypeAlias = "homePage";
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
#pragma warning restore 0109

		public HomePage(IPublishedContent content)
			: base(content)
		{ }

#pragma warning disable 0109 // new is redundant
		public new static PublishedContentType GetModelContentType()
		{
			return PublishedContentType.Get(ModelItemType, ModelTypeAlias);
		}
#pragma warning restore 0109

		public static PublishedPropertyType GetModelPropertyType<TValue>(Expression<Func<HomePage, TValue>> selector)
		{
			return PublishedContentModelUtility.GetModelPropertyType(GetModelContentType(), selector);
		}

		///<summary>
		/// Contact Us Content 1
		///</summary>
		[ImplementPropertyType("contactUsContent1")]
		public IHtmlString ContactUsContent1
		{
			get { return this.GetPropertyValue<IHtmlString>("contactUsContent1"); }
		}

		///<summary>
		/// Contact Us Content 2
		///</summary>
		[ImplementPropertyType("contactUsContent2")]
		public IHtmlString ContactUsContent2
		{
			get { return this.GetPropertyValue<IHtmlString>("contactUsContent2"); }
		}

		///<summary>
		/// Contact Us Content 3
		///</summary>
		[ImplementPropertyType("contactUsContent3")]
		public IHtmlString ContactUsContent3
		{
			get { return this.GetPropertyValue<IHtmlString>("contactUsContent3"); }
		}

		///<summary>
		/// Contact Us Content 4
		///</summary>
		[ImplementPropertyType("contactUsContent4")]
		public IHtmlString ContactUsContent4
		{
			get { return this.GetPropertyValue<IHtmlString>("contactUsContent4"); }
		}

		///<summary>
		/// Contact Us Heading 1
		///</summary>
		[ImplementPropertyType("contactUsHeading1")]
		public string ContactUsHeading1
		{
			get { return this.GetPropertyValue<string>("contactUsHeading1"); }
		}

		///<summary>
		/// Contact Us Heading 2
		///</summary>
		[ImplementPropertyType("contactUsHeading2")]
		public string ContactUsHeading2
		{
			get { return this.GetPropertyValue<string>("contactUsHeading2"); }
		}

		///<summary>
		/// Contact Us Heading 3
		///</summary>
		[ImplementPropertyType("contactUsHeading3")]
		public string ContactUsHeading3
		{
			get { return this.GetPropertyValue<string>("contactUsHeading3"); }
		}

		///<summary>
		/// Contact Us Heading 4
		///</summary>
		[ImplementPropertyType("contactUsHeading4")]
		public string ContactUsHeading4
		{
			get { return this.GetPropertyValue<string>("contactUsHeading4"); }
		}

		///<summary>
		/// Contact Us Main Heading
		///</summary>
		[ImplementPropertyType("contactUsMainHeading")]
		public string ContactUsMainHeading
		{
			get { return this.GetPropertyValue<string>("contactUsMainHeading"); }
		}

		///<summary>
		/// Footer List 1
		///</summary>
		[ImplementPropertyType("footerList1")]
		public IHtmlString FooterList1
		{
			get { return this.GetPropertyValue<IHtmlString>("footerList1"); }
		}

		///<summary>
		/// Footer List 1 Heading
		///</summary>
		[ImplementPropertyType("footerList1Heading")]
		public string FooterList1Heading
		{
			get { return this.GetPropertyValue<string>("footerList1Heading"); }
		}

		///<summary>
		/// Footer List 2
		///</summary>
		[ImplementPropertyType("footerList2")]
		public IHtmlString FooterList2
		{
			get { return this.GetPropertyValue<IHtmlString>("footerList2"); }
		}

		///<summary>
		/// Footer List 2 Heading
		///</summary>
		[ImplementPropertyType("footerList2Heading")]
		public string FooterList2Heading
		{
			get { return this.GetPropertyValue<string>("footerList2Heading"); }
		}

		///<summary>
		/// Footer List 3
		///</summary>
		[ImplementPropertyType("footerList3")]
		public IHtmlString FooterList3
		{
			get { return this.GetPropertyValue<IHtmlString>("footerList3"); }
		}

		///<summary>
		/// Footer List 3 Heading
		///</summary>
		[ImplementPropertyType("footerList3Heading")]
		public string FooterList3Heading
		{
			get { return this.GetPropertyValue<string>("footerList3Heading"); }
		}

		///<summary>
		/// Footer List 4
		///</summary>
		[ImplementPropertyType("footerList4")]
		public IHtmlString FooterList4
		{
			get { return this.GetPropertyValue<IHtmlString>("footerList4"); }
		}

		///<summary>
		/// Footer List 4 Heading
		///</summary>
		[ImplementPropertyType("footerList4Heading")]
		public string FooterList4Heading
		{
			get { return this.GetPropertyValue<string>("footerList4Heading"); }
		}

		///<summary>
		/// Footer List 5
		///</summary>
		[ImplementPropertyType("footerList5")]
		public IHtmlString FooterList5
		{
			get { return this.GetPropertyValue<IHtmlString>("footerList5"); }
		}

		///<summary>
		/// Footer List 5 Heading
		///</summary>
		[ImplementPropertyType("footerList5Heading")]
		public string FooterList5Heading
		{
			get { return this.GetPropertyValue<string>("footerList5Heading"); }
		}

		///<summary>
		/// Go Green CSR
		///</summary>
		[ImplementPropertyType("goGreenCSR")]
		public string GoGreenCsr
		{
			get { return this.GetPropertyValue<string>("goGreenCSR"); }
		}

		///<summary>
		/// Go Green Evolution
		///</summary>
		[ImplementPropertyType("goGreenEvolution")]
		public string GoGreenEvolution
		{
			get { return this.GetPropertyValue<string>("goGreenEvolution"); }
		}

		///<summary>
		/// Go Green Main Heading
		///</summary>
		[ImplementPropertyType("goGreenMainHeading")]
		public string GoGreenMainHeading
		{
			get { return this.GetPropertyValue<string>("goGreenMainHeading"); }
		}

		///<summary>
		/// Go Green Para 1
		///</summary>
		[ImplementPropertyType("goGreenPara1")]
		public IHtmlString GoGreenPara1
		{
			get { return this.GetPropertyValue<IHtmlString>("goGreenPara1"); }
		}

		///<summary>
		/// Go Green Para 2
		///</summary>
		[ImplementPropertyType("goGreenPara2")]
		public IHtmlString GoGreenPara2
		{
			get { return this.GetPropertyValue<IHtmlString>("goGreenPara2"); }
		}

		///<summary>
		/// Go Green Slogan
		///</summary>
		[ImplementPropertyType("goGreenSlogan")]
		public string GoGreenSlogan
		{
			get { return this.GetPropertyValue<string>("goGreenSlogan"); }
		}

		///<summary>
		/// Go Green YourCSR
		///</summary>
		[ImplementPropertyType("goGreenYourCSR")]
		public string GoGreenYourCsr
		{
			get { return this.GetPropertyValue<string>("goGreenYourCSR"); }
		}

		///<summary>
		/// Main Headline
		///</summary>
		[ImplementPropertyType("uspMainHeadline")]
		public string UspMainHeadline
		{
			get { return this.GetPropertyValue<string>("uspMainHeadline"); }
		}

		///<summary>
		/// Supporting Headline
		///</summary>
		[ImplementPropertyType("uspSupportingHeadline")]
		public string UspSupportingHeadline
		{
			get { return this.GetPropertyValue<string>("uspSupportingHeadline"); }
		}

		///<summary>
		/// Whats Cool Content 1
		///</summary>
		[ImplementPropertyType("whatsCoolContent1")]
		public IHtmlString WhatsCoolContent1
		{
			get { return this.GetPropertyValue<IHtmlString>("whatsCoolContent1"); }
		}

		///<summary>
		/// Whats Cool Content 2
		///</summary>
		[ImplementPropertyType("whatsCoolContent2")]
		public IHtmlString WhatsCoolContent2
		{
			get { return this.GetPropertyValue<IHtmlString>("whatsCoolContent2"); }
		}

		///<summary>
		/// Whats Cool Content 3
		///</summary>
		[ImplementPropertyType("whatsCoolContent3")]
		public IHtmlString WhatsCoolContent3
		{
			get { return this.GetPropertyValue<IHtmlString>("whatsCoolContent3"); }
		}

		///<summary>
		/// Whats Cool Content 4
		///</summary>
		[ImplementPropertyType("whatsCoolContent4")]
		public IHtmlString WhatsCoolContent4
		{
			get { return this.GetPropertyValue<IHtmlString>("whatsCoolContent4"); }
		}

		///<summary>
		/// Whats Cool Heading 1
		///</summary>
		[ImplementPropertyType("whatsCoolHeading1")]
		public string WhatsCoolHeading1
		{
			get { return this.GetPropertyValue<string>("whatsCoolHeading1"); }
		}

		///<summary>
		/// Whats Cool Heading 2
		///</summary>
		[ImplementPropertyType("whatsCoolHeading2")]
		public string WhatsCoolHeading2
		{
			get { return this.GetPropertyValue<string>("whatsCoolHeading2"); }
		}

		///<summary>
		/// Whats Cool Heading 3
		///</summary>
		[ImplementPropertyType("whatsCoolHeading3")]
		public string WhatsCoolHeading3
		{
			get { return this.GetPropertyValue<string>("whatsCoolHeading3"); }
		}

		///<summary>
		/// Whats Cool Heading 4
		///</summary>
		[ImplementPropertyType("whatsCoolHeading4")]
		public string WhatsCoolHeading4
		{
			get { return this.GetPropertyValue<string>("whatsCoolHeading4"); }
		}

		///<summary>
		/// Whats Cool Main Heading
		///</summary>
		[ImplementPropertyType("whatsCoolMainHeading")]
		public string WhatsCoolMainHeading
		{
			get { return this.GetPropertyValue<string>("whatsCoolMainHeading"); }
		}

		///<summary>
		/// Whats Cool Slogan
		///</summary>
		[ImplementPropertyType("whatsCoolSlogan")]
		public string WhatsCoolSlogan
		{
			get { return this.GetPropertyValue<string>("whatsCoolSlogan"); }
		}

		///<summary>
		/// Why MayCard Para 1
		///</summary>
		[ImplementPropertyType("whyMayCardPara1")]
		public string WhyMayCardPara1
		{
			get { return this.GetPropertyValue<string>("whyMayCardPara1"); }
		}

		///<summary>
		/// Why MyCard Heading
		///</summary>
		[ImplementPropertyType("whyMyCardHeading")]
		public string WhyMyCardHeading
		{
			get { return this.GetPropertyValue<string>("whyMyCardHeading"); }
		}

		///<summary>
		/// Why MyCard Para 2
		///</summary>
		[ImplementPropertyType("whyMyCardPara2")]
		public string WhyMyCardPara2
		{
			get { return this.GetPropertyValue<string>("whyMyCardPara2"); }
		}

		///<summary>
		/// Why MyCard Sub Heading
		///</summary>
		[ImplementPropertyType("WhyMyCardSubHeading")]
		public string WhyMyCardSubHeading
		{
			get { return this.GetPropertyValue<string>("WhyMyCardSubHeading"); }
		}
	}

	/// <summary>Folder</summary>
	[PublishedContentModel("Folder")]
	public partial class Folder : PublishedContentModel
	{
#pragma warning disable 0109 // new is redundant
		public new const string ModelTypeAlias = "Folder";
		public new const PublishedItemType ModelItemType = PublishedItemType.Media;
#pragma warning restore 0109

		public Folder(IPublishedContent content)
			: base(content)
		{ }

#pragma warning disable 0109 // new is redundant
		public new static PublishedContentType GetModelContentType()
		{
			return PublishedContentType.Get(ModelItemType, ModelTypeAlias);
		}
#pragma warning restore 0109

		public static PublishedPropertyType GetModelPropertyType<TValue>(Expression<Func<Folder, TValue>> selector)
		{
			return PublishedContentModelUtility.GetModelPropertyType(GetModelContentType(), selector);
		}

		///<summary>
		/// Contents:
		///</summary>
		[ImplementPropertyType("contents")]
		public object Contents
		{
			get { return this.GetPropertyValue("contents"); }
		}
	}

	/// <summary>Image</summary>
	[PublishedContentModel("Image")]
	public partial class Image : PublishedContentModel
	{
#pragma warning disable 0109 // new is redundant
		public new const string ModelTypeAlias = "Image";
		public new const PublishedItemType ModelItemType = PublishedItemType.Media;
#pragma warning restore 0109

		public Image(IPublishedContent content)
			: base(content)
		{ }

#pragma warning disable 0109 // new is redundant
		public new static PublishedContentType GetModelContentType()
		{
			return PublishedContentType.Get(ModelItemType, ModelTypeAlias);
		}
#pragma warning restore 0109

		public static PublishedPropertyType GetModelPropertyType<TValue>(Expression<Func<Image, TValue>> selector)
		{
			return PublishedContentModelUtility.GetModelPropertyType(GetModelContentType(), selector);
		}

		///<summary>
		/// Size
		///</summary>
		[ImplementPropertyType("umbracoBytes")]
		public string UmbracoBytes
		{
			get { return this.GetPropertyValue<string>("umbracoBytes"); }
		}

		///<summary>
		/// Type
		///</summary>
		[ImplementPropertyType("umbracoExtension")]
		public string UmbracoExtension
		{
			get { return this.GetPropertyValue<string>("umbracoExtension"); }
		}

		///<summary>
		/// Upload image
		///</summary>
		[ImplementPropertyType("umbracoFile")]
		public Umbraco.Web.Models.ImageCropDataSet UmbracoFile
		{
			get { return this.GetPropertyValue<Umbraco.Web.Models.ImageCropDataSet>("umbracoFile"); }
		}

		///<summary>
		/// Height
		///</summary>
		[ImplementPropertyType("umbracoHeight")]
		public string UmbracoHeight
		{
			get { return this.GetPropertyValue<string>("umbracoHeight"); }
		}

		///<summary>
		/// Width
		///</summary>
		[ImplementPropertyType("umbracoWidth")]
		public string UmbracoWidth
		{
			get { return this.GetPropertyValue<string>("umbracoWidth"); }
		}
	}

	/// <summary>File</summary>
	[PublishedContentModel("File")]
	public partial class File : PublishedContentModel
	{
#pragma warning disable 0109 // new is redundant
		public new const string ModelTypeAlias = "File";
		public new const PublishedItemType ModelItemType = PublishedItemType.Media;
#pragma warning restore 0109

		public File(IPublishedContent content)
			: base(content)
		{ }

#pragma warning disable 0109 // new is redundant
		public new static PublishedContentType GetModelContentType()
		{
			return PublishedContentType.Get(ModelItemType, ModelTypeAlias);
		}
#pragma warning restore 0109

		public static PublishedPropertyType GetModelPropertyType<TValue>(Expression<Func<File, TValue>> selector)
		{
			return PublishedContentModelUtility.GetModelPropertyType(GetModelContentType(), selector);
		}

		///<summary>
		/// Size
		///</summary>
		[ImplementPropertyType("umbracoBytes")]
		public string UmbracoBytes
		{
			get { return this.GetPropertyValue<string>("umbracoBytes"); }
		}

		///<summary>
		/// Type
		///</summary>
		[ImplementPropertyType("umbracoExtension")]
		public string UmbracoExtension
		{
			get { return this.GetPropertyValue<string>("umbracoExtension"); }
		}

		///<summary>
		/// Upload file
		///</summary>
		[ImplementPropertyType("umbracoFile")]
		public string UmbracoFile
		{
			get { return this.GetPropertyValue<string>("umbracoFile"); }
		}
	}

	/// <summary>Member</summary>
	[PublishedContentModel("Member")]
	public partial class Member : PublishedContentModel
	{
#pragma warning disable 0109 // new is redundant
		public new const string ModelTypeAlias = "Member";
		public new const PublishedItemType ModelItemType = PublishedItemType.Member;
#pragma warning restore 0109

		public Member(IPublishedContent content)
			: base(content)
		{ }

#pragma warning disable 0109 // new is redundant
		public new static PublishedContentType GetModelContentType()
		{
			return PublishedContentType.Get(ModelItemType, ModelTypeAlias);
		}
#pragma warning restore 0109

		public static PublishedPropertyType GetModelPropertyType<TValue>(Expression<Func<Member, TValue>> selector)
		{
			return PublishedContentModelUtility.GetModelPropertyType(GetModelContentType(), selector);
		}

		///<summary>
		/// Is Approved
		///</summary>
		[ImplementPropertyType("umbracoMemberApproved")]
		public bool UmbracoMemberApproved
		{
			get { return this.GetPropertyValue<bool>("umbracoMemberApproved"); }
		}

		///<summary>
		/// Comments
		///</summary>
		[ImplementPropertyType("umbracoMemberComments")]
		public string UmbracoMemberComments
		{
			get { return this.GetPropertyValue<string>("umbracoMemberComments"); }
		}

		///<summary>
		/// Failed Password Attempts
		///</summary>
		[ImplementPropertyType("umbracoMemberFailedPasswordAttempts")]
		public string UmbracoMemberFailedPasswordAttempts
		{
			get { return this.GetPropertyValue<string>("umbracoMemberFailedPasswordAttempts"); }
		}

		///<summary>
		/// Last Lockout Date
		///</summary>
		[ImplementPropertyType("umbracoMemberLastLockoutDate")]
		public string UmbracoMemberLastLockoutDate
		{
			get { return this.GetPropertyValue<string>("umbracoMemberLastLockoutDate"); }
		}

		///<summary>
		/// Last Login Date
		///</summary>
		[ImplementPropertyType("umbracoMemberLastLogin")]
		public string UmbracoMemberLastLogin
		{
			get { return this.GetPropertyValue<string>("umbracoMemberLastLogin"); }
		}

		///<summary>
		/// Last Password Change Date
		///</summary>
		[ImplementPropertyType("umbracoMemberLastPasswordChangeDate")]
		public string UmbracoMemberLastPasswordChangeDate
		{
			get { return this.GetPropertyValue<string>("umbracoMemberLastPasswordChangeDate"); }
		}

		///<summary>
		/// Is Locked Out
		///</summary>
		[ImplementPropertyType("umbracoMemberLockedOut")]
		public bool UmbracoMemberLockedOut
		{
			get { return this.GetPropertyValue<bool>("umbracoMemberLockedOut"); }
		}

		///<summary>
		/// Password Answer
		///</summary>
		[ImplementPropertyType("umbracoMemberPasswordRetrievalAnswer")]
		public string UmbracoMemberPasswordRetrievalAnswer
		{
			get { return this.GetPropertyValue<string>("umbracoMemberPasswordRetrievalAnswer"); }
		}

		///<summary>
		/// Password Question
		///</summary>
		[ImplementPropertyType("umbracoMemberPasswordRetrievalQuestion")]
		public string UmbracoMemberPasswordRetrievalQuestion
		{
			get { return this.GetPropertyValue<string>("umbracoMemberPasswordRetrievalQuestion"); }
		}
	}

}



// EOF
