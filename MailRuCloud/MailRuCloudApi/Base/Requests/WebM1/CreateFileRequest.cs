﻿using System;
using System.Net;
using System.Text;
using YaR.MailRuCloud.Api.Base.Requests.Repo;

namespace YaR.MailRuCloud.Api.Base.Requests.WebM1
{
   class CreateFileRequest : BaseRequestJson<WebV2.CreateFileRequest.Result>
    {
        private readonly string _fullPath;
        private readonly string _hash;
        private readonly long _size;
        private readonly ConflictResolver _conflictResolver;

        public CreateFileRequest(IWebProxy proxy, IAuth auth, string fullPath, string hash, long size, ConflictResolver? conflictResolver) 
            : base(proxy, auth)
        {
            _fullPath = fullPath;
            _hash = hash;
            _size = size;
            _conflictResolver = conflictResolver ?? ConflictResolver.Rename;
        }

        protected override string RelationalUri => "/api/m1/file/add";

        protected override byte[] CreateHttpContent()
        {
            string filePart = $"&hash={_hash}&size={_size}";
            string data = $"home={Uri.EscapeDataString(_fullPath)}&conflict={_conflictResolver}&api=2&access_token={Auth.AccessToken}" + filePart;

            return Encoding.UTF8.GetBytes(data);
        }
    }
}
