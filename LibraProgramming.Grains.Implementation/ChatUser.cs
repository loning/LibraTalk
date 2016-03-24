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

            logger.Info($"Changing name: [id : {this.GetPrimaryKey()}; {State.Name}]");

            return WriteStateAsync();
        }

        public override Task OnActivateAsync()
        {
            if (null == State.Name)
            {
                State.Name = "John Doe";
            }

            logger = GetLogger("ChatUser");

            logger.Info($"Initializing state: [id : {this.GetPrimaryKey()}; {State.Name}]");

            return base.OnActivateAsync();
        }
    }
}
