using System.Threading.Tasks;
using LibraProgramming.Grains.Interfaces.Entities;
using Orleans;

namespace LibraProgramming.Grains.Interfaces.Grains
{
    public interface IChatRoom : IGrainWithStringKey
    {
        Task JoinAsync(IChatUser user);

        Task LeaveAsync(IChatUser user);

        Task PublishAsync(IChatUser user, UserMessage message);
    }
}