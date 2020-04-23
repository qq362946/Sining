using CommandLine;

namespace Sining
{
    public class Options : Component
    {
        [Option("Server", Required = true, Default = 0)]
        public int Server { get; set; }
        
        [Option("Single", Required = false, Default = 0)]
        public int Single { get; set; }
    }
}