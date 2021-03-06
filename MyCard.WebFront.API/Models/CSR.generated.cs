//------------------------------------------------------------------------------
// <auto-generated>
//   This code was generated by a tool.
//
//    Umbraco.ModelsBuilder v3.0.7.99
//
//   Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.ModelsBuilder;
using Umbraco.ModelsBuilder.Umbraco;

namespace MyCard.WebFront.API.Models
{
	/// <summary>CSR</summary>
	[PublishedContentModel("cSR")]
	public partial class CSR : PublishedContentModel
	{
#pragma warning disable 0109 // new is redundant
		public new const string ModelTypeAlias = "cSR";
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
#pragma warning restore 0109

		public CSR(IPublishedContent content)
			: base(content)
		{ }

#pragma warning disable 0109 // new is redundant
		public new static PublishedContentType GetModelContentType()
		{
			return PublishedContentType.Get(ModelItemType, ModelTypeAlias);
		}
#pragma warning restore 0109

		public static PublishedPropertyType GetModelPropertyType<TValue>(Expression<Func<CSR, TValue>> selector)
		{
			return PublishedContentModelUtility.GetModelPropertyType(GetModelContentType(), selector);
		}

		///<summary>
		/// Challenges
		///</summary>
		[ImplementPropertyType("challenges")]
		public IEnumerable<IPublishedContent> Challenges
		{
			get { return this.GetPropertyValue<IEnumerable<IPublishedContent>>("challenges"); }
		}

		///<summary>
		/// Challenges Heading
		///</summary>
		[ImplementPropertyType("challengesHeading")]
		public string ChallengesHeading
		{
			get { return this.GetPropertyValue<string>("challengesHeading"); }
		}

		///<summary>
		/// CSR Banner Image
		///</summary>
		[ImplementPropertyType("cSRBannerImage")]
		public IPublishedContent CSrbannerImage
		{
			get { return this.GetPropertyValue<IPublishedContent>("cSRBannerImage"); }
		}

		///<summary>
		/// CSR Banner Image Text
		///</summary>
		[ImplementPropertyType("cSRBannerImageText")]
		public IHtmlString CSrbannerImageText
		{
			get { return this.GetPropertyValue<IHtmlString>("cSRBannerImageText"); }
		}

		///<summary>
		/// General Text
		///</summary>
		[ImplementPropertyType("generalText")]
		public IHtmlString GeneralText
		{
			get { return this.GetPropertyValue<IHtmlString>("generalText"); }
		}

		///<summary>
		/// Green Image
		///</summary>
		[ImplementPropertyType("greenImage")]
		public IPublishedContent GreenImage
		{
			get { return this.GetPropertyValue<IPublishedContent>("greenImage"); }
		}

		///<summary>
		/// Green Text
		///</summary>
		[ImplementPropertyType("greenText")]
		public IHtmlString GreenText
		{
			get { return this.GetPropertyValue<IHtmlString>("greenText"); }
		}

		///<summary>
		/// Mission Image
		///</summary>
		[ImplementPropertyType("missionImage")]
		public IPublishedContent MissionImage
		{
			get { return this.GetPropertyValue<IPublishedContent>("missionImage"); }
		}

		///<summary>
		/// Our Mission Text
		///</summary>
		[ImplementPropertyType("ourMissionText")]
		public IHtmlString OurMissionText
		{
			get { return this.GetPropertyValue<IHtmlString>("ourMissionText"); }
		}

		///<summary>
		/// Solution Text
		///</summary>
		[ImplementPropertyType("solutionText")]
		public IHtmlString SolutionText
		{
			get { return this.GetPropertyValue<IHtmlString>("solutionText"); }
		}
	}
}
