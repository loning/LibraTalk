using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using LibraProgramming.Grains.Interfaces;
using WebHost.Infrastructure;
using WebHost.Infrastructure.Actions;

namespace WebHost.Controllers
{
    public class PollController : ApiController
    {
        // GET api/poll/<id>
        public async Task<HttpResponseMessage> Get(string id)
        {
            var poller = new MessagePoller();

            using (poller.SubscribeTo(ActionMonitors.Chat))
            {
                if (false == await poller.Wait(TimeSpan.FromSeconds(20.0d)))
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent);
                }

//                var room = GrainClient.GrainFactory.GetGrain<IChatRoomGrain>(id);
//                var messages = await room.GetMessagesAsync(0);

                var response = Request.CreateResponse(HttpStatusCode.OK);
                var dict = new Dictionary<string, string>();

                foreach (var message in poller.Messages)
                {
                    dict.Add("[" + message.Id + "]", message.Text);
                }

                response.Content = new FormUrlEncodedContent(dict);

                return response;
            }
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        /*public async Task<string> Post([FromBody]string value)
        {
            var player = GrainClient.GrainFactory.GetGrain<IChatUser>(Guid.NewGuid());
            var result = await player.Echo(value);

            return result;
        }*/

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }

        private class MessagePoller : IObserver<IChatMessageAction>, IDisposable
        {
            private IDisposable token;
            private ManualResetEventSlim flag;
            private bool disposed;

            public ConcurrentQueue<RoomMessage> Messages
            {
                get;
            }

            public MessagePoller()
            {
                flag = new ManualResetEventSlim();
                Messages = new ConcurrentQueue<RoomMessage>();
            }

            public Task<bool> Wait(TimeSpan timeout)
            {
                if (disposed)
                {
                    return Task.FromResult(false);
                }

                return Task.Run(() => flag.Wait(timeout));
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
                var received = (MessageReceivedChatAction) value;

                Messages.Enqueue(received.Message);

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