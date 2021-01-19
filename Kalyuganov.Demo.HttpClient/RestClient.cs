using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kalyuganov.Demo.Http
{
    public class RestClient
    {
        protected readonly HttpClient _httpClient;


        public RestClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // return await _httpClient.GetOne<User>(domain, endpoint);
        public async Task<T> GetOne<T>(string domain, string endpoint)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{domain}/{endpoint}");

            var response = await Send(httpRequest);
            var stringResult = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<T>(stringResult);

            return result;
        }

        // return await _client.GetMany<Users>(domain, endpoint);
        public async Task<IEnumerable<T>> GetMany<T>(string domain, string endpoint)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{domain}/{endpoint}");

            var response = await Send(httpRequest);
            var stringResult = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IEnumerable<T>>(stringResult);

            return result;
        }

        // return await _client.Post<User>(domain, endpoint, user);
        public async Task<T> Post<T>(string domain, string endpoint, T resource)     
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{domain}/{endpoint}");

            string body = JsonConvert.SerializeObject(resource);

            httpRequest.Content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await Send(httpRequest);
            var stringResult = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<T>(stringResult);

            return result;
        }

        // return await _client.Put<User>(domain, endpoint, patch);
        public async Task<T> Put<T>(string serviceUrl, string endpoint, T patch)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"{serviceUrl}/{endpoint}");

            string body = JsonConvert.SerializeObject(patch);

            httpRequest.Content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await Send(httpRequest);
            var stringResult = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<T>(stringResult);

            return result;
        }

        // return await _client.Put<User, UserPatch>(domain, endpoint, patch);
        public async Task<R> Put<R, T>(string serviceUrl, string endpoint, T patch)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"{serviceUrl}/{endpoint}");

            string body = JsonConvert.SerializeObject(patch);

            httpRequest.Content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await Send(httpRequest);
            var stringResult = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<R>(stringResult);

            return result;
        }

        // Task<Unit>
        //   await _httpClient.Delete(domain, endpoint);
        // return Unit.Value;
        public Task Delete(string domain, string endpoint)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"{domain}/{endpoint}");
            return Send(httpRequest);
        }

        protected async Task<HttpResponseMessage> Send(HttpRequestMessage httpRequest)
        {
            var response = await _httpClient.SendAsync(httpRequest);

            if (response.IsSuccessStatusCode)
                return response;

            else
            {
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedException(responseString);
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new BadArgumentException(responseString);
                }
                else if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new ForbiddenException(responseString);
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new ResourceNotFoundException(responseString);
                }                
                else
                {
                    throw new Exception($"HTTP-{response.StatusCode}: {responseString}");
                }
            }
        }
    }
}