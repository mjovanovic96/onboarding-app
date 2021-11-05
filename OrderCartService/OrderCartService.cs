using System;
using System.Collections.Generic;
using System.Fabric;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Data;
using System.Fabric.Description;

namespace OrderCartService
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    public sealed class OrderCartService : StatefulService
    {
        public OrderCartService(StatefulServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new ServiceReplicaListener[]
            {
                new ServiceReplicaListener(serviceContext =>
                    new KestrelCommunicationListener(serviceContext, (url, listener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                        return new WebHostBuilder()
                                    .UseKestrel()
                                    .ConfigureServices(
                                        services => services
                                            .AddSingleton<StatefulServiceContext>(serviceContext)
                                            .AddSingleton<IReliableStateManager>(this.StateManager)
                                            .AddSingleton<OrderCartService>(this))
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseStartup<Startup>()
                                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.UseUniqueServiceUrl)
                                    .UseUrls(url)
                                    .Build();
                    }))
            };
        }

        public void setCountCartsMetric(int value)
        {
            this.Partition.ReportLoad(new List<LoadMetric> { new LoadMetric("CartsCount", value) });
        }

        private void DefinePolicies()
        {
            Uri orderCartServiceName = GetOrderCartServiceName(Context);
            FabricClient fabricClient = new FabricClient();

            AverageServiceLoadScalingTrigger trigger = new AverageServiceLoadScalingTrigger
            {
                MetricName = "CartsCount",
                ScaleInterval = TimeSpan.FromMinutes(1),
                LowerLoadThreshold = 3,
                UpperLoadThreshold = 10
            };

            AddRemoveIncrementalNamedPartitionScalingMechanism mechanism = new AddRemoveIncrementalNamedPartitionScalingMechanism
            {
                MaxPartitionCount = 5,
                MinPartitionCount = 1,
                ScaleIncrement = 1
            };
            
            ScalingPolicyDescription policy = new ScalingPolicyDescription(mechanism, trigger);
            StatefulServiceUpdateDescription updateServiceDescription = new StatefulServiceUpdateDescription();

            if (updateServiceDescription.ScalingPolicies == null)
            {
                updateServiceDescription.ScalingPolicies = new List<ScalingPolicyDescription>();
            }
            updateServiceDescription.ScalingPolicies.Add(policy);

            fabricClient.ServiceManager.UpdateServiceAsync(orderCartServiceName, updateServiceDescription);
        }

        internal static Uri GetOrderCartServiceName(ServiceContext context)
        {
            return new Uri($"{context.CodePackageActivationContext.ApplicationName}/OrderCartService");
        }
    }
}
