using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Pug.RestApi.Featuring.AspNet;

public interface IWebApiFeatureDiscovery
{
	IEnumerable<DeclaredFeature> DiscoverDeclaredFeatures(HttpContext context);
}