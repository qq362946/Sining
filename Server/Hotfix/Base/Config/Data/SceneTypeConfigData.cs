namespace Sining.Config
{
	[Config]
	public partial class SceneTypeConfigData : AConfig<SceneTypeConfig>
	{
		public static SceneTypeConfigData Instance { get; private set; }

		public SceneTypeConfigData()
		{
			Instance = this;
		}
	}
}
