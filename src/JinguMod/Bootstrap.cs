using System;
using System.IO;
using System.Reflection;

namespace JinguMod
{
    public static class Bootstrap
    {
        public static readonly bool Loaded;
        public static readonly string Status;

        static Bootstrap()
        {
            try
            {
                var modsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mods");
                if (Directory.Exists(modsDir))
                {
                    foreach (var dllPath in Directory.GetFiles(modsDir, "*.dll"))
                    {
                        try
                        {
                            var patchAsm = Assembly.LoadFrom(dllPath);
                            var entryType = patchAsm.GetType("JinguModPatch.PatchEntry");
                            if (entryType != null)
                            {
                                var loadedField = entryType.GetField("Loaded");
                                if (loadedField != null)
                                    loadedField.GetValue(null);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[JinguMod] Failed to load {Path.GetFileName(dllPath)}: {ex.Message}");
                        }
                    }
                }
                Status = "OK";
            }
            catch (Exception ex)
            {
                Status = ex.GetType().Name + ": " + ex.Message;
            }
            finally { Loaded = true; }
        }
    }
}
