﻿using System;
using System.Collections.Generic;
using CommandLine;
using YaR.MailRuCloud.Api.Base;

namespace YaR.CloudMailRu.Console
{
    class CommandLineOptions
    {
        [Option('p', "port", Separator = ',', Required = false, Default = new[]{801}, HelpText = "WebDAV server port")]
        public IEnumerable<int> Port { get; set; }

        [Option('h', "host", Required = false, Default = "http://127.0.0.1", HelpText = "WebDAV server host, including protocol")]
        public string Host { get; set; }

        [Obsolete]
        [Option('l', "login", Required = false, HelpText = "Login to Mail.ru Cloud", Hidden = true)]
        public string Login { get; set; }

        [Obsolete]
        [Option('s', "password", Required = false, HelpText = "Password to Mail.ru Cloud", Hidden = true)]
        public string Password { get; set; }

        [Option("maxthreads", Default = 5, HelpText = "Maximum concurrent connections to cloud.mail.ru")]
        public int MaxThreadCount { get; set; }

        [Option("user-agent", HelpText = "\"browser\" user-agent")]
        public string UserAgent { get; set; }

        [Option("install", Required = false, HelpText = "install as Windows service")]
        public string ServiceInstall { get; set; }

        [Option("uninstall", Required = false, HelpText = "uninstall Windows service")]
        public string ServiceUninstall { get; set; }

        [Option("service", Required = false, Default = false, HelpText = "Started as a service")]
        public bool ServiceRun { get; set; }

        [Option("protocol", Default = Protocol.WebM1Bin, HelpText = "Cloud protocol")]
        public Protocol Protocol { get; set; }

        [Option("cache-listing", Default = 30, HelpText = "Cache folders listing, sec")]
        public int CacheListingSec { get; set; }

		[Option("cache-listing-depth", Default = 1, HelpText = "List query folder depth", Hidden = true)]
		public int CacheListingDepth { get; set; }
	}
}
