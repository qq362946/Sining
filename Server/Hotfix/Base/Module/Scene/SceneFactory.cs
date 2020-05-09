using System;
using Sining.Config;
using Sining.Module;
using Sining.Tools;

namespace Sining
{
    public static class SceneFactory
    {
        public static async STask Create(Scene scene)
        {
            switch ((SceneType) scene.SceneId)
            {
                case SceneType.ServerManageScene:
                    // 添加单进程组件（方便调试、所有服务都是再一个进程处理）。
                    scene.AddComponent<StartSingleProcessComponent>();
                    // 添加多进程组件（用于发布到服务器时使用）。
                    // scene.AddComponent<StartMultiProgressComponent>();
                    break;
                case SceneType.UserScene:
                    // 用户管理组件
                    scene.AddComponent<UserManageComponent>();
                    break;
                case SceneType.GateUserScene:
                    var user = ComponentFactory.Create<User>(scene);
                    Log.Debug(await UserHelper.Create(scene,user));
                    break;
                case SceneType.CoinNodeScene:
                    break;
                case SceneType.FinanceScene:
                    break;
                case SceneType.None:
                    throw new Exception($"No SceneType found for {scene.SceneId}");
                default:
                    throw new Exception($"No SceneType found for {scene.SceneId}");
            }

            // switch ((SceneType)scene.SceneType)
            // {
            //     case SceneType.UserScene:
            //         // // 用户管理组件
            //         // scene.AddComponent<UserManageComponent>();
            //         // // 财务账单组件
            //         // await scene.AddComponent<FinancingComponent>().Init();
            //         // // 币充值组件
            //         // await scene.AddComponent<CoinRechargeComponent>().Init();
            //         // // CBE组件
            //         // await scene.AddComponent<CbeCoinComponent>().Init();
            //         // // 提现管理组件
            //         // await scene.AddComponent<WithdrawManageComponent>().Init();
            //
            //      
            //         
            //         // var withdrawInfo = WithdrawManageComponent.Instance.Create(
            //         //     2233823394298,
            //         //     "0x92655ed5c13ed963dda2a845df33e4f862566ff0",
            //         //     10,
            //         //     CoinConfigType.CBE, 0, 0, CoinConfigType.CBE);
            //         
            //         // await WithdrawManageComponent.Instance.AddWithdraw(withdrawInfo);
            //        
            //         // Log.Debug(await WithdrawManageComponent.Instance.Approval( withdrawInfo.Id));
            //         //
            //        
            //         // var user = ComponentFactory.Create<User>(scene);
            //         //
            //         // user.Email = "123";
            //         // user.Mobile = "123";
            //         // user.Password = "123";
            //         //
            //         // await UserManageComponent.Instance.CreateUser(user);
            //         // Log.Debug("2222");
            //         break;
            //     default:
            //         throw new Exception($"No SceneType found for {(int) scene.SceneType}");
            // }

            await STask.CompletedTask;
        }
    }
}