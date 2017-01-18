using System.Threading.Tasks;
using Orleans;

namespace FSM.Orleans
{
    public interface IStateMachineGrain<TStateMachineData, in TStateMachineMessage> : IGrainWithGuidKey
    {
        Task<TStateMachineData> ProcessMessage(TStateMachineMessage message);
    }
}