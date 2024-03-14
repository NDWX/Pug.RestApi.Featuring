using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Pug.RestApi.Featuring.AspNet;

internal static class ApiDescriptionExtensions
{
	private static bool TryGetMethodInfo( this ApiDescription apiDescription, out MethodInfo? methodInfo )
	{
		if( apiDescription.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor )
		{
			methodInfo = controllerActionDescriptor.MethodInfo;
			return true;
		}

        if (apiDescription.ActionDescriptor?.EndpointMetadata is not null)
        {
            methodInfo = apiDescription.ActionDescriptor.EndpointMetadata
                .OfType<MethodInfo>()
                .FirstOrDefault();

            return methodInfo != null;
        }

		methodInfo = null;
		return false;
	}

	public static IEnumerable<object> GetCustomAttributes( this ApiDescription apiDescription, out Type? declaringType )
	{
		declaringType = null;

		if( !apiDescription.TryGetMethodInfo( out MethodInfo? methodInfo ) )
			return Enumerable.Empty<object>();

		declaringType = methodInfo!.DeclaringType;

		return methodInfo.GetCustomAttributes( true );

	}
}