using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

#if NET6_0_OR_GREATER
using System.Text.Json;
#else
using System.Text;
using Newtonsoft.Json;
#endif

namespace Pug.RestApi.Featuring.AspNet;

public static class WebApplicationExtensions
{
	private const string MetaDataFeaturesPath = "/.meta-data/features";

#if NET6_0_OR_GREATER
	public static WebApplication UseFeaturesEndpoints( this WebApplication webApplication,
														bool requireAuthorization )
	{
		IEndpointConventionBuilder endpointConventionBuilder =
			webApplication.MapGet(
					MetaDataFeaturesPath,
					async context => await HandleFeatureListRequest( context, null )
				);

		if( requireAuthorization )
			endpointConventionBuilder.RequireAuthorization();

		return webApplication;
	}

#endif

	public static IApplicationBuilder UseFeaturesEndpoints(
		[Required] this IApplicationBuilder applicationBuilder )
	{
		applicationBuilder.Map(
				new PathString( MetaDataFeaturesPath ),
				builder => { builder.Use( HandleFeatureListRequest ); }
			);

		return applicationBuilder;
	}

	private static async Task HandleFeatureListRequest( HttpContext context, Func<Task>? _ )
	{
		IWebApiFeatureDiscovery webApiFeatureDiscovery =
			context.RequestServices.GetRequiredService<IWebApiFeatureDiscovery>();

		byte[]? featuresJsonContent = GetFeaturesJsonContent( webApiFeatureDiscovery, context );

		context.Response.StatusCode = 200;
		context.Response.ContentType = "application/json";
		context.Response.ContentLength = featuresJsonContent!.LongLength;
#if NET6_0_OR_GREATER
		await context.Response.Body.WriteAsync( featuresJsonContent );
#else
		using MemoryStream memoryStream = new ( featuresJsonContent );
		await memoryStream.CopyToAsync( context.Response.Body );
#endif
		await context.Response.Body.FlushAsync().ConfigureAwait( false );
	}

	private static byte[] GetFeaturesJsonContent( IWebApiFeatureDiscovery webApiFeatureDiscovery,
												HttpContext context )
	{
		IEnumerable<DeclaredFeature> declaredFeatures =
			webApiFeatureDiscovery.DiscoverDeclaredFeatures( context );

		using MemoryStream memoryStream = new ();

#if NET6_0_OR_GREATER
		JsonSerializer.Serialize(
				memoryStream,
				declaredFeatures,
				new JsonSerializerOptions()
				{
					WriteIndented = false
				}
			);
#else
		JsonSerializer jsonSerializer =
			JsonSerializer.CreateDefault( new JsonSerializerSettings() { Formatting = Formatting.None } );

		using JsonWriter jsonWriter =
			new JsonTextWriter( new StreamWriter( memoryStream, Encoding.UTF8, 32, true ) );

		jsonSerializer.Serialize( jsonWriter, declaredFeatures );

		jsonWriter.Flush();
#endif
		return memoryStream.ToArray();
	}
}