using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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
    [Switch("WebHost.Controllers", typeof (SourceSwitch))]
    public class PollController : ApiController
    {
        private static readonly TraceSource trace = new TraceSource("WebHost.Controllers");

        // GET api/poll/<id>
        public async Task<HttpResponseMessage> Get(string id)
        {
            var poller = new MessagePoller();

            using (poller.SubscribeTo(ActionMonitors.Messages))
            {
                if (false == await poller.Wait(TimeSpan.FromSeconds(20.0d)))
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent);
                }

                var response = Request.CreateResponse(HttpStatusCode.OK);
                var dict = new Dictionary<string, string>();

                var messages = poller.GetMessages();

                dict.Add("count", messages.Length.ToString());

                for (var index = 0; index < messages.Length; index++)
                {
                    var message = messages[index];

                    dict.Add("[" + index + "_mid]", message.Id.ToString());
                    dict.Add("[" + index + "_pid]", message.PublisherId.ToString());
                    dict.Add("[" + index + "_nick]", message.PublisherNick);
                    dict.Add("[" + index + "_text]", message.Text);
                }

                response.Content = new FormUrlEncodedContent(dict);

                return response;
            }
        }

        private class MessagePoller : IObserver<IChatMessageAction>, IDisposable
        {
            private IDisposable token;
            private ManualResetEventSlim flag;
            private bool disposed;
            private ConcurrentQueue<RoomMessage> messages;

            public MessagePoller()
            {
                flag = new ManualResetEventSlim();
                messages = new ConcurrentQueue<RoomMessage>();
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

            public RoomMessage[] GetMessages()
            {
                var temp = messages.ToArray();
                messages = new ConcurrentQueue<RoomMessage>();
                return temp;
            }

            void IObserver<IChatMessageAction>.OnNext(IChatMessageAction value)
            {
                var received = (MessageReceivedChatAction) value;

                messages.Enqueue(received.Message);

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