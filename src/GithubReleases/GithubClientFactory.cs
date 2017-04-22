﻿using Octokit;
using System;

namespace JeremyTCD.ContDeployer.Plugin.GithubReleases
{
    /// <summary>
    /// As suggested here - https://github.com/octokit/octokit.net/issues/1538
    /// </summary>
    public class GithubClientFactory : IGithubClientFactory
    {
        private string _serverUrl;
        private string _appName;

        public GithubClientFactory(string serverUrl, string appName)
        {
            _serverUrl = serverUrl;
            _appName = appName;
        }

        public IGitHubClient CreateClient(string apiKey)
        {
            return new GitHubClient(new ProductHeaderValue(_appName), new Uri(_serverUrl))
            {
                Credentials = new Credentials(apiKey)
            };
        }
    }
}