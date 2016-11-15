using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibraProgramming.Grains.Interfaces.Grains;
using Orleans;
using Orleans.Providers;

namespace LibraProgramming.Grains.Implementation.Grains
{
    /// <summary>
    /// 
    /// </summary>
    public class RoomGrainState
    {
        public List<IChatUser> Users
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [StorageProvider(ProviderName = "MemoryStore")]
    public class RoommatesGrain : Grain<RoomGrainState>, IRoommates
    {
        async Task<bool> IRoommates.AddUserAsync(IChatUser user)
        {
            if (null == user)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var id = user.GetPrimaryKey();

            if (State.Users.Exists(roommate => SameUser(roommate, id)))
            {
                return false;
            }

            State.Users.Add(user);

            await WriteStateAsync();

            return true;
        }

        async Task<bool> IRoommates.RemoveUserAsync(IChatUser user)
        {
            if (null == user)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var id = user.GetPrimaryKey();
            var index = State.Users.FindIndex(roommate => SameUser(roommate, id));

            if (index < 0)
            {
                return false;
            }

            State.Users.RemoveAt(index);

            await WriteStateAsync();

            return true;
        }

        Task<bool> IRoommates.HasUserAsync(IChatUser user)
        {
            if (null == user)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var id = user.GetPrimaryKey();

            return Task.FromResult(State.Users.Exists(roommate => SameUser(roommate, id)));
        }

        /// <summary>
        /// This method is called at the end of the process of activating a grain.
        /// It is called before any messages have been dispatched to the grain.
        /// For grains with declared persistent state, this method is called after the State property has been populated.
        /// </summary>
        public override Task OnActivateAsync()
        {
            if (null == State.Users)
            {
                State.Users = new List<IChatUser>();
            }

            return base.OnActivateAsync();
        }

        private static bool SameUser(IChatUser user, Guid id)
        {
            return user.GetPrimaryKey() == id;
        }
    }
}