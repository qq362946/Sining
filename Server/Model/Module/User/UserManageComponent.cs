namespace Sining.Module
{
    public class UserManageComponent : Component
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