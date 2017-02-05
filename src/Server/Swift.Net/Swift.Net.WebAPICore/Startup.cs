using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swift.Net.DocumentDB.Repositories;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Azure.Documents.Client;
using System.Net.Http;

namespace Swift.Net.Core
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            // Add Forwarded Headers Options - Ensures Reading Proper IP Address of Requests
            services.Configure<ForwardedHeadersOptions>(option => { option.ForwardedHeaders = ForwardedHeaders.XForward‌​edFor; });

            // Add Global HttpClient Singleton
            //services.AddSingleton(typeof(HttpClient), new HttpClient());
            // Same thing as above but written another way:
            //services.Add(new ServiceDescriptor(typeof(HttpClient), p => new HttpClient(), ServiceLifetime.Singleton));

            /// Add DocumentDB Singletons

            // Example with Singleton DocumentClient;
            services.AddSingleton(typeof(DocumentClient), new DocumentClient(new Uri("https://{YOUR_DOCUMENT_ENDPOINT}.documents.azure.com:443"), authKeyOrResourceToken: "{YOUR_AUTH_KEY}"));

            // Service provider to resolve singleton DocumentClient in Configuration - could be moved to factory or using Autofac.
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            services.AddSingleton(typeof(IAnalyticRepository), new AnalyticRepository(serviceProvider.GetService<DocumentClient>(), "{YOUR PARTITION KEY VALUE}", "{YOUR_DATABASE}", "{YOUR_COLLECTION}"));


            // Example with Default Values
            services.AddSingleton<IAnalyticRepository, AnalyticRepository>();
            // Example with All Arguments
            services.AddSingleton(typeof(IAnalyticRepository), new AnalyticRepository("{YOUR PARTITION KEY VALUE}", "{YOUR_DATABASE}", "{YOUR_COLLECTION}", "https://{YOUR_DOCUMENT_ENDPOINT}.documents.azure.com:443", "{YOUR_AUTH_KEY}"));
            // Example with Named Arguments
            services.AddSingleton(typeof(IAnalyticRepository), new AnalyticRepository(endpoint:"https://{YOUR_DOCUMENT_ENDPOINT}.documents.azure.com:443",authKey:"{YOUR_AUTH_KEY}"));

            // Add Redis Cache
            services.AddDistributedRedisCache(options => {
                options.Configuration = "synctimeout=2000,{YOUR_ENDPOINT}.redis.cache.windows.net:6380,password={YOUR_PASSWORD},ssl=True,abortConnect=False";
                options.InstanceName = "Swift.Net";
            });

            // Add framework services
            services.AddMvc();

            // Inject implementation of ISwaggerProvider
            // More info: https://docs.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger
            //services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
