using System.Collections.Generic;

namespace Sining.Module
{
    public class User : Component
    {
        public HashSet<UserWallet> Wallets = new HashSet<UserWallet>();     // 钱包               // 钱包地址
        public string Email;                                                // 邮箱
        public string Mobile;                                               // 手机号
        public string Nick;                                                 // 昵称
        public string MnemonicKey;                                          // 助记词
        public string PrivateKey;                                           // 私钥
        public string PayPassword;                                          // 支付密码
        public string Password;                                             // 登录密码
        public string Status;                                               // 状态,0禁用，1启用
        public long ParentId;                                               // 上级ID
        public string ParentMobile;                                         // 上级手机号
        public string GoogleKey;                                            // GoogleKey
        public bool IsBindGoogle;                                           // 是否绑定Google
        public bool IsBindMobile;                                           // 是否绑定手机号
        public int AuthStatus;                                              // 0未认证 1身份证认证 2高级认证 3全部认证
        public long RegisterTime;                                           // 注册时间
        public string RegisterIp;                                           // 注册IP
        public string Root;                                                 // 邀请关系链
        public string InviteCode;                                           // 邀请码
        public string AuthName;                                             // 认证姓名
        public string IdCardNo;                                             // 身份证号码
        public string Token;                                                // Session
        public string Head;                                                 // 头像
        public string Country;                                              // 国家ID

        public override void Dispose()
        {
            if (IsDispose)
            {
                return;
            }

            base.Dispose();

            Wallets.Clear();
            Email = null;
            Mobile = null;
            Nick = null;
            MnemonicKey = null;
            PrivateKey = null;
            PayPassword = null;
            Password = null;
            Status = null;
            ParentId = 0;
            ParentMobile = null;
            GoogleKey = null;
            IsBindGoogle = false;
            IsBindMobile = false;
            AuthStatus = 0;
            RegisterTime = 0;
            RegisterIp = null;
            Root = null;
            InviteCode = null;
            AuthName = null;
            IdCardNo = null;
            Token = null;
            Head = null;
            Country = null;
        }
    }
}