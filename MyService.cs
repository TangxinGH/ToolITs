
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

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

        internal string? getConnstr()
        {
            string? connstr = "";
            var db_type = getDbType();

            Func<string,string> sqlite = x => {
               
                if (Convert.ToBoolean(configuration.GetSection(x).GetSection("realtive").Value))
                {    //var folder = Environment.SpecialFolder.LocalApplicationData;
                    //var path = Environment.GetFolderPath(folder);
                    var path = Environment.CurrentDirectory;
                    //AppDomain.CurrentDomain.BaseDirectory

                    var value = configuration.GetSection(x).GetSection("value").Value;
                    var pattern = new Regex( @"(Data\s?Source\s?=)(.*?)(;.*)");
                    var result = pattern.Replace(value, m =>
                     m.Groups[1].Value + Path.Join(path, m.Groups[2].Value.Trim() ) + m.Groups[3].Value);
                    return result;
                }
                else
                {
                    return configuration.GetSection(x).GetSection("value").Value;
                  
                }
            };
            
             

            switch (db_type)
            {
                case "SqliteConnstr": connstr = sqlite(db_type);
                    break;
                default: connstr = configuration.GetSection(db_type).Value;
                    break;
            }
            return connstr;
        }
        /// <summary>
        /// get db type from configuration
        /// </summary>
        /// <returns></returns>
        internal string? getDbType() => configuration.GetSection("DB_TYPE").Value;


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