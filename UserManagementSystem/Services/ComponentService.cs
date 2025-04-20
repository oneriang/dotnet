using System.Text.Json;
using System.Text.Json.Serialization;
using UserManagementSystem.Models;
using UserManagementSystem.Models.ComponentModels;

namespace UserManagementSystem.Services
{
    public class ComponentService
    {
        private readonly IWebHostEnvironment _env;
        
        public ComponentService(IWebHostEnvironment env)
        {
            _env = env;
        }
        
        public async Task<LayoutConfig> LoadLayoutAsync(string configName = "user-management")
        {
            var configPath = Path.Combine(_env.ContentRootPath, "Configurations", $"{configName}.jsonc");
            var json = await File.ReadAllTextAsync(configPath);
            json = RemoveJsonComments(json);
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };
            
            return JsonSerializer.Deserialize<LayoutConfig>(json, options);
        }
        
        public async Task<Dictionary<string, FormComponent>> LoadFormsAsync(string configName = "user-management")
        {
            var configPath = Path.Combine(_env.ContentRootPath, "Configurations", $"{configName}.jsonc");
            var json = await File.ReadAllTextAsync(configPath);
            json = RemoveJsonComments(json);
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };
            
            var config = JsonSerializer.Deserialize<ConfigRoot>(json, options);
            
            return config?.Forms ?? new Dictionary<string, FormComponent>();
        }
        
        private string RemoveJsonComments(string json)
        {
            var lines = json.Split('\n')
                .Where(line => !line.TrimStart().StartsWith("//"))
                .ToArray();
            return string.Join('\n', lines);
        }
    }
    
    public class ConfigRoot
    {
        public LayoutConfig Layout { get; set; }
        public Dictionary<string, FormComponent> Forms { get; set; }
    }
}
