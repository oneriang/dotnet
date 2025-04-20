using System.Text.Json;
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
            
            return JsonSerializer.Deserialize<LayoutConfig>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        
        public async Task<Dictionary<string, FormComponent>> LoadFormsAsync(string configName = "user-management")
        {
            var configPath = Path.Combine(_env.ContentRootPath, "Configurations", $"{configName}.jsonc");
            var json = await File.ReadAllTextAsync(configPath);
            json = RemoveJsonComments(json);
            
            var config = JsonSerializer.Deserialize<ConfigRoot>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            return config.Forms;
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
