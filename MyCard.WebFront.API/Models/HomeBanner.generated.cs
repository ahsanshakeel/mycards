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
	/// <summary>Home Banner</summary>
	[PublishedContentModel("homeBanner")]
	public partial class HomeBanner : PublishedContentModel
	{
#pragma warning disable 0109 // new is redundant
		public new const string ModelTypeAlias = "homeBanner";
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
#pragma warning restore 0109

		public HomeBanner(IPublishedContent content)
			: base(content)
		{ }

#pragma warning disable 0109 // new is redundant
		public new static PublishedContentType GetModelContentType()
		{
			return PublishedContentType.Get(ModelItemType, ModelTypeAlias);
		}
#pragma warning restore 0109

		public static PublishedPropertyType GetModelPropertyType<TValue>(Expression<Func<HomeBanner, TValue>> selector)
		{
			return PublishedContentModelUtility.GetModelPropertyType(GetModelContentType(), selector);
		}

		///<summary>
		/// Banner Image
		///</summary>
		[ImplementPropertyType("bannerImage")]
		public IPublishedContent BannerImage
		{
			get { return this.GetPropertyValue<IPublishedContent>("bannerImage"); }
		}

		///<summary>
		/// Tag Line 1
		///</summary>
		[ImplementPropertyType("tagLine1")]
		public string TagLine1
		{
			get { return this.GetPropertyValue<string>("tagLine1"); }
		}

		///<summary>
		/// Tag Line 2
		///</summary>
		[ImplementPropertyType("tagLine2")]
		public string TagLine2
		{
			get { return this.GetPropertyValue<string>("tagLine2"); }
		}
	}
}
