﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraProgramming.Grains.Interfaces;
using Orleans;
using Orleans.Concurrency;
using Orleans.Providers;
using Orleans.Runtime;
using Orleans.Streams;

namespace LibraProgramming.Grains.Implementation
{
    public class CharRoomState : GrainState
    {
        public string Id
        {
            get;
            set;
        }

        public Queue<RoomMessage> Messages
        {
            get;
            set;
        }
    }

    [Reentrant]
    [StorageProvider(ProviderName = "MemoryStore")]
    public class ChatRoomGrain : Grain<CharRoomState>, IChatRoomGrain
    {
        private IAsyncStream<RoomMessage> stream;
        private List<Guid> users;
        private Logger logger;
         
        Task<IList<RoomMessage>> IChatRoomGrain.GetMessagesAsync(int startFromId)
        {
            var messages = State.Messages
                .Where(message => startFromId <= message.Id)
                .ToList();
            return Task.FromResult<IList<RoomMessage>>(messages);
        }

        public Task AddUserAsync(Guid userId)
        {
            users.Add(userId);
            logger.Info($"LibraProgramming.Grains.Implementation.ChatRoom.AddUser | Room: {State.Id} user: {userId}");
            return TaskDone.Done;
        }

        Task IChatRoomGrain.PublishMessageAsync(Guid userId, PublishMessage message)
        {
            if (!users.Contains(userId))
            {
                return TaskDone.Done;
            }

            var roomMessage = new RoomMessage
            {
                Id = State.Messages.Count,
                PublisherId = userId,
                Date = DateTime.UtcNow,
                Text = message.Text
            };

            State.Messages.Enqueue(roomMessage);

            logger.Info($"LibraProgramming.Grains.Implementation.ChatRoomGrain.PublishMessageAsync | New message published");

            return Task.WhenAll(WriteStateAsync(), stream.OnNextAsync(roomMessage));
        }

        /// <summary>
        /// This method is called at the end of the process of activating a grain.
        /// It is called before any messages have been dispatched to the grain.
        /// For grains with declared persistent state, this method is called after the State property has been populated.
        /// </summary>
        public override Task OnActivateAsync()
        {
            State.Id = this.GetPrimaryKeyString();

            if (null == State.Messages)
            {
                State.Messages = new Queue<RoomMessage>();
            }

            users = new List<Guid>();
            logger = GetLogger(nameof(ChatRoomGrain));

            var provider = GetStreamProvider("SMSProvider");

            stream = provider.GetStream<RoomMessage>(Streams.Id, this.GetPrimaryKeyString());
            
            return base.OnActivateAsync();
        }
    }
}