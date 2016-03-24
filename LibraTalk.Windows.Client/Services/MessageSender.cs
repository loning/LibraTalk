using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Foundation;
using LibraProgramming.Windows.UI.Xaml.Commands;
using LibraTalk.Windows.Client.Properties;

namespace LibraTalk.Windows.Client.Services
{
    [PublicAPI]
    public sealed class MessageSender : IMessageSender
    {
        private readonly Uri baseUri;
        private readonly WeakEvent<TypedEventHandler<IMessageSender, ReceivingMessageEventArgs>> messageReceived;

        public event TypedEventHandler<IMessageSender, ReceivingMessageEventArgs> MessageReceived
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

        public MessageSender([NotNull] Uri baseUri)
        {
            this.baseUri = baseUri;
            messageReceived = new WeakEvent<TypedEventHandler<IMessageSender, ReceivingMessageEventArgs>>();
        }

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

        public void Receive()
        {
            Task.Factory
                .StartNew(DoReceiveInternalAsync)
                .ContinueWith(DoFireReceiveEvent)
                .ConfigureAwait(false);
        }

        private async Task DoReceiveInternalAsync()
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
                }
            }
            catch (Exception exception)
            {
                Debugger.Break();
            }
        }

        private void DoFireReceiveEvent(Task<Task> obj)
        {
            messageReceived.Invoke(this, new ReceivingMessageEventArgs());
        }
    }
}