using System.Threading.Tasks;
using LibraProgramming.Grains.Interfaces.Entities;
using Orleans;

namespace LibraProgramming.Grains.Interfaces.Grains
{
    public interface IChatRoom : IGrainWithStringKey
    {
        Task AdmitUserAsync(IChatUser user);

        Task QuitUserAsync(IChatUser user);

        Task PublishMessageAsync(IChatUser user, UserMessage message);
    }
}