using System;
using System.Threading.Tasks;
using LibraProgramming.Grains.Interfaces.Entities;
using LibraProgramming.Grains.Interfaces.Grains;
using Orleans;
using Orleans.Providers;
using Orleans.Runtime;

namespace LibraProgramming.Grains.Implementation.Grains
{
    /// <summary>
    /// Chat user grain internal state object.
    /// </summary>
    public class ChatUserGrainState
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
    public class ChatUserGrain : Grain<ChatUserGrainState>, IChatUser
    {
        private IChatRoom room;
        private Logger logger;

        public override Task OnActivateAsync()
        {
            if (null == State.Name)
            {
                State.Name = "John Doe";
            }

            logger = GetLogger("ChatUserGrain");

            logger.Info($"LibraProgramming.Grains.Implementation.Grains.ChatUserGrain.OnActivateAsync | Initializing state for user: {this.GetPrimaryKey()}");

            return base.OnActivateAsync();
        }

        Task<UserProfile> IChatUser.GetUserProfileAsync()
        {
            return Task.FromResult(
                new UserProfile
                {
                    Name = State.Name
                }
            );
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

        Task IChatUser.SetCurrentRoomAsync(IChatRoom value)
        {
            room = value;

            return TaskDone.Done;
        }

        Task IChatUser.PublishMessageAsync(UserMessage message)
        {
            if (null == message)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (null == room)
            {
                throw new InvalidOperationException();
            }

            return room.PublishMessageAsync(this, message);
        }
    }
}
