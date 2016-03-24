using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Devices.Sensors;
using Windows.Foundation;
using LibraProgramming.Windows.UI.Xaml.Commands;
using LibraTalk.Windows.Client.Properties;
using Newtonsoft.Json;

namespace LibraTalk.Windows.Client.Services
{
    [PublicAPI]
    public sealed class MessageSender : IMessageSender
    {
        private readonly Uri baseUri;
//        private readonly WeakEvent<TypedEventHandler<IMessageSender, ReceivingMessageEventArgs>> messageReceived;

        /*public event TypedEventHandler<IMessageSender, ReceivingMessageEventArgs> MessageReceived
        {
            add
            {
                messageReceived.AddHandler(value);
            }
            remove
            {
                messageReceived.RemoveHandler(value);
            }
        }*/

        public MessageSender([NotNull] Uri baseUri)
        {
            this.baseUri = baseUri;
//            messageReceived = new WeakEvent<TypedEventHandler<IMessageSender, ReceivingMessageEventArgs>>();
        }

        public async Task<string> GetUserName(Guid id)
        {
            var builder = new UriBuilder(baseUri);

            builder.Path += "me/" + id.ToString("D");

            var request = WebRequest.Create(builder.Uri);

            request.ContentType = "application/json";
            request.Method = HttpMethod.Get.ToString();
            request.Headers[HttpRequestHeader.CacheControl] = "no-cache";

            try
            {
                var response = (await request.GetResponseAsync()) as HttpWebResponse;

                if (null == response)
                {
                    return null;
                }

                if (HttpStatusCode.OK != response.StatusCode)
                {
                    return null;
                }

                var stream = response.GetResponseStream();

                using (var reader = new StreamReader(stream))
                {
                    var serializer = new JsonSerializer();
                    return serializer.Deserialize<string>(new JsonTextReader(reader));
                }
            }
            catch (Exception exception)
            {
                throw new AggregateException(exception);
            }
        }

        public async Task SetUserName(Guid id, string name)
        {
            var builder = new UriBuilder(baseUri);

            builder.Path += "me/" + id.ToString("D");

            var request = WebRequest.Create(builder.Uri);

            request.ContentType = "application/json";
            request.Method = HttpMethod.Post.ToString();

            try
            {
                var stream = await request.GetRequestStreamAsync();

                using (var writer = new StreamWriter(stream))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(writer, new {userName = name});
                }

                var response = (await request.GetResponseAsync()) as HttpWebResponse;

                if (null == response)
                {
                    throw new Exception();
                }

                if (HttpStatusCode.OK != response.StatusCode)
                {
                    throw new Exception();
                }
            }
            catch (Exception exception)
            {
                throw new AggregateException(exception);
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