using System.Threading.Tasks;
using Orleans;

namespace LibraProgramming.Grains.Interfaces
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
//        Task<UserProfile> GetUserProfileAsync();

        /// <summary>
        /// Sets name for the player.
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
//        Task SetProfileAsync(UserProfile profile);

//        Task PublishMessageAsync(PublishMessage message);
    }
}
