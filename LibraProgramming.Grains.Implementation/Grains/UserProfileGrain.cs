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
    public class UserProfileState
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
    public class UserProfileGrain : Grain<UserProfileState>, IUserProfile
    {
        private Logger logger;

        public override Task OnActivateAsync()
        {
            if (null == State.Name)
            {
                State.Name = "John Doe";
            }

            logger = GetLogger("ChatUserGrain");

            logger.Info($"LibraProgramming.Grains.Implementation.Grains.UserProfileGrain | Initializing state for user: {this.GetPrimaryKey()}");

            return base.OnActivateAsync();
        }

        Task<UserProfile> IUserProfile.GetProfileAsync()
        {
            logger.Info($"LibraProgramming.Grains.Implementation.Grains.UserProfileGrain | GetProfileAsync for user: {this.GetPrimaryKey()}");
            return Task.FromResult(
                new UserProfile
                {
                    Name = State.Name
                }
            );
        }

        Task IUserProfile.SetProfileAsync(UserProfile profile)
        {
            if (null == profile)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            State.Name = profile.Name;

            logger.Info($"LibraProgramming.Grains.Implementation.Grains.UserProfileGrain | SetProfileAsync for user: {this.GetPrimaryKey()}");

            return WriteStateAsync();
        }
    }
}
