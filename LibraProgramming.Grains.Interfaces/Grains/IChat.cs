using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;

namespace LibraProgramming.Grains.Interfaces.Grains
{
    /// <summary>
    /// 
    /// </summary>
    public interface IChat : IGrainWithIntegerKey
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        Task<IReadOnlyCollection<IUserProfile>> GetUsersAsync(string alias);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<bool> JoinUserAsync(string alias, Guid user);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<bool> LeaveUserAsync(string alias, Guid user);
    }
}