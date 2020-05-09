using Sining;
using Sining.Message;
using Sining.Module;
using Sining.Network;

namespace Server.Model.Handler
{
    [MessageSystem]
    public class ServerStartFinishedHandler : MessageHandler<ServerStartFinished>
    {
        protected override async STask Run(Session session, ServerStartFinished message)
        {
            ServerHelper.STaskCompletionSource.SetResult();

            await STask.CompletedTask;
        }
    }
}