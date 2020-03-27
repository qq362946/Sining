using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Sining.Config
{
	public partial class ZoneConfig : IConfig, IObject
	{
		public int Id { get; set; }
		[BsonDefaultValue("")]
		public string DbConnection { get; set; }
		[BsonDefaultValue("")]
		public string DbName { get; set; }
	}
}
