using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MonoPatcher
{
    public class PatcherConfig
    {
        public string GameExe = "JinGu.exe";
        public string TargetAssembly = "Assembly-CSharp.dll";
        public string ManagedSubdir = "JinGu_Data/Managed";
        public string EntryType = "GameEntry";
        public string ModAssembly = "JinguMod.dll";
        public string BootstrapType = "JinguMod.Bootstrap";
        public string PatchNamespace = "JinguModPatch";
        public string PatchEntryType = "PatchEntry";
        public string ModsDir = "Mods";

        public string FullPatchTypeName => PatchNamespace + "." + PatchEntryType;
        public string GameProcessName => Path.GetFileNameWithoutExtension(GameExe);
        public string ModAssemblyName => Path.GetFileNameWithoutExtension(ModAssembly);

        public static PatcherConfig Load(string path)
        {
            var config = new PatcherConfig();
            if (!File.Exists(path))
                return config;

            var values = ParseSimpleJson(File.ReadAllText(path));
            string v;
            if (values.TryGetValue("gameExe", out v)) config.GameExe = v;
            if (values.TryGetValue("targetAssembly", out v)) config.TargetAssembly = v;
            if (values.TryGetValue("managedSubdir", out v)) config.ManagedSubdir = v;
            if (values.TryGetValue("entryType", out v)) config.EntryType = v;
            if (values.TryGetValue("modAssembly", out v)) config.ModAssembly = v;
            if (values.TryGetValue("bootstrapType", out v)) config.BootstrapType = v;
            if (values.TryGetValue("patchNamespace", out v)) config.PatchNamespace = v;
            if (values.TryGetValue("patchEntryType", out v)) config.PatchEntryType = v;
            if (values.TryGetValue("modsDir", out v)) config.ModsDir = v;
            return config;
        }

        static Dictionary<string, string> ParseSimpleJson(string json)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            int i = 0;

            while (i < json.Length)
            {
                SkipWhitespaceAndStructural();
                if (i >= json.Length) break;
                if (json[i] != '"') { i++; continue; }
                string key = ReadString();
                SkipWhitespaceAndStructural();
                if (i < json.Length && json[i] == ':') i++;
                SkipWhitespaceAndStructural();
                if (i < json.Length && json[i] == '"')
                {
                    string value = ReadString();
                    result[key] = value;
                }
                SkipWhitespaceAndStructural();
            }
            return result;

            void SkipWhitespaceAndStructural()
            {
                while (i < json.Length && " \t\n\r{},".IndexOf(json[i]) >= 0) i++;
            }

            string ReadString()
            {
                i++;
                var sb = new StringBuilder();
                while (i < json.Length && json[i] != '"')
                {
                    if (json[i] == '\\' && i + 1 < json.Length)
                    {
                        i++;
                        switch (json[i])
                        {
                            case '"': sb.Append('"'); break;
                            case '\\': sb.Append('\\'); break;
                            case '/': sb.Append('/'); break;
                            case 'n': sb.Append('\n'); break;
                            case 't': sb.Append('\t'); break;
                            default: sb.Append(json[i]); break;
                        }
                    }
                    else
                    {
                        sb.Append(json[i]);
                    }
                    i++;
                }
                if (i < json.Length) i++;
                return sb.ToString();
            }
        }
    }
}
