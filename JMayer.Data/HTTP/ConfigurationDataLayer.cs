﻿using JMayer.Data.Data;
using System.Net;
using System.Net.Http.Json;

#warning I feel like there should be more to this. Maybe configuration can do export/import?

namespace JMayer.Data.HTTP
{
    /// <summary>
    /// The class for interacting with remote data via HTTP using CRUD operations.
    /// </summary>
    /// <typeparam name="T">A ConfigurationDataObject which represents data on the remote server.</typeparam>
    public class ConfigurationDataLayer<T> : DataLayer<T>, IConfigurationDataLayer<T> where T : ConfigurationDataObject
    {
        /// <summary>
        /// The method returns all the remote data objects as a list view.
        /// </summary>
        /// <returns>A list of DataObjects.</returns>
        public async Task<List<ListView>?> GetAllListViewAsync()
        {
            List<ListView>? listView = [];
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync($"{_typeName}/All/ListView");

            if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
            {
                listView = await httpResponseMessage.Content.ReadFromJsonAsync<List<ListView>?>();
            }

            return listView;
        }
    }
}
