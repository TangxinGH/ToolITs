
using Microsoft.Extensions.Configuration;

namespace ToolITs
{
    internal class MyService
    {
        private readonly IConfiguration configuration;

        public MyService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public List<tns_oracle> GetTns() => configuration.GetSection("TNS").Get<List<tns_oracle>>();

    }

    internal class tns_oracle
    {
        public string Name { get; set; }
        public string Connectionstring { get; set; }
        //string Ip { get; set; }
        //string Port { get; set; }
        //string Service { get; set; }
        //string User { get; set; }
        //string Pass { get; set; }
    }
}