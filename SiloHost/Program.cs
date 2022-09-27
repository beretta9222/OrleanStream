using Common;
using GrainInterfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Hosting;
using Orleans.Providers;

try
{
    var host = new HostBuilder()
        .UseOrleans(ConfigureSilo)
        .ConfigureLogging(logging => logging.AddConsole())
        .Build();

    await host.RunAsync();

    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex);
    return 1;
}

static void ConfigureSilo(ISiloBuilder siloBuilder)
{
    siloBuilder
        .UseLocalhostClustering(serviceId: Constants.ServiceId, clusterId: Constants.ServiceId).AddSimpleMessageStreamProvider(Constants.StreamProvider); //AddMemoryStreams<GrainInterfaces.IProducerGrain>(Constants.StreamProvider);
}
