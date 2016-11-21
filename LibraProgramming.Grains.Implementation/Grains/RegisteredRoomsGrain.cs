using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using LibraProgramming.Grains.Interfaces.Grains;
using Orleans;
using Orleans.Providers;

namespace LibraProgramming.Grains.Implementation.Grains
{
    /// <summary>
    /// Persistent rooms state.
    /// </summary>
    public sealed class RegisteredRoomsState
    {
        /// <summary>
        /// Gets or sets an list of the rooms.
        /// </summary>
        public Dictionary<string, Guid> Rooms
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [StorageProvider(ProviderName = "MemoryStore")]
    public class RegisteredRoomsGrain : Grain<RegisteredRoomsState>, IRegisteredRooms
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyDictionary<string, Guid>> IRegisteredRooms.GetRoomsAsync()
        {
            return Task.FromResult<IReadOnlyDictionary<string, Guid>>(new ReadOnlyDictionary<string, Guid>(State.Rooms));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Task OnActivateAsync()
        {
            if (null == State.Rooms)
            {
                State.Rooms = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
            }

            return base.OnActivateAsync();
        }
    }
}