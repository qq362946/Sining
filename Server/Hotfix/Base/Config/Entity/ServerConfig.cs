using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Sining.Config
{
	public partial class ServerConfig : IConfig, IObject
	{
		public int Id { get; set; }
		[BsonDefaultValue(0)]
		public int ServerType { get; set; }
		[BsonDefaultValue("")]
		public string InnerIP { get; set; }
		[BsonDefaultValue(0)]
		public int InnerPort { get; set; }
		[BsonDefaultValue("")]
		public string OuterIP { get; set; }
	}
}
