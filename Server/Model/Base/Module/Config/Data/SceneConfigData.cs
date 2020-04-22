namespace Sining.Config
{
	[Config]
	public partial class SceneConfigData : AConfig<SceneConfig>
	{
		public static SceneConfigData Instance { get; private set; }

		public SceneConfigData()
		{
			Instance = this;
		}
	}
}
