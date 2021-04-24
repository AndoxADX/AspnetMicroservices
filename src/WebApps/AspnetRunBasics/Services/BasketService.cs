using AspnetRunBasics.Extensions;
using AspnetRunBasics.Models;
using AspnetRunBasics.Utils;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AspnetRunBasics.Services
{
    public class BasketService : IBasketService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BasketService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<BasketModel> GetBasket(string userName)
        {
            var httpClient = _httpClientFactory.CreateClient(IdentityClient.ShopAPIClient);
            var request = new HttpRequestMessage(HttpMethod.Get, $"/Basket/{userName}");
            var response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            return await response.ReadContentAs<BasketModel>();
        }

        public async Task<BasketModel> UpdateBasket(BasketModel model)
        {
            var httpClient = _httpClientFactory.CreateClient(IdentityClient.ShopAPIClient);
            var request = new HttpRequestMessage(HttpMethod.Post, $"/Basket");
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            request.Content = content;

            var response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            return await response.ReadContentAs<BasketModel>();
        }

        public async Task CheckoutBasket(BasketCheckoutModel model)
        {
            var httpClient = _httpClientFactory.CreateClient(IdentityClient.ShopAPIClient);
            var request = new HttpRequestMessage(HttpMethod.Post, $"/Basket/Checkout");
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            request.Content = content;

            await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
        }
    }
}
