using System.Threading.Tasks;
using LibraProgramming.Grains.Interfaces.Entities;
using Orleans;

namespace LibraProgramming.Grains.Interfaces.Grains
{
    /// <summary>
    /// Grain interface IChatUser
    /// </summary>
	public interface IUserProfile : IGrainWithGuidKey
    {
        /// <summary>
        /// Gets the player name.
        /// </summary>
        /// <returns></returns>
        Task<UserProfile> GetProfileAsync();

        /// <summary>
        /// Sets name for the player.
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        Task SetProfileAsync(UserProfile profile);
    }
}
