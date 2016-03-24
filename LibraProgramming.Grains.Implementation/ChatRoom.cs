using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibraProgramming.Grains.Interfaces;
using Orleans;
using Orleans.Concurrency;
using Orleans.Providers;
using Orleans.Runtime;

namespace LibraProgramming.Grains.Implementation
{
    public class CharRoomState : GrainState
    {
        public string Id
        {
            get;
            set;
        }

        public Queue<ChatMessage> Messages
        {
            get;
            set;
        }
    }

    /*[Reentrant]
    [StorageProvider(ProviderName = "MemoryStore")]
    public class ChatRoom : Grain<CharRoomState>, IChatRoom
    {
        private List<Guid> users;
        private Logger logger;
         
        public Task<IList<ChatMessage>> GetMessages(string name)
        {
            throw new NotImplementedException();
        }

        public Task AddUser(Guid userId)
        {
            throw new NotImplementedException();
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
                State.Messages = new Queue<ChatMessage>();
            }

            users = new List<Guid>();
            logger = GetLogger(nameof(ChatRoom));

            return base.OnActivateAsync();
        }
    }*/
}