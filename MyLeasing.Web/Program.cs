﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MyLeasing.Web.Data;

namespace MyLeasing.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }


    //public class Program
    //{
    //    public static void Main(string[] args)
    //    {
    //        IWebHost host = CreateWebHostBuilder(args).Build();
    //        RunSeeding(host);
    //        host.Run();
    //    }

    //    private static void RunSeeding(IWebHost host)
    //    {
    //        IServiceScopeFactory scopeFactory = host.Services.GetService<IServiceScopeFactory>();
    //        using (IServiceScope scope = scopeFactory.CreateScope())
    //        {
    //            SeedDb seeder = scope.ServiceProvider.GetService<SeedDb>();
    //            seeder.SeedAsync().Wait();
    //        }
    //    }

    //    public static IWebHostBuilder CreateWebHostBuilder(string[] args)
    //    {
    //        return WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();
    //    }
    //}
}
