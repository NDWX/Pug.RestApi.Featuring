using System.Runtime.Serialization;

namespace Pug.RestApi.Featuring;

public record DeclaredFeature
{
	[DataMember( IsRequired = true )]
	public string Name
	{
		get;
#if NETCORE6_0_OR_GREATER
		init;
#else
		set;
#endif
	} = null!;

	[DataMember(IsRequired = true)]
	public string Version
	{
		get;
#if NETCORE6_0_OR_GREATER
		init;
#else
		set;
#endif
	} = null!;
}