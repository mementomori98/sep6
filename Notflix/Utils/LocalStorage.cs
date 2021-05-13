using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Notflix.Utils
{
    public class LocalStorage
    {

        private readonly IJSRuntime _jsRuntime;

        public LocalStorage(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<T> Get<T>(string key)
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
            if (json == null)
                return default;

            return JsonSerializer.Deserialize<T>(json);
        }

        public async Task Set<T>(string key, T item)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, JsonSerializer.Serialize(item));
        }

        public async Task Remove(string key)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
        }
    }
}