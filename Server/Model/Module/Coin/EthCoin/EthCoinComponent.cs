using Nethereum.Hex.HexConvertors;
using Nethereum.Web3;

namespace Sining.Module
{
    public class EthCoinComponent : Component
    {
        public Web3 Web3;
        public string Url;
        public string NodeName;
        public string ContractAddress;
        public int JsonId;
        public readonly HexBigIntegerBigEndianConvertor BigConvertor = new HexBigIntegerBigEndianConvertor();

        public override void Dispose()
        {
            if (IsDispose)
            {
                return;
            }
            
            base.Dispose();

            Url = null;
            NodeName = null;
            Web3 = null;
            ContractAddress = null;
            JsonId = 0;
        }
    }
}