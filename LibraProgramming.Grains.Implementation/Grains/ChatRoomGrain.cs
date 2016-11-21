using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibraProgramming.Grains.Interfaces;
using LibraProgramming.Grains.Interfaces.Entities;
using LibraProgramming.Grains.Interfaces.Grains;
using Orleans;
using Orleans.Concurrency;
using Orleans.Streams;

namespace LibraProgramming.Grains.Implementation.Grains
{
    public class ChatRoomGrain : Grain, IChatRoom
    {
        private const string RoomsNamespace = "$ROOMS";

        private IAsyncStream<RoomMessage> messages;

        async Task IChatRoom.JoinAsync(IChatUser user)
        {
            if (null == user)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var room = this.GetPrimaryKeyString();
            var rooms = GrainFactory.GetGrain<IRegisteredRooms>(RoomsNamespace);
            var list = await rooms.GetRoomsAsync();

            Guid id;

            if (false == list.TryGetValue(room, out id))
            {
                id = await rooms.OpenRoomAsync(room);
            }

            var roommates = GrainFactory.GetGrain<IRoommates>(id);
            var accepted = await roommates.AddUserAsync(user);

            if (accepted)
            {
                await user.SetCurrentRoomAsync(this);
            }
        }

        async Task IChatRoom.LeaveAsync(IChatUser user)
        {
            if (null == user)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var room = this.GetPrimaryKeyString();
            var rooms = GrainFactory.GetGrain<IRegisteredRooms>(RoomsNamespace);
            var list = await rooms.GetRoomsAsync();

            Guid id;

            if (false == list.TryGetValue(room, out id))
            {
                throw new ArgumentException("No room found");
            }

            var roommates = GrainFactory.GetGrain<IRoommates>(id);
            var removed = await roommates.RemoveUserAsync(user);

            if (removed)
            {
                await user.SetCurrentRoomAsync(null);
            }
        }

        async Task IChatRoom.PublishAsync(IChatUser user, UserMessage message)
        {
            if (null == user)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (null == message)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var room = this.GetPrimaryKeyString();
            var rooms = GrainFactory.GetGrain<IRegisteredRooms>(RoomsNamespace);
            var list = await rooms.GetRoomsAsync();

            Guid id;

            if (false == list.TryGetValue(room, out id))
            {
                throw new ArgumentException("No room found");
            }

            var roommates = GrainFactory.GetGrain<IRoommates>(id);
            var present = await roommates.HasUserAsync(user);

            if (present)
            {
                await messages.OnNextAsync(new RoomMessage
                {
                    Id = 1,
                    Date = DateTime.UtcNow,
                    PublisherId = user.GetPrimaryKey(),
                    Text = message.Text
                });
            }
        }

        async Task<IReadOnlyCollection<IChatUser>> IChatRoom.GetUsersAsync()
        {
            var room = this.GetPrimaryKeyString();
            var rooms = GrainFactory.GetGrain<IRegisteredRooms>(RoomsNamespace);
            var list = await rooms.GetRoomsAsync();

            Guid id;

            if (false == list.TryGetValue(room, out id))
            {
                throw new ArgumentException("No room found");
            }

            var roommates = GrainFactory.GetGrain<IRoommates>(id);
            var users = await roommates.GetUsersAsync();

            return users;
        }

        public override async Task OnActivateAsync()
        {
            var provider = GetStreamProvider("SMSProvider");
            var rooms = GrainFactory.GetGrain<IRegisteredRooms>(RoomsNamespace);
            var list = await rooms.GetRoomsAsync();

            Guid id;

            if (false == list.TryGetValue(this.GetPrimaryKeyString(), out id))
            {
                throw new ArgumentException("No room found");
            }

            messages = provider.GetStream<RoomMessage>(id, RoomsNamespace);

            await base.OnActivateAsync();
        }
    }
}