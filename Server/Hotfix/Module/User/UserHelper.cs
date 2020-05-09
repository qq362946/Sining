using Sining.Config;
using Sining.Message;
using Sining.Tools;

namespace Sining.Module
{
    public static class UserHelper
    {
        /// <summary>
        /// 创建一个账号
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static async STask<bool> Create(Scene scene, User user)
        {
            var userIndex = (int) user.Id % ServerConfigData.UserServers.Count;

            var response = await new CreateUserRequest
            {
                SceneId = (int) SceneType.UserScene,
                User = user
            }.Call<CreateUserResponse>(scene, ServerConfigData.UserServers[userIndex].Id);

            return response.ErrorCode == 0;
        }
    }
}