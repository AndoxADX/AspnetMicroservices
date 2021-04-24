using AspnetRunBasics.Extensions;
using AspnetRunBasics.Models;
using AspnetRunBasics.Utils;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspnetRunBasics.Services
{
    public class OrderService : IOrderService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public OrderService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<IEnumerable<OrderResponseModel>> GetOrdersByUserName(string userName)
        {
            var httpClient = _httpClientFactory.CreateClient(IdentityClient.ShopAPIClient);
            var request = new HttpRequestMessage(HttpMethod.Get, $"/Order/{userName}");
            var response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            return await response.ReadContentAs<List<OrderResponseModel>>();
        }
    }
}
