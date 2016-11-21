using System;
using System.Threading.Tasks;
using Orleans;

namespace LibraProgramming.Grains.Interfaces.Grains
{
    public interface IActiveChatRooms : IGrainWithIntegerKey
    {
        Task<Guid> GetRoomAsync(string name);
    }
}