using Sining.Tools;

namespace Sining.Module
{
    public static class CbeConfigComponentSystem
    {
        public static async STask Save(this CbeConfigComponent self)
        {
            await SingleDataHeler.Save(self);
        }

        public static async STask Save(this CbeConfigComponent self, object transactionSession)
        {
            await SingleDataHeler.Save(transactionSession, self);
        }
    }
}