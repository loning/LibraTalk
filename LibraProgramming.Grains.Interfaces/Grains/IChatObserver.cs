using System;
using System.Threading.Tasks;
using LibraProgramming.Grains.Interfaces.Entities;
using Orleans;

namespace LibraProgramming.Grains.Interfaces.Grains
{
    /// <summary>
    /// 
    /// </summary>
    public interface IChatObserver : IGrainWithGuidKey
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task SubscribeAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        Task<ChatMessage> WaitForUpdates(TimeSpan timeout);
    }
}