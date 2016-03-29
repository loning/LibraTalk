using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraProgramming.Grains.Interfaces;
using Orleans;
using Orleans.Concurrency;
using Orleans.Providers;
using Orleans.Runtime;

namespace LibraProgramming.Grains.Implementation
{
    public class CharRoomState : GrainState
    {
        public string Id
        {
            get;
            set;
        }

        public Queue<ChatMessage> Messages
        {
            get;
            set;
        }
    }

    [Reentrant]
    [StorageProvider(ProviderName = "MemoryStore")]
    public class ChatRoom : Grain<CharRoomState>, IChatRoom
    {
        private List<Guid> users;
        private Logger logger;
         
        public Task<IList<ChatMessage>> GetMessages()
        {
            var messages = State.Messages.ToList();
            return Task.FromResult<IList<ChatMessage>>(messages);
        }

        public Task AddUser(Guid userId)
        {
            users.Add(userId);
            logger.Info($"LibraProgramming.Grains.Implementation.ChatRoom.AddUser | Room: {State.Id} user: {userId}");
            return TaskDone.Done;
        }

        public Task PublishMessage(Guid userId, string text)
        {
            if (!users.Contains(userId))
            {
                return TaskDone.Done;
            }

            var message = new ChatMessage
            {
                Id = State.Messages.Count,
                PublisherId = userId,
                Date = DateTime.UtcNow,
                Text = text
            };

            State.Messages.Enqueue(message);

            return WriteStateAsync();
        }

        /// <summary>
        /// This method is called at the end of the process of activating a grain.
        /// It is called before any messages have been dispatched to the grain.
        /// For grains with declared persistent state, this method is called after the State property has been populated.
        /// </summary>
        public override Task OnActivateAsync()
        {
            State.Id = this.GetPrimaryKeyString();

            if (null == State.Messages)
            {
                State.Messages = new Queue<ChatMessage>();
            }

            users = new List<Guid>();
            logger = GetLogger(nameof(ChatRoom));

            return base.OnActivateAsync();
        }
    }
}