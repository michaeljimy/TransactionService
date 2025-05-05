using System.Net.Http.Headers;
using System.Text.Json;
using Transaction_Service.Models.DTOs;
using Transaction_Service.Models.DTOs.Requests;
using Transaction_Service.Repositories.Interfaces;

namespace Transaction_Service.Services
{
    public class WalletService: IServiceWallet
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WalletService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<WalletWithTypeDto> GetWalletAsync(Guid walletId)
        {
            // Remove the "Bearer " part
            var token = GetBearerTokenFromCurrentContext();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Send the request to the Wallet Service
            
            var response = await _httpClient.GetAsync($"{walletId}");
            response.EnsureSuccessStatusCode();

            // Deserialize the JSON response into your DTO
            var json = await response.Content.ReadAsStringAsync();
            var walletWithType = JsonSerializer.Deserialize<WalletWithTypeDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return walletWithType;
        }

        public async Task<WalletDto> GetSentWalletAsync(Guid walletId)
        {
            // Remove the "Bearer " part
            var token = GetBearerTokenFromCurrentContext();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Send the request to the Wallet Service

            var response = await _httpClient.GetAsync($"sent/{walletId}");
            response.EnsureSuccessStatusCode();

            // Deserialize the JSON response into your DTO
            var json = await response.Content.ReadAsStringAsync();
            var wallet = JsonSerializer.Deserialize<WalletDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return wallet;
        }

        public async Task<WalletDto> updateWalletAsync(WalletUpdateRequest request)
        {
             string token = GetBearerTokenFromCurrentContext();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Post the update request JSON to the Wallet API endpoint.
            // The URL should match your Wallet API route.
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("balance", request);
            response.EnsureSuccessStatusCode();

            // Deserialize the response as a WalletDto.
            
            if (!response.IsSuccessStatusCode)
            {
                // Log or handle the error as needed.
                throw new Exception($"API call failed with status code {response.StatusCode}");
            }

            var contentString = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(contentString))
            {
                throw new Exception("Response content is empty.");
            }
            WalletDto walletDto = await response.Content.ReadFromJsonAsync<WalletDto>();
            return walletDto;

        }

        private string GetBearerTokenFromCurrentContext()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                throw new System.Exception("HTTP Context not found.");
            }
            string authorizationHeader = httpContext.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                throw new System.UnauthorizedAccessException("Bearer token not found in the header.");
            }
            return authorizationHeader.Substring("Bearer ".Length).Trim();
        }

    }
}
