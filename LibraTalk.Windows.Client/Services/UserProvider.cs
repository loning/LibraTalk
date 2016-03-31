using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using LibraProgramming.Windows.UI.Xaml.Commands;
using LibraTalk.Windows.Client.Models;
using LibraTalk.Windows.Client.Properties;

namespace LibraTalk.Windows.Client.Services
{
    public class ReceivingMessageEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ComponentModel.CancelEventArgs"/> class with the <see cref="P:System.ComponentModel.CancelEventArgs.Cancel"/> property set to the given value.
        /// </summary>
        /// <param name="cancel">true to cancel the event; otherwise, false. </param>
        public ReceivingMessageEventArgs(bool cancel)
            : base(cancel)
        {
        }
    }

    [PublicAPI]
    public sealed class UserProvider
    {
        private readonly Uri baseUri;
        private readonly Guid userId;
        private readonly WeakEvent<TypedEventHandler<UserProvider, ReceivingMessageEventArgs>> messageReceived;

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

        public UserProvider([NotNull] Uri baseUri, [NotNull]Guid userId)
        {
            this.baseUri = baseUri;
            this.userId = userId;
            messageReceived = new WeakEvent<TypedEventHandler<UserProvider, ReceivingMessageEventArgs>>();
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

        /*
                public async Task SendMessageAsync([NotNull] IDictionary<string, string> message)
                {
                    if (null == message)
                    {
                        return;
                    }

                    var path = new UriBuilder(baseUri)
                    {
                        Query = "message"
                    };

                    var request = WebRequest.Create(path.Uri);

                    request.ContentType = "application/x-www-form-urlencoded";
                    request.Method = HttpMethod.Post.ToString();

                    using (var stream = await request.GetRequestStreamAsync())
                    {
                        var content = new FormUrlEncodedContent(message);
                        await content.CopyToAsync(stream);
                    }

                    var response = await request.GetResponseAsync();
                }
        */

        /*
                public void Receive()
                {
                    Task.Factory
                        .StartNew(DoReceiveInternalAsync)
                        .ConfigureAwait(false);
                }
        */

        /*
                private async Task DoReceiveInternalAsync()
                {
                    var ok = true;

                    while (ok)
                    {
                        var builder = new UriBuilder(baseUri);

                        builder.Path += "poll/12345";
                        builder.Query = "token=test";

                        var request = WebRequest.Create(builder.Uri);

                        request.ContentType = "application/xml";
                        request.Method = HttpMethod.Get.ToString();
                        request.Headers[HttpRequestHeader.CacheControl] = "no-cache";

                        try
                        {
                            var response = (await request.GetResponseAsync()) as HttpWebResponse;

                            if (null == response)
                            {
                                return;
                            }

                            if (HttpStatusCode.OK == response.StatusCode)
                            {
                                using (var reader = new StreamReader(response.GetResponseStream()))
                                {
                                    var content = reader.ReadToEnd();
                                    Debug.WriteLine(content);
                                }

                                var args = new ReceivingMessageEventArgs(false);

                                messageReceived.Invoke(this, args);

                                ok = false == args.Cancel;
                            }
                        }
                        catch (Exception exception)
                        {
                            Debugger.Break();
                        }
                    }
                }
        */
    }
}