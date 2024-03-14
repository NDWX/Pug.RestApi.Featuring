using System;

namespace Pug.RestApi.Featuring;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ApiFeatureAttribute : Attribute
{
	public string Name { get; }
	public string Version { get; }

	public ApiFeatureAttribute( string name, string version )
	{
		if( string.IsNullOrWhiteSpace( name ) )
			throw new ArgumentException( "Value cannot be null or whitespace.", nameof(name) );

		Name = name.TrimEnd('/');
		Version = version;
	}
}