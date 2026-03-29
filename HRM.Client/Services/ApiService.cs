using System.Net.Http.Json;
using System.Net.Http.Headers;
using Blazored.LocalStorage;
using HRM.Shared.DTOs;

namespace HRM.Client.Services;

public class ApiService
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;

    public ApiService(HttpClient http, ILocalStorageService localStorage)
    {
        _http = http;
        _localStorage = localStorage;
    }

    private async Task EnsureAuthHeaderAsync()
    {
        if (_http.DefaultRequestHeaders.Authorization is null)
        {
            var token = await _localStorage.GetItemAsStringAsync("authToken");
            if (!string.IsNullOrWhiteSpace(token))
            {
                token = token.Trim('"');
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }

    public async Task<T?> GetAsync<T>(string url)
    {
        await EnsureAuthHeaderAsync();
        return await _http.GetFromJsonAsync<T>(url);
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data)
    {
        await EnsureAuthHeaderAsync();
        var response = await _http.PostAsJsonAsync(url, data);
        return await response.Content.ReadFromJsonAsync<TResponse>();
    }

    public async Task<TResponse?> PutAsync<TRequest, TResponse>(string url, TRequest data)
    {
        await EnsureAuthHeaderAsync();
        var response = await _http.PutAsJsonAsync(url, data);
        return await response.Content.ReadFromJsonAsync<TResponse>();
    }

    public async Task<ApiResponse<string>?> DeleteAsync(string url)
    {
        await EnsureAuthHeaderAsync();
        var response = await _http.DeleteAsync(url);
        return await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
    }

    public async Task<ApiResponse<string>?> PostFileAsync(string url, MultipartFormDataContent content)
    {
        await EnsureAuthHeaderAsync();
        var response = await _http.PostAsync(url, content);
        return await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
    }
}
