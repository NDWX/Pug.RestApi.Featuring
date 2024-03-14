using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Pug.RestApi.Featuring.AspNet;

internal class WebApiFeatureDiscovery : IWebApiFeatureDiscovery
{
	private readonly IApiDescriptionGroupCollectionProvider _apiDescriptionGroupCollectionProvider;

	private IEnumerable<DeclaredFeature>? discoveredFeatures;

	public WebApiFeatureDiscovery(
		IApiDescriptionGroupCollectionProvider apiDescriptionGroupCollectionProvider )
	{
		_apiDescriptionGroupCollectionProvider = apiDescriptionGroupCollectionProvider;
	}

	public IEnumerable<DeclaredFeature> DiscoverDeclaredFeatures(HttpContext _)
	{
		if( discoveredFeatures is not null )
			return discoveredFeatures;

		List<DeclaredFeature> declaredFeatures = new ();

		foreach( ApiDescriptionGroup endpointDescriptionGroup in _apiDescriptionGroupCollectionProvider.ApiDescriptionGroups.Items )
		{
			foreach( ApiDescription endpointDescription in endpointDescriptionGroup.Items )
			{
				IEnumerable<ApiSubFeatureAttribute> subFeatureAttributes =
					endpointDescription
						.GetCustomAttributes( out Type? declaringType )
						.OfType<ApiSubFeatureAttribute>();

				if( declaringType is null )
					continue;

				IEnumerable<ApiFeatureAttribute> featureAttributes = declaringType.GetCustomAttributes<ApiFeatureAttribute>();

				declaredFeatures.AddRange( featureAttributes.Select( featureAttribute => new DeclaredFeature()
				{
					Name = featureAttribute.Name, Version = featureAttribute.Version
				} ) );

				declaredFeatures.AddRange( subFeatureAttributes.Select( featureAttribute => new DeclaredFeature()
				{
					Name = $"{featureAttribute.Feature}/{featureAttribute.Name}", Version = featureAttribute.Version
				} ) );
			}
		}

		discoveredFeatures = declaredFeatures.Distinct();

		return discoveredFeatures;
	}
}