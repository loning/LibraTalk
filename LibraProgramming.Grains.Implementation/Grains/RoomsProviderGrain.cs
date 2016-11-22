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
    public sealed class RoomsProviderState
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
    public class RoomsProviderGrain : Grain<RoomsProviderState>, IRoomsProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyDictionary<string, Guid>> IRoomsProvider.GetAllRoomsAsync()
        {
            return Task.FromResult<IReadOnlyDictionary<string, Guid>>(new ReadOnlyDictionary<string, Guid>(State.Rooms));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        async Task<Guid> IRoomsProvider.GetOrAddRoomAsync(string alias)
        {
            Guid id;

            if (State.Rooms.TryGetValue(alias, out id))
            {
                return id;
            }

            id = Guid.NewGuid();
            State.Rooms.Add(alias, id);

            await WriteStateAsync();

            return id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        async Task<bool> IRoomsProvider.RemoveRoomAsync(string alias)
        {
            var succeeded = State.Rooms.Remove(alias);

            if (false == succeeded)
            {
                return false;
            }

            await WriteStateAsync();

            return true;
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