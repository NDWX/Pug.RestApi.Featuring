using System;

namespace Pug.RestApi.Featuring;

[AttributeUsage( AttributeTargets.Class | AttributeTargets.Method)]
public class ApiSubFeatureAttribute : Attribute
{
	public string Feature { get; }
	public string Version { get; }
	public string Name { get; }

	public ApiSubFeatureAttribute( string feature, string version, string name )
	{
		if( string.IsNullOrWhiteSpace( feature ) )
			throw new ArgumentException( "Value cannot be null or whitespace.", nameof(feature) );

		if( string.IsNullOrWhiteSpace( name ) )
			throw new ArgumentException( "Value cannot be null or whitespace.", nameof(name) );

		Feature = feature.Trim('/');
		Version = version;
		Name = name.TrimEnd('/');
	}
}