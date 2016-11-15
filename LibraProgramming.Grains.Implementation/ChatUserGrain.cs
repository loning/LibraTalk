using System;
using System.Threading.Tasks;
using LibraProgramming.Grains.Interfaces;
using Orleans;
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
                Name = State.Name
            });
        }

        Task IChatUser.SetProfileAsync(UserProfile profile)
        {
            if (null == profile)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            State.Name = profile.Name;

            return WriteStateAsync();
        }

/*
        Task IChatUser.PublishMessageAsync(PublishMessage message)
        {
            var room = GrainFactory.GetGrain<IChatRoomGrain>("default");

            logger.Info($"LibraProgramming.Grains.Implementation.ChatUser.PublishMessageAsync | Publishing message from user: {this.GetPrimaryKey()}");

            return room.PublishMessageAsync(this.GetPrimaryKey(), message);
            return TaskDone.Done;
        }
*/

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
