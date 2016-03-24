using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using Orleans;
using SampleGrainInterfaces;
using WebHost.Infrastructure;
using WebHost.Infrastructure.Actions;

namespace WebHost.Controllers
{
    public class PollController : ApiController
    {
        // GET api/poll/<id>
        public async Task<string> Get(string id, [FromUri(Name = "token")]string token)
        {
//            ActionMonitors.Chat.TrackAction(new MessageReceivedChatAction());
            var poll = new ChatPoll();

            using (poll.SubscribeTo(ActionMonitors.Chat))
            {
                if (await poll.Wait(TimeSpan.FromSeconds(20.0d)))
                {
                    return "available";
                }

                return "empty";
            }
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public async Task<string> Post([FromBody]string value)
        {
            var player = GrainClient.GrainFactory.GetGrain<IPlayerGrain>(Guid.NewGuid());
            var result = await player.Echo(value);

            return result;
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }

        private class ChatPoll : IObserver<IChatMessageAction>, IDisposable
        {
            private IDisposable token;
            private ManualResetEventSlim flag;
            private bool disposed;

            public ChatPoll()
            {
                flag = new ManualResetEventSlim();
            }

            public Task<bool> Wait(TimeSpan timeout)
            {
                return Task.Run(() =>
                {
                    if (disposed)
                    {
                        return false;
                    }

                    return flag.Wait(timeout);
                });
            }

            public IDisposable SubscribeTo(IObservable<IChatMessageAction> provider)
            {
                if (null == provider)
                {
                    throw new ArgumentNullException(nameof(provider));
                }

                token = provider.Subscribe(this);

                return this;
            }

            void IObserver<IChatMessageAction>.OnNext(IChatMessageAction value)
            {
                flag.Set();
            }

            void IObserver<IChatMessageAction>.OnError(Exception error)
            {
                flag.Reset();
            }

            void IObserver<IChatMessageAction>.OnCompleted()
            {
                Dispose(true);
            }

            void IDisposable.Dispose()
            {
                Dispose(true);
            }

            private void Dispose(bool dispose)
            {
                if (disposed)
                {
                    return;
                }

                try
                {
                    if (dispose)
                    {
                        flag.Dispose();
                        token.Dispose();

                        flag = null;
                        token = null;
                    }
                }
                finally
                {
                    disposed = true;
                }
            }
        }
    }
}