using Orleans;
using Orleans.Providers;

namespace GrainInterfaces;

public interface IProducerGrain : IGrainWithStringKey,Orleans.Providers.IMemoryMessageBodySerializer
{
    Task StartProducing(string ns, Guid key);

    Task StopProducing();
}
