using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;

namespace LibraProgramming.Grains.Interfaces
{
    public interface IChatRoom : IGrainWithStringKey
    {
        Task<IList<ChatMessage>> GetMessages();

        Task AddUser(Guid userId);

        Task PublishMessage(Guid userId, string text);
    }
}