using System.Threading.Tasks;
using LibraProgramming.Grains.Interfaces.Entities;
using Orleans;

namespace LibraProgramming.Grains.Interfaces.Grains
{
    /// <summary>
    /// Grain interface IChatUser
    /// </summary>
	public interface IChatUser : IGrainWithGuidKey
    {
        /// <summary>
        /// Gets the player name.
        /// </summary>
        /// <returns></returns>
        Task<UserProfile> GetUserProfileAsync();

        /// <summary>
        /// Sets name for the player.
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        Task SetProfileAsync(UserProfile profile);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        Task SetCurrentRoomAsync(IChatRoom value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task PublishMessageAsync(UserMessage message);
    }
}
