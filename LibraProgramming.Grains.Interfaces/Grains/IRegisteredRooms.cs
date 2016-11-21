using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;

namespace LibraProgramming.Grains.Interfaces.Grains
{
    public interface IRegisteredRooms : IGrainWithStringKey
    {
        Task<IReadOnlyDictionary<string, Guid>> GetRoomsAsync();
    }
}