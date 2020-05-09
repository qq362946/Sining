using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Sining.Config
{
	public partial class SceneTypeConfig : IConfig, IObject
	{
		public int Id { get; set; }
		[BsonDefaultValue("")]
		public string SceneTypeName { get; set; }
	}
}
