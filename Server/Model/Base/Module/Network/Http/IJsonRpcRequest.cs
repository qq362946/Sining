namespace Sining.Network
{
    public interface IJsonRpcRequest
    {
        void Init(string method, int id, params object[] @params);
    }
}