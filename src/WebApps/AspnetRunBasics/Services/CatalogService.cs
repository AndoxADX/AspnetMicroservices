using AspnetRunBasics.Extensions;
using AspnetRunBasics.Models;
using AspnetRunBasics.Utils;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AspnetRunBasics.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CatalogService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<UserInfoViewModel> GetUserInfo()
        {
            var idpClient = _httpClientFactory.CreateClient(IdentityClient.IDPClient);

            var metaDataResponse = await idpClient.GetDiscoveryDocumentAsync();

            if (metaDataResponse.IsError)
            {
                throw new HttpRequestException("Something went wrong while requesting the access token");
            }

            var accessToken = await _httpContextAccessor
                .HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            var userInfoResponse = await idpClient.GetUserInfoAsync(
               new UserInfoRequest
               {
                   Address = metaDataResponse.UserInfoEndpoint,
                   Token = accessToken
               });

            if (userInfoResponse.IsError)
            {
                throw new HttpRequestException("Something went wrong while getting user info");
            }

            var userInfoDictionary = new Dictionary<string, string>();

            foreach (var claim in userInfoResponse.Claims)
            {
                userInfoDictionary.Add(claim.Type, claim.Value);
            }

            return new UserInfoViewModel(userInfoDictionary);
        }

        public async Task<IEnumerable<CatalogModel>> GetCatalog()
        {
            var httpClient = _httpClientFactory.CreateClient(IdentityClient.ShopAPIClient);
            var request = new HttpRequestMessage(HttpMethod.Get, $"/Catalog");
            var response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            return await response.ReadContentAs<List<CatalogModel>>();
        }

        public async Task<CatalogModel> GetCatalog(string id)
        {
            var httpClient = _httpClientFactory.CreateClient(IdentityClient.ShopAPIClient);
            var request = new HttpRequestMessage(HttpMethod.Get, $"/Catalog/{id}");
            var response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            return await response.ReadContentAs<CatalogModel>();
        }

        public async Task<IEnumerable<CatalogModel>> GetCatalogByCategory(string category)
        {
            var httpClient = _httpClientFactory.CreateClient(IdentityClient.ShopAPIClient);
            var request = new HttpRequestMessage(HttpMethod.Get, $"/Catalog/GetProductByCategory/{category}");
            var response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            return await response.ReadContentAs<List<CatalogModel>>();
        }

        public async Task<CatalogModel> CreateCatalog(CatalogModel model)
        {
            var httpClient = _httpClientFactory.CreateClient(IdentityClient.ShopAPIClient);
            var request = new HttpRequestMessage(HttpMethod.Post, $"/Catalog");
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            request.Content = content;
            var response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<CatalogModel>();
            else
            {
                throw new Exception("Something went wrong when calling api.");
            }
        }
    }
}
