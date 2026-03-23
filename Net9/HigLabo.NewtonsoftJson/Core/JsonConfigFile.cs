using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace HigLabo.Core;

public class JsonConfigFile
{
    private JObject? _JObject;

    public String Json { get; set; } = "";


    public static JsonConfigFile CreateFromFile(String filePath)
    {
        var json = System.IO.File.ReadAllText(filePath);
        return CreateFromJson(json);
    }
    public static JsonConfigFile CreateFromJson(String json)
    {
        var jo = JsonConvert.DeserializeObject<JObject>(json);
        if (jo == null) { throw new ArgumentException("json is invalid.", nameof(json)); }

        var setting = new JsonConfigFile();
        setting._JObject = jo;
        setting.Json = json;
        return setting;
    }
    public String GetValue(String key)
    {
        var v = GetValueOrEmpty(key);
        if (v == null) { throw new KeyNotFoundException($"Key not found. key is {key}."); }
        return v;
    }
    public Guid GetGuidValue(String key)
    {
        return Guid.Parse(this.GetValue(key));
    }
    public String GetValueOrEmpty(String key)
    {
        try
        {
            var jo = this._JObject!;
            if (jo.TryGetValue(key, out var o))
            {
                return o.ToString();
            }
        }
        catch { }
        return "";
    }
    public T CreateObject<T>(String key)
    {
        if (_JObject == null) { throw new InvalidOperationException(); }
        if (_JObject.TryGetValue(key, out var v))
        {
            if (v != null)
            {
                var o = JsonConvert.DeserializeObject<T>(v.ToString());
                if (o != null) { return o; }
            }
        }
        throw new ArgumentException($"Key not found. key is {key}.", nameof(key));
    }
    public Dictionary<string, string> CreateDictionary(String key)
    {
        return CreateObject<Dictionary<string, string>>(key);
    }
    public string GetDictionaryValue(String key, string dictionaryKey)
    {
        var d = CreateObject<Dictionary<string, string>>(key);
        return d[dictionaryKey];
    }
}
