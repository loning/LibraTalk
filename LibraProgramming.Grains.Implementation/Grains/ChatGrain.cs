using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using LibraProgramming.Grains.Interfaces.Entities;
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
        /// <param name="description"></param>
        /// <returns></returns>
        async Task<Room> IChat.RegisterRoomAsync(string alias, string description)
        {
            var rooms = GrainFactory.GetGrain<IRooms>(Rooms);
            var room = await rooms.GetRoomAsync(alias);

            if (null == room)
            {
                room = await rooms.RegisterRoomAsync(alias, description);
            }

            return room;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        async Task<Room> IChat.GetRoomAsync(string alias)
        {
            var rooms = GrainFactory.GetGrain<IRooms>(Rooms);
            return await rooms.GetRoomAsync(alias);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        async Task<IReadOnlyCollection<IUserProfile>> IChat.GetUsersAsync(string alias)
        {
            var rooms = GrainFactory.GetGrain<IRooms>(Rooms);
            var room = await rooms.GetRoomAsync(alias);

            if (null == room)
            {
                var empty = new List<IUserProfile>();
                return new ReadOnlyCollection<IUserProfile>(empty);
            }

            var roommates = GrainFactory.GetGrain<IRoommates>(room.Id);

            return await roommates.GetUsersAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        async Task<bool> IChat.JoinUserAsync(string alias, Guid user)
        {
            var rooms = GrainFactory.GetGrain<IRooms>(Rooms);
            var room = await rooms.GetRoomAsync(alias);

            if (null == room)
            {
                return false;
            }

            var roommates = GrainFactory.GetGrain<IRoommates>(room.Id);
            var profile = GrainFactory.GetGrain<IUserProfile>(user);

            if (await roommates.AddUserAsync(profile))
            {

                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        async Task<bool> IChat.LeaveUserAsync(string alias, Guid user)
        {
            var rooms = GrainFactory.GetGrain<IRooms>(Rooms);
            var room = await rooms.GetRoomAsync(alias);

            if (null == room)
            {
                return false;
            }

            var roommates = GrainFactory.GetGrain<IRoommates>(room.Id);
            var profile = GrainFactory.GetGrain<IUserProfile>(user);

            return await roommates.RemoveUserAsync(profile);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="author"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        async Task IChat.PublishMessageAsync(string alias, Guid author, UserMessage message)
        {
            var rooms = GrainFactory.GetGrain<IRooms>(Rooms);
            var room = await rooms.GetRoomAsync(alias);

            if (null == room)
            {
                return;
            }

            var roommates = GrainFactory.GetGrain<IRoommates>(room.Id);
            var profile = GrainFactory.GetGrain<IUserProfile>(author);

            if (false == await roommates.HasUserAsync(profile))
            {
                return;
            }

            var provider = GetStreamProvider("SMSProvider");
            var stream = provider.GetStream<ChatMessage>(room.Id, Rooms);

            await stream.OnNextAsync(new ChatMessage
            {
                Author = author,
                Date = DateTime.UtcNow,
                Text = message.Text
            });
        }
    }
}