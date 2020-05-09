namespace Sining.Module
{
    public class StartMultiProgressComponent : Component
    {
        public override void Dispose()
        {
            if (IsDispose)
            {
                return;
            }
            
            base.Dispose();
        }
    }
}