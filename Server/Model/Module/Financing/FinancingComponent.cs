namespace Sining.Module
{
    public class FinancingComponent : Component
    {
        public static FinancingComponent Instance;
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