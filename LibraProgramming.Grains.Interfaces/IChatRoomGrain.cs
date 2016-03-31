using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;

namespace LibraProgramming.Grains.Interfaces
{
    public interface IChatRoomGrain : IGrainWithStringKey
    {
        Task<IList<RoomMessage>> GetMessagesAsync(int startFromId);

        Task AddUser(Guid userId);

        Task<bool> PublishMessageAsync(Guid userId, PublishMessage message);
    }
}