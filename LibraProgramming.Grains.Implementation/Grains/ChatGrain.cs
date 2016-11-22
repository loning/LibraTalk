using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibraProgramming.Grains.Interfaces.Grains;
using Orleans;
using Orleans.Concurrency;

namespace LibraProgramming.Grains.Implementation.Grains
{
    /// <summary>
    /// 
    /// </summary>
    [StatelessWorker]
    public class ChatGrain : Grain, IChat
    {
        private const string Rooms = "$ROOMS";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        async Task<bool> IChat.JoinUserAsync(string alias, Guid user)
        {
            var rooms = GrainClient.GrainFactory.GetGrain<IRoomsProvider>(Rooms);
            var profile = GrainClient.GrainFactory.GetGrain<IUserProfile>(user);
            var id = await rooms.GetOrAddRoomAsync(alias);
            var roommates = GrainClient.GrainFactory.GetGrain<IRoommates>(id);

            return await roommates.AddUserAsync(profile);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        async Task<IReadOnlyCollection<IUserProfile>> IChat.GetUsersAsync(string alias)
        {
            var rooms = GrainClient.GrainFactory.GetGrain<IRoomsProvider>(Rooms);
            var id = await rooms.GetOrAddRoomAsync(alias);
            var roommates = GrainClient.GrainFactory.GetGrain<IRoommates>(id);

            return await roommates.GetUsersAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        async Task<bool> IChat.LeaveUserAsync(string alias, Guid user)
        {
            var rooms = GrainClient.GrainFactory.GetGrain<IRoomsProvider>(Rooms);
            var profile = GrainClient.GrainFactory.GetGrain<IUserProfile>(user);
            var id = await rooms.GetOrAddRoomAsync(alias);
            var roommates = GrainClient.GrainFactory.GetGrain<IRoommates>(id);

            return await roommates.RemoveUserAsync(profile);
        }
    }
}