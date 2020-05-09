namespace Sining.Module
{
    public struct CoinSendResult
    {
        public int Status; // -1:余额不足  -2:手续费不足  -3:转帐成功
        public string Msg;
        public string Hash;
    }

    public struct CoinHashResult
    {
        public int Status;
        public string Confirms;
        public string Msg;
    }

    public struct TransferListResult
    {
        public string From;
        public string To;
        public string Hash;
        public decimal Money;
        public string Contact;
        public string Block;
        public string Time;
    }
}