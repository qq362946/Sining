namespace Sining
{
    public class ErrorCode
    {
        public const string MoacConnectionFailed = "MoacConnectionFailed";
        public const string MoacInsufficientBalance = "MoacInsufficientBalance";
        public const string MoacOtherError = "MoacOtherError";
        public const string AddWithdrawMoneyLessZero = "AddWithdrawMoneyLessZero"; // 提现的金额不能小于0
        public const string AddWithdrawNotFoundUser = "AddWithdrawNotFoundUser"; // 提现申请时无法找到指定用户
        public const string AddWithdrawNotFoundFeeWallet = "AddWithdrawNotFoundFeeWallet"; // 提现申请时无法找到指定手续费钱包
        public const string AddWithdrawMinWithDraw = "AddWithdrawMinWithDraw"; // 小于配表里的最低提现金额
        public const string AddWithdrawMinWithDrawFee = "AddWithdrawMinWithDrawFee"; // 小于小于配表的最低提现费用
        
        public const int CreateUserError = 10000; // 创建角色Handler发生错误
        public const int CreateUserParamIsNull = 10001; // 创建角色传递的参数部分为空
    }
}