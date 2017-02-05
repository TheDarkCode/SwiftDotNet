[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Swift.Net.WebAPI.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(Swift.Net.WebAPI.App_Start.NinjectWebCommon), "Stop")]

namespace Swift.Net.WebAPI.App_Start
{
    using System;
    using System.Web;

    using Swift.Net.WebAPI.Repositories;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Web.Common;
    using Swift.Net.WebAPI.Controllers.api;
    using Swift.Net.WebAPI.Repositories;
    using Microsoft.Azure.Documents.Client;

    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

            RegisterServices(kernel);

            System.Web.Http.GlobalConfiguration.Configuration.DependencyResolver = new Ninject.WebApi.DependencyResolver.NinjectDependencyResolver(kernel);

            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            var authkey = new System.Security.SecureString();

            foreach (char c in "{YOUR_AUTH_KEY}")
            {
                authkey.AppendChar(c);
            }

            // Bind a DocumentClient for injection.
            kernel.Bind<DocumentClient>().ToSelf().WithConstructorArgument("serviceEndpoint", new Uri("https:/{YOUR_ENDPOINT}.documents.azure.com:443"))
                .WithConstructorArgument("authKey", authkey)
                .WithConstructorArgument("connectionPolicy", new ConnectionPolicy
                {
                    ConnectionMode = ConnectionMode.Direct,
                    ConnectionProtocol = Protocol.Tcp,
                    MaxConnectionLimit = 1000,
                    RequestTimeout = new TimeSpan(1, 0, 0),
                    RetryOptions = new RetryOptions
                    {
                        MaxRetryAttemptsOnThrottledRequests = 20, // Variable, 10-50. Client automatically will wait before retrying.
                        MaxRetryWaitTimeInSeconds = 100 // Most browsers or intermediates like Cloudflare will max out at around 100 by default. Only set higher if you need to ensure operation completes. Otherwise responsiveness is at issue.
                    }
                })
                .WithConstructorArgument("desiredConsistencyLevel", default(Microsoft.Azure.Documents.ConsistencyLevel?));
            
            // Bind client to repositories, and repositories to kernel as singletons.
            kernel.Bind<IAnalyticRepository>().To<AnalyticRepository>().InSingletonScope().WithConstructorArgument("client", kernel.Get<DocumentClient>());

            // Alternative without reused client.
            //kernel.Bind<IAnalyticRepository>().To<AnalyticRepository>().InRequestScope(); // Or: .InSingletonScope();

            // Example injecting repositories into other services.
            //kernel.Bind<IUpdateService>().To<UpdateService>().InSingletonScope()
            //    .WithConstructorArgument("analyticsRepo", kernel.Get<IAnalyticRepository>());
        }
    }
}
