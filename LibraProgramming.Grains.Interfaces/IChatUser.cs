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
        Task<string> GetName();

        /// <summary>
        /// Sets name for the player.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task SetName(string name);

        Task PublishMessage(PublishMessage message);
    }
}
