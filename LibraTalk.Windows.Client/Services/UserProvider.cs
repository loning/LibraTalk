using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using LibraProgramming.Windows.UI.Xaml.Commands;
using LibraTalk.Windows.Client.Models;
using LibraTalk.Windows.Client.Properties;
using HttpStatusCode = Windows.Web.Http.HttpStatusCode;

namespace LibraTalk.Windows.Client.Services
{
    public class RoomMessage
    {
        public long Id
        {
            get;
            set;
        }

        public Guid PublisherId
        {
            get;
            set;
        }

        public string PublisherNick
        {
            get;
            set;
        }

        /*public DateTime Date
        {
            get;
            set;
        }*/

        public string Text
        {
            get;
            set;
        }
    }

    public class ReceivingMessageEventArgs : EventArgs
    {
        public IEnumerable<RoomMessage> Messages
        {
            get;
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:System.EventArgs"/>.
        /// </summary>
        public ReceivingMessageEventArgs(IEnumerable<RoomMessage> messages)
        {
            Messages = messages;
        }
    }

    public class PollingCancelledEventArgs : EventArgs
    {
    }

    [PublicAPI]
    public sealed class UserProvider
    {
        private readonly Uri baseUri;
        private readonly Guid userId;
        private readonly WeakEvent<TypedEventHandler<UserProvider, ReceivingMessageEventArgs>> messageReceived;
        private readonly WeakEvent<TypedEventHandler<UserProvider, PollingCancelledEventArgs>> pollingCancelled;

        public event TypedEventHandler<UserProvider, ReceivingMessageEventArgs> MessageReceived
        {
            add
            {
                messageReceived.AddHandler(value);
            }
            remove
            {
                messageReceived.RemoveHandler(value);
            }
        }

        public event TypedEventHandler<UserProvider, PollingCancelledEventArgs> PollingCancelled
        {
            add
            {
                pollingCancelled.AddHandler(value);
            }
            remove
            {
                pollingCancelled.RemoveHandler(value);
            }
        } 

        public UserProvider([NotNull] Uri baseUri, [NotNull] Guid userId)
        {
            this.baseUri = baseUri;
            this.userId = userId;
            messageReceived = new WeakEvent<TypedEventHandler<UserProvider, ReceivingMessageEventArgs>>();
            pollingCancelled = new WeakEvent<TypedEventHandler<UserProvider, PollingCancelledEventArgs>>();
        }

        public async Task<Profile> GetProfileAsync()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept
                    .Add(HttpMediaTypeWithQualityHeaderValue.Parse("application/x-www-form-urlencoded"));

                try
                {
                    var response = await client
                        .GetAsync(
                            new Uri(baseUri + "user/" + userId.ToString("D")),
                            HttpCompletionOption.ResponseHeadersRead
                        );

                    var message = response.EnsureSuccessStatusCode();
                    var content = await message.Content.ReadAsStringAsync();
                    var decoder = new WwwFormUrlDecoder(content);

                    return new Profile
                    {
                        Id = Guid.Parse(decoder.GetFirstValueByName("id")),
                        Name = decoder.GetFirstValueByName("name")
                    };
                }
                catch (Exception exception)
                {
                    throw new AggregateException(exception);
                }
            }
        }

        public async Task SetProfileAsync(Profile profile)
        {
            using (var client = new HttpClient())
            {
                var response = await client
                    .PutAsync(
                        new Uri(baseUri + "user/" + userId.ToString("D")),
                        new HttpFormUrlEncodedContent(
                            new Dictionary<string, string>
                            {
                                {
                                    "id", userId.ToString("D")
                                },
                                {
                                    "name", profile.Name
                                }
                            })
                    );
                response.EnsureSuccessStatusCode();
            }
        }

        public async Task PublishMessageAsync(string message)
        {
            using (var client = new HttpClient())
            {
                var response = await client
                    .PutAsync(
                        new Uri(baseUri + "messages"),
                        new HttpFormUrlEncodedContent(
                            new Dictionary<string, string>
                            {
                                {
                                    "user", userId.ToString("D")
                                },
                                {
                                    "text", message
                                }
                            })
                    );
                response.EnsureSuccessStatusCode();
            }
        }

        public async Task<IEnumerable<string>> GetMessagesAsync(int from)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept
                    .Add(HttpMediaTypeWithQualityHeaderValue.Parse("application/x-www-form-urlencoded"));


                try
                {
                    var response = await client
                        .GetAsync(
                            new Uri(baseUri + String.Format("messages?ticket={0}", from)),
                            HttpCompletionOption.ResponseHeadersRead
                        );

                    var message = response.EnsureSuccessStatusCode();

                    if (HttpStatusCode.NoContent == message.StatusCode)
                    {
                        return Enumerable.Empty<string>();
                    }

                    var content = await message.Content.ReadAsStringAsync();
                    var decoder = new WwwFormUrlDecoder(content);

                    return decoder.Select(entry => entry.Value);
                }
                catch (Exception exception)
                {
                    throw new AggregateException(exception);
                }
            }
        }

        public async Task JoinRoomAsync(string room)
        {
            using (var client = new HttpClient())
            {
                var response = await client
                    .PutAsync(
                        new Uri(baseUri + String.Format("room/{0}", room)),
                        new HttpFormUrlEncodedContent(
                            new Dictionary<string, string>
                            {
                                {
                                    "id", userId.ToString("D")
                                }
                            })
                    );
                response.EnsureSuccessStatusCode();
            }
        }

        public void Poll(CancellationToken token)
        {
            Task.Run(() => DoPollInternalAsync(token)).ConfigureAwait(false);
        }

        private async Task DoPollInternalAsync(CancellationToken token)
        {
            try
            {
                while (true)
                {
                    token.ThrowIfCancellationRequested();

                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.CacheControl.Clear();
                        client.DefaultRequestHeaders.CacheControl.Add(new HttpNameValueHeaderValue("no-cache"));
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept
                            .Add(HttpMediaTypeWithQualityHeaderValue.Parse("application/x-www-form-urlencoded"));

                        token.ThrowIfCancellationRequested();

                        try
                        {
                            var response = await client
                                .GetAsync(
                                    new Uri(baseUri + "poll/default"),
                                    HttpCompletionOption.ResponseHeadersRead
                                )
                                .AsTask(
                                    token
                                );

                            if (!response.IsSuccessStatusCode)
                            {
                                break;
                            }

                            if (HttpStatusCode.NoContent == response.StatusCode)
                            {
                                continue;
                            }

                            token.ThrowIfCancellationRequested();

                            var content = await response.Content.ReadAsStringAsync();
                            var decoder = new WwwFormUrlDecoder(content);

                            var messages = new Collection<RoomMessage>();
                            var count = int.Parse(decoder.GetFirstValueByName("count"));

                            for (var index = 0; index < count; index++)
                            {
                                var mid = Convert.ToInt64(decoder.GetFirstValueByName("[" + index + "_mid]"));
                                var pid = Guid.Parse(decoder.GetFirstValueByName("[" + index + "_pid]"));
                                var nick = decoder.GetFirstValueByName("[" + index + "_nick]");
                                var text = decoder.GetFirstValueByName("[" + index + "_text]");

                                messages.Add(new RoomMessage
                                {
                                    Id = mid,
                                    PublisherId = pid,
                                    PublisherNick = nick,
                                    Text = text
                                });
                            }

                            messageReceived.Invoke(this, new ReceivingMessageEventArgs(messages));
                        }
                        catch (COMException exception)
                        {
                            throw new AggregateException(exception);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                pollingCancelled.Invoke(this, new PollingCancelledEventArgs());
            }
        }
    }
}