using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibraProgramming.Grains.Interfaces.Entities;
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
        Task<Room> GetRoomAsync(string alias);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        Task<Room> RegisterRoomAsync(string alias, string description);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="author"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task PublishMessageAsync(string alias, Guid author, UserMessage message);
    }
}