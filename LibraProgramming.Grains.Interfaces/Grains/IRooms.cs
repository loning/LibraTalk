using System.Threading.Tasks;
using LibraProgramming.Grains.Interfaces.Entities;
using Orleans;

namespace LibraProgramming.Grains.Interfaces.Grains
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRooms : IGrainWithStringKey
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
        Task<bool> DropRoomAsync(string alias);
    }
}