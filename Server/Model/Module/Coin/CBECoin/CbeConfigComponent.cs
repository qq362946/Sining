namespace Sining.Module
{
    public class CbeConfigComponent : Component
    {
        public long Block;

        public override void Dispose()
        {
            if (IsDispose)
            {
                return;
            }
            
            base.Dispose();
            Block = 0;
        }
    }
}