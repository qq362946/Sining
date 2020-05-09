using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Sining.Config
{
	public partial class CoinNodeConfig : IConfig, IObject
	{
		public int Id { get; set; }
		[BsonDefaultValue(0)]
		public int NodeId { get; set; }
		[BsonDefaultValue("")]
		public string RPCServer { get; set; }
		[BsonDefaultValue("")]
		public string RPCUser { get; set; }
		[BsonDefaultValue("")]
		public string RPCPassword { get; set; }
	}
}
