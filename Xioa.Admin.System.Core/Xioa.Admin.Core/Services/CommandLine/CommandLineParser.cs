using System;
using System.Collections.Generic;
using System.Linq;


namespace Xioa.Admin.Core.Services.CommandLine;

public class CommandLineParser {
    private readonly Dictionary<string, string> _parameters;

    public CommandLineParser(string[] args) {
        _parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        ParseArguments(args);
    }

    private void ParseArguments(string[] args) {
        for (int i = 0; i < args.Length; i++)
        {
            string arg = args[i];
            if (arg.StartsWith("-") || arg.StartsWith("--"))
            {
                string key = arg.TrimStart('-');
                string value = null;

                // 检查下一个参数是否是值
                if (i + 1 < args.Length && !args[i + 1].StartsWith("-"))
                {
                    value = args[++i];
                }

                _parameters[key] = value ?? "true";
            }
        }
    }

    public bool HasParameter(string name) {
        return _parameters.ContainsKey(name);
    }

    public string GetValue(string name, string defaultValue = null) {
        return _parameters.TryGetValue(name, out string value) ? value : defaultValue;
    }

    public int GetIntValue(string name, int defaultValue = 0) {
        if (_parameters.TryGetValue(name, out string value) &&
            int.TryParse(value, out int result))
        {
            return result;
        }

        return defaultValue;
    }

    public bool GetBoolValue(string name, bool defaultValue = false) {
        if (_parameters.TryGetValue(name, out string value))
        {
            return value.Equals("true", StringComparison.OrdinalIgnoreCase);
        }

        return defaultValue;
    }
    
    
    public float GetFloatValue(string name, float defaultValue = 0.0f)
    {
        if (_parameters.TryGetValue(name, out string value) && 
            float.TryParse(value, out float result))
        {
            return result;
        }
        return defaultValue;
    }

    public T GetEnumValue<T>(string name, T defaultValue) where T : struct
    {
        if (_parameters.TryGetValue(name, out string value) && 
            Enum.TryParse<T>(value, true, out T result))
        {
            return result;
        }
        return defaultValue;
    }

    public IEnumerable<string> GetValues(string name)
    {
        return _parameters.Keys.Where(k => k.StartsWith(name + "["));
    }
}