using System;
using System.Threading.Tasks;
using Orleans;
using SampleGrainInterfaces;

namespace SampleGrains
{
    /// <summary>
    /// Grain implementation class PlayerGrain.
    /// </summary>
    public class PlayerGrain : Grain, IPlayerGrain
    {
        private PlayerInfo _info;

        Task<string> IPlayerGrain.GetName()
        {
            return Task.FromResult(_info.Name);
        }

        Task IPlayerGrain.SetName(string name)
        {
            _info = new PlayerInfo(_info.Key, name);
            return TaskDone.Done;
        }

        Task<string> IPlayerGrain.Echo(string text)
        {
            Console.WriteLine($"[Player: {_info.Name}] echoing: {text}");
            return Task.FromResult(text);
        }

        Task IPlayerGrain.Die()
        {
            return TaskDone.Done;
        }

        public override Task OnActivateAsync()
        {
            _info = new PlayerInfo(this.GetPrimaryKey(), "John Doe");

            return base.OnActivateAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        private class PlayerInfo
        {
            public Guid Key
            {
                get;
            }

            public string Name
            {
                get;
            }

            public PlayerInfo(Guid key, string name)
            {
                Key = key;
                Name = name;
            }
        }
    }
}
