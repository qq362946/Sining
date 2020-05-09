using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Sining.Config
{
	public partial class CoinConfig : IConfig, IObject
	{
		public int Id { get; set; }
		[BsonDefaultValue("")]
		public string Name { get; set; }
		[BsonDefaultValue("")]
		public string EnName { get; set; }
		[BsonDefaultValue("")]
		public string FullName { get; set; }
		[BsonDefaultValue("")]
		public string Logo { get; set; }
		[BsonDefaultValue(0L)]
		public long AddTime { get; set; }
		[BsonDefaultValue(0)]
		public decimal Price { get; set; }
		[BsonDefaultValue("")]
		public string Description { get; set; }
		[BsonDefaultValue("")]
		public string PublishTime { get; set; }
		[BsonDefaultValue("")]
		public string PublishNum { get; set; }
		[BsonDefaultValue("")]
		public string CirculationNum { get; set; }
		[BsonDefaultValue("")]
		public string CrowdPrice { get; set; }
		[BsonDefaultValue("")]
		public string WhitePaper { get; set; }
		[BsonDefaultValue("")]
		public string WebUrl { get; set; }
		[BsonDefaultValue("")]
		public string Browser { get; set; }
		[BsonDefaultValue(0)]
		public decimal WithDrawFee { get; set; }
		[BsonDefaultValue(0)]
		public decimal MinWithDrawFee { get; set; }
		[BsonDefaultValue(0)]
		public int IsWithDraw { get; set; }
		[BsonDefaultValue(0)]
		public int IsRecharge { get; set; }
		[BsonDefaultValue(0)]
		public int IsAutoWithDraw { get; set; }
		[BsonDefaultValue(0)]
		public int Status { get; set; }
		[BsonDefaultValue("")]
		public string RPCServer { get; set; }
		[BsonDefaultValue("")]
		public string RPCUser { get; set; }
		[BsonDefaultValue("")]
		public string RPCPassword { get; set; }
		[BsonDefaultValue(0)]
		public decimal MinWithDraw { get; set; }
		[BsonDefaultValue(0)]
		public decimal MinRecharge { get; set; }
		[BsonDefaultValue(0)]
		public decimal MaxWithDraw { get; set; }
		[BsonDefaultValue(0)]
		public int Fixed { get; set; }
		[BsonDefaultValue("")]
		public string RechargeInfo { get; set; }
		[BsonDefaultValue("")]
		public string WithDrawInfo { get; set; }
		[BsonDefaultValue(0)]
		public int Confirms { get; set; }
		[BsonDefaultValue("")]
		public string WalletUrl { get; set; }
		[BsonDefaultValue("")]
		public string MobileWalletUrl { get; set; }
		[BsonDefaultValue(0)]
		public int Protocol { get; set; }
		[BsonDefaultValue(0)]
		public int Sort { get; set; }
		[BsonDefaultValue("")]
		public string MainAddress { get; set; }
		[BsonDefaultValue("")]
		public string Ext { get; set; }
		[BsonDefaultValue(0)]
		public int Decimals { get; set; }
		[BsonDefaultValue(0)]
		public int NodeId { get; set; }
	}
}
