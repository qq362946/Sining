using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Sining.Config
{
	public partial class ServerTypeConfig : IConfig, IObject
	{
		public int Id { get; set; }
		[BsonDefaultValue("")]
		public string ServerTypeName { get; set; }
	}
}
