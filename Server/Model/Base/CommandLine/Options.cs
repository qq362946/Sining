using CommandLine;

namespace Sining
{
    public class Options : Component
    {
        [Option("Server", Required = false, Default = 0)]
        public int Server { get; set; }
    }
}