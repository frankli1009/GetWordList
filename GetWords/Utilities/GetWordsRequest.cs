using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using GetWords.Models;
using Newtonsoft.Json;

namespace GetWords.Utilities
{
    public class GetWordsRequest
    {
        private readonly IHttpClientFactory _clientFactory;

        public GetWordsRequest(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<Tuple<string, List<string>>> OnGetWordsRequest(string url, WordRequirement wr)
        {
            string errMsg = null;

            string urlAPI = url + "/" + wr.Letters;
            urlAPI += "/" + $"{wr.Length}";
            if (!(string.IsNullOrEmpty(wr.ExtraRegEx)))
            {
                urlAPI += "/" + HttpUtility.UrlEncode(wr.ExtraRegEx);
            }
            if (wr.Rejects.Any())
            {
                var rejectedJson = JsonConvert.SerializeObject(wr.Rejects);
                urlAPI += "?rejected=" + HttpUtility.UrlEncode(rejectedJson);
            }

            var request = new HttpRequestMessage(HttpMethod.Get,urlAPI);

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return new Tuple<string, List<string>>(null, JsonConvert.DeserializeObject<List<string>>(await response.Content.ReadAsStringAsync()));
            }
            else
            {
                errMsg = $"Failed to request to api with status code {response.StatusCode}.";
                return new Tuple<string, List<string>>(errMsg, null);
            }
        }
    }
}
