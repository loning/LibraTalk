﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;

namespace LibraProgramming.Grains.Interfaces.Grains
{
    /// <summary>
    /// Chat room grain.
    /// </summary>
    public interface IRoommates : IGrainWithGuidKey
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<bool> AddUserAsync(IUserProfile user);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<bool> RemoveUserAsync(IUserProfile user);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<bool> HasUserAsync(IUserProfile user);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyCollection<IUserProfile>> GetUsersAsync();
    }
}