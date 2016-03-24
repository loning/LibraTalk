using System.Threading.Tasks;
using Orleans;

namespace SampleGrainInterfaces
{
    /// <summary>
    /// Grain interface IPlayerGrain
    /// </summary>
	public interface IPlayerGrain : IGrainWithGuidKey
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        Task<string> Echo(string text);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task Die();
    }
}
