using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using LibraProgramming.Grains.Interfaces;
using Orleans;
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
    }

    [StorageProvider(ProviderName = "MemoryStore")]
    public class ChatRoomGrain : Grain<ChatRoomState>, IChatRoomGrain
    {
        private IAsyncStream<RoomMessage> stream;
        private ConcurrentDictionary<Guid, string> users;
        private long messageCount;
        private Logger logger;
         
        public async Task AddUserAsync(Guid userId)
        {
            var user = GrainFactory.GetGrain<IChatUser>(userId);
            var profile = await user.GetUserProfileAsync();

            if (users.TryAdd(profile.Id, profile.Name))
            {
                logger.Info(1, $"[ChatRoom.AddUser] Room: {State.Id} user: {userId}");
            }
        }

        async Task IChatRoomGrain.PublishMessageAsync(Guid userId, PublishMessage message)
        {
            string nick;

            if (false == users.TryGetValue(userId, out nick))
            {
                logger.Info(1, $"[ChatRoom.PublishMessageAsync] User {userId} not joined the room {State.Id}");
                return;
            }

            var roomMessage = new RoomMessage
            {
                Id = messageCount++,
                PublisherId = userId,
                PublisherNick = nick,
                Date = DateTime.UtcNow,
                Text = message.Text
            };

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

            users = new ConcurrentDictionary<Guid, string>();
            logger = GetLogger(nameof(ChatRoomGrain));

            var provider = GetStreamProvider("SMSProvider");

            stream = provider.GetStream<RoomMessage>(Streams.Id, this.GetPrimaryKeyString());
            
            return base.OnActivateAsync();
        }
    }
}