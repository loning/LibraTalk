using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibraProgramming.Grains.Interfaces.Entities;
using LibraProgramming.Grains.Interfaces.Grains;
using Orleans;
using Orleans.Providers;

namespace LibraProgramming.Grains.Implementation.Grains
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public sealed class RoomDescriptor
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Id
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Alias
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Persistent rooms state.
    /// </summary>
    [Serializable]
    public sealed class RoomsState
    {
        /// <summary>
        /// Gets or sets an list of the rooms.
        /// </summary>
        public List<RoomDescriptor> Rooms
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [StorageProvider(ProviderName = "MemoryStore")]
    public class RoomsGrain : Grain<RoomsState>, IRooms
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        Task<Room> IRooms.GetRoomAsync(string alias)
        {
            if (null == alias)
            {
                throw new ArgumentNullException(nameof(alias));
            }

            var descriptor = FindRoomByAlias(alias);

            if (null == descriptor)
            {
                return Task.FromResult<Room>(null);
            }

            return Task.FromResult(new Room
            {
                Id = descriptor.Id,
                Alias = descriptor.Alias,
                Description = descriptor.Description
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        async Task<Room> IRooms.RegisterRoomAsync(string alias, string description)
        {
            if (null == alias)
            {
                throw new ArgumentNullException(nameof(alias));
            }

            var descriptor = FindRoomByAlias(alias);

            if (null == descriptor)
            {
                descriptor = new RoomDescriptor
                {
                    Id = Guid.NewGuid(),
                    Alias = alias,
                    Description = description
                };

                State.Rooms.Add(descriptor);

                await WriteStateAsync();
            }

            return new Room
            {
                Id = descriptor.Id,
                Alias = descriptor.Alias,
                Description = descriptor.Description
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        async Task<bool> IRooms.DropRoomAsync(string alias)
        {
            if (null == alias)
            {
                throw new ArgumentNullException(nameof(alias));
            }

            var descriptor = FindRoomByAlias(alias);
            var exists = null != descriptor;

            if (exists)
            {
                State.Rooms.Remove(descriptor);

                await WriteStateAsync();
            }

            return exists;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Task OnActivateAsync()
        {
            if (null == State.Rooms)
            {
                State.Rooms = new List<RoomDescriptor>();
            }

            return base.OnActivateAsync();
        }

        private RoomDescriptor FindRoomByAlias(string alias)
        {
            var comparer = StringComparer.OrdinalIgnoreCase;
            return State.Rooms.Find(room => comparer.Equals(alias, room.Alias));
        }
    }
}