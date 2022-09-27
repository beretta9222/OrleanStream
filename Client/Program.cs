using GrainInterfaces;
using Orleans;
using Microsoft.Extensions.Logging;
using Orleans.Streams;
using Orleans.Hosting;
using Common;

try
{
    var client = new ClientBuilder()
        .UseLocalhostClustering(serviceId: Constants.ServiceId, clusterId: Constants.ClusterId)
        .ConfigureLogging(logging => logging.AddConsole())
        .Build();

    await client.Connect(CreateRetryFilter());
    Console.WriteLine("Connect!");

    var key = Guid.NewGuid();
    var producer = client.GetGrain<IProducerGrain>("my-producer");
    await producer.StartProducing(Constants.StreamNamespace, key);
    // Получение потока
    var stream = client
        .GetStreamProvider(Constants.StreamProvider)
        .GetStream<int>(key, Constants.StreamNamespace);
    await stream.SubscribeAsync(OnNextAsync);

    await Task.Delay(TimeSpan.FromSeconds(15));

    await producer.StopProducing();

    Console.ReadKey();
    return 0;
}
catch (Exception e)
{
    Console.WriteLine(e);
    Console.ReadKey();
    return 1;
}

static Task OnNextAsync(int item, StreamSequenceToken? token = null)
{
    Console.WriteLine("OnNextAsync: item: {0}, token = {1}", item, token);
    return Task.CompletedTask;
}

static Func<Exception, Task<bool>> CreateRetryFilter(int maxAttempts = 5)
{
    var attempt = 0;
    return RetryFilter;

    async Task<bool> RetryFilter(Exception exception)
    {
        attempt++;
        Console.WriteLine($"Cluster client attempt {attempt} of {maxAttempts} failed to connect to cluster.  Exception: {exception}");
        if (attempt > maxAttempts)
        {
            return false;
        }

        await Task.Delay(TimeSpan.FromSeconds(4));
        return true;
    }
}