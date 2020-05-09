namespace Sining.Module
{
    public class CoinRechargeComponent : Component
    {
        public static CoinRechargeComponent Instance;
        public MongoDBComponent DbComponent;

        public override void Dispose()
        {
            if (IsDispose)
            {
                return;
            }
            
            base.Dispose();

            DbComponent = null;
            Instance = null;
        }
    }
}