using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;

namespace LibraProgramming.Grains.Interfaces.Grains
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRoomsProvider : IGrainWithStringKey
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyDictionary<string, Guid>> GetAllRoomsAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        Task<Guid> GetOrAddRoomAsync(string alias);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        Task<bool> RemoveRoomAsync(string alias);
    }
}