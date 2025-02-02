using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace deepbim
{
    public static class DeepSeekService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string ApiBaseUrl = "DEEPBIM_API_URL";

        public static async Task<string> GetResponse(string userQuestion, CancellationToken cancellationToken = default)
        {
            try
            {
                // Match FastAPI's request format with "user_input" key
                var requestData = new
                {
                    user_input = userQuestion // Changed from "question" to "user_input"
                };

                var json = JsonSerializer.Serialize(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(ApiBaseUrl, content, cancellationToken);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseContent);

                // Handle potential API error messages
                if (apiResponse?.Response?.StartsWith("Error:") ?? false)
                {
                    Console.WriteLine($"API Error: {apiResponse.Response}");
                    return "Sorry, I encountered an error processing your request.";
                }

                return apiResponse?.Response ?? "No response received.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Network/System Error: {ex.Message}");
                return "Sorry, I'm having trouble connecting to the service.";
            }
        }

        private class ApiResponse
        {
            [JsonPropertyName("response")]
            public string Response { get; set; }
        }
    }
}