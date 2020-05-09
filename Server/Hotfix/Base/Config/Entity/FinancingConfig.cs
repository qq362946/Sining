using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Sining.Config
{
	public partial class FinancingConfig : IConfig, IObject
	{
		public int Id { get; set; }
		[BsonDefaultValue("")]
		public string call_index { get; set; }
		[BsonDefaultValue("")]
		public string title { get; set; }
	}
}
