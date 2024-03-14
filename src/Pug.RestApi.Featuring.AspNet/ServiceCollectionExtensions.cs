using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;

namespace Pug.RestApi.Featuring.AspNet;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddWebApiFeatureDiscovery( this IServiceCollection serviceCollection )
	{
		serviceCollection.AddSingleton<IWebApiFeatureDiscovery>(
				sp =>
					new WebApiFeatureDiscovery(
						sp.GetRequiredService<IApiDescriptionGroupCollectionProvider>() )
			);

		return serviceCollection;
	}
}