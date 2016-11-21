using System;
using System.Threading.Tasks;
using LibraProgramming.Grains.Interfaces.Grains;
using Orleans;
using Orleans.Concurrency;

namespace LibraProgramming.Grains.Implementation.Grains
{
    /// <summary>
    /// 
    /// </summary>
    [StatelessWorker]
    public class AcviteRoomsGrain : Grain, IActiveChatRooms
    {
        private const string Namespace = "$ROOMS";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        async Task<Guid> IActiveChatRooms.GetRoomAsync(string name)
        {
            var registration = GrainClient.GrainFactory.GetGrain<IRegisteredRooms>(Namespace);
            var rooms = await registration.GetRoomsAsync();

            Guid id;

            if (false == rooms.TryGetValue(name, out id))
            {
                id = Guid.Empty;
            }

            return id;
        }
    }
}