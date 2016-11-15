using System;
using System.Threading.Tasks;
using LibraProgramming.Grains.Interfaces;
using LibraProgramming.Grains.Interfaces.Entities;
using LibraProgramming.Grains.Interfaces.Grains;
using Orleans;
using Orleans.Concurrency;
using Orleans.Streams;

namespace LibraProgramming.Grains.Implementation.Grains
{
    [StatelessWorker]
    public class ChatRoomGrain : Grain, IChatRoom
    {
        private IAsyncStream<RoomMessage> messages;

        async Task IChatRoom.AdmitUserAsync(IChatUser user)
        {
            if (null == user)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var roommates = GrainFactory.GetGrain<IRoommates>(this.GetPrimaryKeyString());
            var accepted = await roommates.AddUserAsync(user);

            if (accepted)
            {
                await user.SetCurrentRoomAsync(this);
            }
        }

        async Task IChatRoom.QuitUserAsync(IChatUser user)
        {
            if (null == user)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var roommates = GrainFactory.GetGrain<IRoommates>(this.GetPrimaryKeyString());
            var removed = await roommates.RemoveUserAsync(user);

            if (removed)
            {
                await user.SetCurrentRoomAsync(null);
            }
        }

        async Task IChatRoom.PublishMessageAsync(IChatUser user, UserMessage message)
        {
            if (null == user)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (null == message)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var roommates = GrainFactory.GetGrain<IRoommates>(this.GetPrimaryKeyString());
            var present = await roommates.HasUserAsync(user);

            if (present)
            {
                // publish message to stream
            }
        }

        public override Task OnActivateAsync()
        {
            var provider = GetStreamProvider("SMSProvider");

            messages = provider.GetStream<RoomMessage>(Streams.Id, this.GetPrimaryKeyString());

            return base.OnActivateAsync();
        }
    }
}