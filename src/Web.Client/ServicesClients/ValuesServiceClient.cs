﻿namespace Web.Client.ServicesClients
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Skeleton.Web.Integration;

    public class ValuesServiceClient : BaseClient
    {
        public ValuesServiceClient(ClientConfiguration configuration) : base(configuration)
        {
        }

        public ValuesServiceClient(HttpMessageHandler messageHandler, ClientConfiguration configuration) : base(messageHandler, configuration)
        {
        }

        public IEnumerable<string> Get()
        {
            return Get<IEnumerable<string>>("api/values");
        }

        public Task<IEnumerable<string>> GetAsync()
        {
            return GetAsync<IEnumerable<string>>("api/values");
        }

        public string Get(int id)
        {
            return Get<string>($"api/values/{id}");
        }

        public Task<string> GetAsync(int id)
        {
            return GetAsync<string>($"api/values/{id}");
        }

        public void Set(int id, string value)
        {
            Put($"api/values/{id}", value);
        }

        public Task SetAsync(int id, string value)
        {
            return PutAsync($"api/values/{id}", value);
        }

        public void Delete(int id)
        {
            Delete($"api/values/{id}");
        }

        public Task DeleteAsync(int id)
        {
            return DeleteAsync($"api/values/{id}");
        }
    }
}