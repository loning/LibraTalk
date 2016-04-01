using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraProgramming.Grains.Interfaces;
using Orleans;
using Orleans.Concurrency;
using Orleans.Providers;
using Orleans.Runtime;
using Orleans.Streams;

namespace LibraProgramming.Grains.Implementation
{
    public class ChatRoomState : GrainState
    {
        public string Id
        {
            get;
            set;
        }

        public Queue<RoomMessage> Messages
        {
            get;
            set;
        }
    }

//    [Reentrant]
    [StorageProvider(ProviderName = "MemoryStore")]
    public class ChatRoomGrain : Grain<ChatRoomState>, IChatRoomGrain
    {
        private IAsyncStream<RoomMessage> stream;
        private ConcurrentDictionary<Guid, string> users;
        private Logger logger;
         
        Task<IList<RoomMessage>> IChatRoomGrain.GetMessagesAsync(int startFromId)
        {
            var messages = State.Messages
                .Where(message => startFromId <= message.Id)
                .ToList();
            return Task.FromResult<IList<RoomMessage>>(messages);
        }

        public async Task AddUserAsync(Guid userId)
        {
            var user = GrainFactory.GetGrain<IChatUser>(userId);
            var profile = await user.GetUserProfileAsync();

            users.TryAdd(profile.Id, profile.Name);

            logger.Info($"LibraProgramming.Grains.Implementation.ChatRoom.AddUser | Room: {State.Id} user: {userId}");
        }

        async Task IChatRoomGrain.PublishMessageAsync(Guid userId, PublishMessage message)
        {
            string nick;

            if (false == users.TryGetValue(userId, out nick))
            {
                return;
            }

            var roomMessage = new RoomMessage
            {
                Id = State.Messages.Count,
                PublisherId = userId,
                PublisherNick = nick,
                Date = DateTime.UtcNow,
                Text = message.Text
            };

            State.Messages.Enqueue(roomMessage);

            logger.Info(1, "ChatRoomGrain New message published");

            await Task.WhenAll(WriteStateAsync(), stream.OnNextAsync(roomMessage));
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
                State.Messages = new Queue<RoomMessage>();
            }

            users = new ConcurrentDictionary<Guid, string>();
            logger = GetLogger(nameof(ChatRoomGrain));

            var provider = GetStreamProvider("SMSProvider");

            stream = provider.GetStream<RoomMessage>(Streams.Id, this.GetPrimaryKeyString());
            
            return base.OnActivateAsync();
        }
    }
}