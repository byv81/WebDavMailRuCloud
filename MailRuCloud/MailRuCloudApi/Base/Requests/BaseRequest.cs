﻿using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace YaR.MailRuCloud.Api.Base.Requests
{
    public abstract class BaseRequest<TConvert, T> where T : class
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(typeof(BaseRequest<TConvert, T>));

        protected readonly CloudApi CloudApi;

        protected BaseRequest(CloudApi cloudApi)
        {
            CloudApi = cloudApi;
        }

        protected abstract string RelationalUri { get; }

        protected virtual HttpWebRequest CreateRequest(string baseDomain = null)
        {
            string domain = string.IsNullOrEmpty(baseDomain) ? ConstSettings.CloudDomain : baseDomain;
            var uriz = new Uri(new Uri(domain), RelationalUri);
            
            // supressing escaping is obsolete and breaks, for example, chinese names
            // url generated for %E2%80%8E and %E2%80%8F seems ok, but mail.ru replies error
            // https://stackoverflow.com/questions/20211496/uri-ignore-special-characters
            //var udriz = new Uri(new Uri(domain), RelationalUri, true);

            var request = (HttpWebRequest)WebRequest.Create(uriz);
            request.Proxy = CloudApi.Account.Proxy;
            request.CookieContainer = CloudApi.Account.Cookies;
            request.Method = "GET";
            request.ContentType = ConstSettings.DefaultRequestType;
            request.Accept = "application/json";
            request.UserAgent = ConstSettings.UserAgent;

            return request;
        }

        protected virtual byte[] CreateHttpContent()
        {
            return null;
        }


        public async Task<T> MakeRequestAsync()
        {
            var httprequest = CreateRequest();

            var content = CreateHttpContent();
            if (content != null)
            {
                httprequest.Method = "POST";
                var stream = httprequest.GetRequestStream();
                stream.Write(content, 0, content.Length);
            }
            Logger.Debug($"HTTP:{httprequest.Method}:{httprequest.RequestUri.AbsoluteUri}");

            using (var response = (HttpWebResponse)await Task.Factory.FromAsync(httprequest.BeginGetResponse, asyncResult => httprequest.EndGetResponse(asyncResult), null))
            {
                if ((int)response.StatusCode >= 500)
                    throw new RequestException("Server fault") {StatusCode = response.StatusCode}; // Let's throw exception. It's server fault

                RequestResponse<T> result;
                using (var responseStream = response.GetResponseStream())
                {
                    result = DeserializeMessage(Transport(responseStream));
                }

                if (!result.Ok || response.StatusCode != HttpStatusCode.OK)
                {
                    var exceptionMessage = $"Request failed (status code {(int)response.StatusCode}): {result.Description}";
                    throw new RequestException(exceptionMessage)
                    {
                        StatusCode = response.StatusCode,
                        ResponseBody = string.Empty, //responseText,
                        Description = result.Description,
                        ErrorCode = result.ErrorCode
                    };
                }
                var retVal = result.Result;
                return retVal;
            }
        }

        protected abstract TConvert Transport(Stream stream);

        protected abstract RequestResponse<T> DeserializeMessage(TConvert data);
    }
}