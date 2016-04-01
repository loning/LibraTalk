using System;
using System.Threading.Tasks;
using LibraProgramming.Grains.Interfaces;
using Orleans;
using Orleans.Concurrency;
using Orleans.Providers;
using Orleans.Runtime;

namespace LibraProgramming.Grains.Implementation
{
    public class ChatUserState : GrainState
    {
        public string Name
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Grain implementation class ChatUserGrain.
    /// </summary>
    [StorageProvider(ProviderName = "MemoryStore")]
    public class ChatUserGrain : Grain<ChatUserState>, IChatUser
    {
        private Logger logger;

        Task<UserProfile> IChatUser.GetUserProfileAsync()
        {
            return Task.FromResult(new UserProfile
            {
                Id = this.GetPrimaryKey(),
                Name = State.Name
            });
        }

        Task IChatUser.SetProfileAsync(UserProfile profile)
        {
            if (null == profile)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            if (this.GetPrimaryKey() != profile.Id)
            {
                throw new ArgumentOutOfRangeException();
            }

            State.Name = profile.Name;

            logger.Info($"LibraProgramming.Grains.Implementation.ChatUser.SetName | Changing for user: {profile.Id} to: {State.Name}");

            return WriteStateAsync();
        }

        Task IChatUser.PublishMessageAsync(PublishMessage message)
        {
            var room = GrainFactory.GetGrain<IChatRoomGrain>("default");

            logger.Info($"LibraProgramming.Grains.Implementation.ChatUser.PublishMessageAsync | Publishing message from user: {this.GetPrimaryKey()}");

            return room.PublishMessageAsync(this.GetPrimaryKey(), message);
        }

        public override Task OnActivateAsync()
        {
            if (null == State.Name)
            {
                State.Name = "John Doe";
            }

            logger = GetLogger("ChatUserGrain");

            logger.Info($"LibraProgramming.Grains.Implementation.ChatUser.OnActivateAsync | Initializing state for user: {this.GetPrimaryKey()}");

            return base.OnActivateAsync();
        }
    }
}
