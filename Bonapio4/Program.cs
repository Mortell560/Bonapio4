using Bonapio4Database;
using Discord;
using Discord.Addons.Hosting;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;



namespace Bonapio4
{
    class Program
    {
        private static bool IsRunning = true;
        public static event EventHandler OnShutdown;

        static async Task Main()
        {
            IHostBuilder builder = new HostBuilder()
                .ConfigureAppConfiguration(x =>
                {
                    IConfigurationRoot configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("config.json", false, true)
                        .Build();

                    x.AddConfiguration(configuration);
                })
                .ConfigureLogging(x =>
                {
                    x.AddConsole();
                    x.SetMinimumLevel(LogLevel.Debug);
                })
                .ConfigureDiscordHost<DiscordSocketClient>((context, config) =>
                {
                    config.SocketConfig = new DiscordSocketConfig
                    {
                        LogLevel = LogSeverity.Verbose,
                        AlwaysDownloadUsers = true,
                        MessageCacheSize = 12,
                    };

                    config.Token = context.Configuration["token"];
                })
                .UseCommandService((context, config) =>
                {
                    config = new CommandServiceConfig()
                    {
                        CaseSensitiveCommands = false,
                        LogLevel = LogSeverity.Verbose
                    };
                })
                .ConfigureServices((context, services) =>
                {
                    services
                    .AddHostedService<CommandHandler>()
                    .AddDbContext<StudentContext>()
                    .AddSingleton<InteractiveService>()
                    .AddSingleton<Servers>()
                    .AddSingleton<Clubs>()
                    .AddSingleton<Profiles>()
                    .AddSingleton<Students>();
                })
                .UseConsoleLifetime();

            IHost host = builder.Build();
            using (host)
            {
                await host.StartAsync();

                while (IsRunning)
                {
                    await Task.Delay(200);
                }
                await host.StopAsync();
            }
            
            
        }

        /// <summary>
        /// Get bot's configuration (token + prefix) from the config.json file 
        /// </summary>
        /// <returns>ConfigJson</returns>
        public async Task<ConfigJson> GetConfigAsync()
        {
            string json = string.Empty;
            using (FileStream fs = File.OpenRead("config.json"))
            using (StreamReader sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync();

            return JsonConvert.DeserializeObject<ConfigJson>(json);
        }

        public static void EndProgram()
        {
            OnShutdown?.Invoke(null, null);
            IsRunning = false;
        }
    }
}
