using System;
using System.Threading.Tasks;
using LibraProgramming.Grains.Interfaces;
using Orleans;
using Orleans.Concurrency;
using Orleans.Providers;
using Orleans.Runtime;

namespace LibraProgramming.Grains.Implementation
{
    public class ChatUserState : GrainState
    {
        public string Name
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Grain implementation class ChatUser.
    /// </summary>
    [StorageProvider(ProviderName = "MemoryStore")]
    public class ChatUser : Grain<ChatUserState>, IChatUser
    {
        private Logger logger;

        Task<string> IChatUser.GetName()
        {
            return Task.FromResult(State.Name);
        }

        Task IChatUser.SetName(string name)
        {
            State.Name = name;

            logger.Info($"LibraProgramming.Grains.Implementation.ChatUser.SetName | Changing for user: {this.GetPrimaryKey()} to: {State.Name}");

            return WriteStateAsync();
        }

        public Task PublishMessage(PublishMessage message)
        {
            var room = GrainFactory.GetGrain<IChatRoom>("default");
            return room.PublishMessage(this.GetPrimaryKey(), message.Text);
        }

        public override Task OnActivateAsync()
        {
            if (null == State.Name)
            {
                State.Name = "John Doe";
            }

            logger = GetLogger("ChatUser");

            logger.Info($"LibraProgramming.Grains.Implementation.ChatUser.OnActivateAsync | Initializing state for user: {this.GetPrimaryKey()}");

            return base.OnActivateAsync();
        }
    }
}
