using Aqorn.Models;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Aqorn;

internal class Options : IOptions
{
    public string SpecFile { get; } = null!;
    public List<string> DataFiles { get; } = [];
    public Dictionary<string, string> Parameters { get; } = [];

    public int InsertBatchSize { get; } = 100;

    public Options(IErrorLog errors, Queue<string> args)
    {
        if (args.Count == 0)
        {
            errors.Add("No arguments provided.");
            return;
        }

        // Parse input
        var hadArg = false;
        while (errors.ErrorCount == 0 && args.Count > 0)
        {
            if (popKey(out var key, out var suffix))
            {
                switch (key)
                {
                    case "-s":
                    case "--spec":
                        if (!popValue(out var specfile))
                        {
                            errors.Add($"Missing value for '{key}'.");
                        }
                        else if (SpecFile != null)
                        {
                            errors.Add("Multiple specification files given.");
                        }
                        else
                        {
                            SpecFile = specfile;
                        }
                        break;

                    case "-d":
                    case "--data":
                        hadArg = true;
                        if (!popValue(out var datafile))
                        {
                            errors.Add($"Missing value for '{key}'.");
                            break;
                        }
                        while (datafile != null)
                        {
                            DataFiles.Add(datafile);
                            popValue(out datafile);
                        }
                        break;

                    case "-p":
                    case "--param":
                        if (!popValue(out var value))
                        {
                            errors.Add($"Missing value for parameter '{suffix ?? key}'.");
                            break;
                        }
                        if (suffix != null)
                        {
                            Parameters[suffix] = value;
                            break;
                        }

                        if (value[0] != '{')
                        {
                            // Get object from JSON file
                            if (!File.Exists(value))
                            {
                                errors.Add($"Parameter file not found: {value}");
                                break;
                            }
                            try
                            {
                                value = File.ReadAllText(value);
                            }
                            catch
                            {
                                errors.Add($"Unable to read JSON file for parameter '{suffix}'.");
                                break;
                            }
                        }

                        // Parse JSON dictionary
                        try
                        {
                            var dic = JsonSerializer.Deserialize<Dictionary<string, string>>(value);
                            foreach (var (k, v) in dic ?? [])
                            {
                                Parameters[k] = v;
                            }
                        }
                        catch
                        {
                            errors.Add($"Invalid JSON dictionary for parameter '{suffix}'.");
                        }
                        break;
                }
                continue;
            }

            if (hadArg)
            {
                errors.Add("Unexpected value: " + key);
            }
            else if (key != null)
            {
                if (SpecFile == null)
                {
                    SpecFile = key;
                }
                else
                {
                    DataFiles.Add(key);
                }
            }
        }

        if (SpecFile == null)
        {
            errors.Add("No specification file provided.");
        }
        if (DataFiles.Count == 0)
        {
            errors.Add("No data files provided.");
        }

        bool popKey([MaybeNullWhen(false)] out string key, out string? suffix)
        {
            suffix = null;
            if (!args.TryDequeue(out key)
                || key[0] != '-')
            {
                return false;
            }

            var col = key.IndexOf(':');
            if (col >= 0 && col != key.Length - 1)
            {
                suffix = key[(col + 1)..];
                key = key[..col];
            }
            return true;
        }
        bool popValue([MaybeNullWhen(false)] out string value)
        {
            if (!args.TryPeek(out value)
                || value[0] == '-')
            {
                return false;
            }
            args.Dequeue();
            return true;
        }
    }
}