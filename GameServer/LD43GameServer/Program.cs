using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LD43GameServer
{
    public class Program
    {
        public static Config Config;
        public static GameServer GameServer;
        public static void Main(string[] args)
        {
            using (var fs = new FileStream("./config.json", FileMode.Open))
            using (var sr = new StreamReader(fs))
            using (var jr = new JsonTextReader(sr))
            {
                var serializer = new JsonSerializer();
                Config = serializer.Deserialize<Config>(jr);
                
            }
            ServerLog.SetLogFile(Config.LogFilePath);
            GameServer = new GameServer();
            GameServer.Start();
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls($"http://{Config.Host}:{Config.Port}/")
                .UseStartup<Startup>();
    }
}
