using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace Sining.Module
{
    [Function("balanceOf", "uint256")]
    public class BalanceOfFunction : FunctionMessage
    {
        [Parameter("address", "_owner", 1)]
        public string Owner { get; set; }

        public void Init(string owner)
        {
            Owner = owner;
        }
    }
}