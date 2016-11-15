﻿using System.Threading.Tasks;
using Orleans;

namespace LibraProgramming.Grains.Interfaces.Grains
{
    /// <summary>
    /// Chat room grain.
    /// </summary>
    public interface IRoommates : IGrainWithStringKey
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<bool> AddUserAsync(IChatUser user);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<bool> RemoveUserAsync(IChatUser user);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<bool> HasUserAsync(IChatUser user);
    }
}