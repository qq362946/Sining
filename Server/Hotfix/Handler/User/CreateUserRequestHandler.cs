using System;
using Sining.Message;
using Sining.Module;
using Sining.Network;

namespace Sining.Handler.User
{
    [MessageSystem]
    public class CreateUserRequestHandler : RPCSceneMessageHandler<CreateUserRequest, CreateUserResponse>
    {
        protected override async STask Run(Session session, Scene scene, CreateUserRequest request, CreateUserResponse response, Action reply)
        {
            if (request.User == null)
            {
                response.ErrorCode = ErrorCode.CreateUserParamIsNull;

                return;
            }

            try
            {
                await scene.GetComponent<UserManageComponent>().CreateUser(request.User);
            }
            catch (Exception e)
            {
                Log.Error(e);

                response.ErrorCode = ErrorCode.CreateUserError;
            }
        }
    }
}