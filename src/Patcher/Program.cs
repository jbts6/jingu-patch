using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace JinguPatcher
{
    class Program
    {
        const int RetryCount = 5;
        const int RetryDelayMs = 1000;

        static int Main(string[] args)
        {
            string gameDir = args.Length > 0 ? args[0] : FindGameDirectory();
            if (gameDir == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR] Cannot find game directory.");
                Console.WriteLine("        Place JinguPatcher folder inside the game root directory.");
                Console.ResetColor();
                PauseIfDoubleClick();
                return 1;
            }

            string toolDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);
            string managedDir = Path.Combine(gameDir, "JinGu_Data", "Managed");
            string assemblyPath = Path.Combine(managedDir, "Assembly-CSharp.dll");
            string gameModsDir = Path.Combine(gameDir, "Mods");
            string backupPath = assemblyPath + ".bak";

            if (!File.Exists(assemblyPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] Assembly-CSharp.dll not found: {assemblyPath}");
                Console.ResetColor();
                PauseIfDoubleClick();
                return 1;
            }

            CheckGameNotRunning();

            Console.WriteLine($"Game directory : {gameDir}");
            Console.WriteLine($"Tool directory : {toolDir}");
            Console.WriteLine();

            DeployFiles(toolDir, gameDir, managedDir, gameModsDir);

            var patchDlls = Directory.GetFiles(gameModsDir, "*.dll")
                .Where(f => !Path.GetFileName(f).Equals("JinguMod.dll", StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (patchDlls.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[WARN] No patch DLLs found in Mods/ directory.");
                Console.ResetColor();
                PauseIfDoubleClick();
                return 0;
            }

            Console.WriteLine($"Patch DLLs    : {patchDlls.Count}");
            Console.WriteLine();

            if (!File.Exists(backupPath))
            {
                Console.WriteLine($"Backing up original -> {backupPath}");
                RetryFileOp(() => File.Copy(assemblyPath, backupPath));
            }
            else
            {
                Console.WriteLine("Restoring from backup before patching...");
                RetryFileOp(() => File.Copy(backupPath, assemblyPath, overwrite: true));
            }

            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(managedDir);
            resolver.AddSearchDirectory(gameModsDir);

            int totalPatches = 0;
            string tempPath = assemblyPath + ".tmp";

            var readerParams = new ReaderParameters { AssemblyResolver = resolver };
            using (var gameAsm = AssemblyDefinition.ReadAssembly(assemblyPath, readerParams))
            {
                var gameModule = gameAsm.MainModule;

                foreach (var patchDllPath in patchDlls)
                {
                    Console.WriteLine($"--- Processing: {Path.GetFileName(patchDllPath)} ---");
                    try
                    {
                        using (var patchAsm = AssemblyDefinition.ReadAssembly(patchDllPath,
                            new ReaderParameters { AssemblyResolver = resolver }))
                        {
                            foreach (var patchType in patchAsm.MainModule.GetTypes())
                            {
                                foreach (var patchMethod in patchType.Methods)
                                {
                                    if (!patchMethod.IsStatic || !patchMethod.IsPublic)
                                        continue;

                                    var parsed = ParsePatchMethodName(patchMethod.Name);
                                    if (parsed == null)
                                        continue;

                                    string kind = parsed.Value.kind;
                                    string targetTypeName = parsed.Value.typeName;
                                    string targetMethodName = parsed.Value.methodName;

                                    var targetType = gameModule.GetType(targetTypeName);
                                    if (targetType == null)
                                    {
                                        targetType = gameModule.GetTypes()
                                            .FirstOrDefault(t => t.Name == targetTypeName);
                                    }
                                    if (targetType == null)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Yellow;
                                        Console.WriteLine($"  [SKIP] Type not found: {targetTypeName}");
                                        Console.ResetColor();
                                        continue;
                                    }

                                    var targetMethod = targetType.Methods
                                        .FirstOrDefault(m => m.Name == targetMethodName);
                                    if (targetMethod == null)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Yellow;
                                        Console.WriteLine($"  [SKIP] Method not found: {targetTypeName}.{targetMethodName}");
                                        Console.ResetColor();
                                        continue;
                                    }

                                    var importedPatch = gameModule.ImportReference(patchMethod);
                                    InjectPatch(gameModule, targetMethod, importedPatch, kind);

                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine($"  [OK] {kind}_{targetTypeName}_{targetMethodName}");
                                    Console.ResetColor();
                                    totalPatches++;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"  [ERROR] {ex.Message}");
                        Console.ResetColor();
                    }
                }

                InjectGameEntryCctor(gameModule);

                if (File.Exists(tempPath)) File.Delete(tempPath);
                gameAsm.Write(tempPath);
            }

            RetryFileOp(() => File.Copy(tempPath, assemblyPath, overwrite: true));
            File.Delete(tempPath);

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Done! {totalPatches} patch(es) injected.");
            Console.ResetColor();
            PauseIfDoubleClick();
            return 0;
        }

        static void DeployFiles(string toolDir, string gameDir, string managedDir, string gameModsDir)
        {
            Console.WriteLine("--- Deploying files ---");

            string toolJinguMod = Path.Combine(toolDir, "JinguMod.dll");
            if (File.Exists(toolJinguMod))
            {
                string dest = Path.Combine(managedDir, "JinguMod.dll");
                Console.WriteLine($"  JinguMod.dll -> {dest}");
                RetryFileOp(() => File.Copy(toolJinguMod, dest, overwrite: true));
            }

            string toolModsDir = Path.Combine(toolDir, "Mods");
            if (Directory.Exists(toolModsDir))
            {
                if (!Directory.Exists(gameModsDir))
                    Directory.CreateDirectory(gameModsDir);

                foreach (var src in Directory.GetFiles(toolModsDir, "*.dll"))
                {
                    string dest = Path.Combine(gameModsDir, Path.GetFileName(src));
                    Console.WriteLine($"  Mods\\{Path.GetFileName(src)} -> {dest}");
                    RetryFileOp(() => File.Copy(src, dest, overwrite: true));
                }
            }

            Console.WriteLine();
        }

        static void CheckGameNotRunning()
        {
            var procs = Process.GetProcessesByName("JinGu");
            if (procs.Length > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR] JinGu.exe is still running! Please close the game first.");
                Console.ResetColor();
                foreach (var p in procs)
                {
                    try { Console.WriteLine($"  PID: {p.Id}"); } finally { p.Dispose(); }
                }
                PauseIfDoubleClick();
                Environment.Exit(2);
            }
        }

        static void RetryFileOp(Action action, string description = null)
        {
            for (int attempt = 1; attempt <= RetryCount; attempt++)
            {
                try
                {
                    action();
                    return;
                }
                catch (IOException ex) when (attempt < RetryCount)
                {
                    string msg = description ?? ex.Message;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"  [RETRY {attempt}/{RetryCount}] File locked: {msg}");
                    Console.ResetColor();
                    Thread.Sleep(RetryDelayMs);
                }
            }

            try
            {
                action();
            }
            catch (IOException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] File still locked after {RetryCount} retries: {ex.Message}");
                Console.WriteLine("Make sure the game and Steam are fully closed, then try again.");
                Console.ResetColor();
                PauseIfDoubleClick();
                Environment.Exit(3);
            }
        }

        static void PauseIfDoubleClick()
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }

        static string FindGameDirectory()
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            while (dir != null)
            {
                if (File.Exists(Path.Combine(dir, "JinGu.exe")))
                    return dir;
                dir = Path.GetDirectoryName(dir);
            }
            return null;
        }

        static (string kind, string typeName, string methodName)? ParsePatchMethodName(string name)
        {
            int i1 = name.IndexOf('_');
            if (i1 < 0) return null;
            string kind = name.Substring(0, i1);
            if (kind != "Prefix" && kind != "Postfix") return null;

            string rest = name.Substring(i1 + 1);
            int i2 = rest.IndexOf('_');
            if (i2 < 0) return null;

            string typeName = rest.Substring(0, i2);
            string methodName = rest.Substring(i2 + 1);
            if (string.IsNullOrEmpty(typeName) || string.IsNullOrEmpty(methodName)) return null;

            return (kind, typeName, methodName);
        }

        static void InjectPatch(ModuleDefinition module, MethodDefinition target,
            MethodReference patch, string kind)
        {
            var il = target.Body.GetILProcessor();

            var argLoads = BuildArgLoads(il, target, patch);
            var callInstr = il.Create(OpCodes.Call, patch);

            Instruction insertPoint;
            if (kind == "Prefix")
            {
                insertPoint = target.Body.Instructions[0];

                il.InsertBefore(insertPoint, callInstr);
                for (int i = 0; i < argLoads.Count; i++)
                    il.InsertBefore(callInstr, argLoads[i]);
            }
            else
            {
                var lastRet = target.Body.Instructions
                    .LastOrDefault(i => i.OpCode == OpCodes.Ret);
                if (lastRet != null)
                {
                    il.InsertBefore(lastRet, callInstr);
                    for (int i = 0; i < argLoads.Count; i++)
                        il.InsertBefore(callInstr, argLoads[i]);
                }
            }

            if (patch.ReturnType.FullName == "System.Boolean" && kind == "Prefix")
            {
                var firstOriginal = target.Body.Instructions
                    .FirstOrDefault(i => i.OpCode != OpCodes.Nop && i != callInstr
                        && !argLoads.Contains(i));
                if (firstOriginal == null) firstOriginal = target.Body.Instructions[0];

                var continueLabel = il.Create(OpCodes.Nop);
                il.InsertBefore(firstOriginal, continueLabel);

                if (target.ReturnType.FullName != "System.Void")
                {
                    var defaultVal = GetDefaultLoad(il, target.ReturnType);
                    il.InsertAfter(callInstr, il.Create(OpCodes.Brfalse_S, continueLabel));
                    for (int i = defaultVal.Count - 1; i >= 0; i--)
                        il.InsertAfter(callInstr, defaultVal[i]);
                    il.InsertAfter(callInstr, il.Create(OpCodes.Ret));
                }
                else
                {
                    il.InsertAfter(callInstr, il.Create(OpCodes.Brtrue_S, continueLabel));
                    il.InsertAfter(callInstr, il.Create(OpCodes.Ret));
                }
            }
            else if (patch.ReturnType.FullName != "System.Void" && kind == "Prefix")
            {
                il.InsertAfter(callInstr, il.Create(OpCodes.Pop));
            }
        }

        static List<Instruction> GetDefaultLoad(ILProcessor il, TypeReference type)
        {
            var instrs = new List<Instruction>();
            switch (type.FullName)
            {
                case "System.Boolean":
                case "System.Byte":
                case "System.SByte":
                case "System.Int16":
                case "System.UInt16":
                case "System.Int32":
                case "System.UInt32":
                    instrs.Add(il.Create(OpCodes.Ldc_I4_0));
                    break;
                case "System.Int64":
                case "System.UInt64":
                    instrs.Add(il.Create(OpCodes.Ldc_I4_0));
                    instrs.Add(il.Create(OpCodes.Conv_I8));
                    break;
                case "System.Single":
                    instrs.Add(il.Create(OpCodes.Ldc_R4, 0f));
                    break;
                case "System.Double":
                    instrs.Add(il.Create(OpCodes.Ldc_R8, 0.0));
                    break;
                default:
                    instrs.Add(il.Create(OpCodes.Ldnull));
                    break;
            }
            return instrs;
        }

        static List<Instruction> BuildArgLoads(ILProcessor il,
            MethodDefinition target, MethodReference patch)
        {
            var loads = new List<Instruction>();
            bool isInstance = !target.IsStatic;
            int targetStartArg = isInstance ? 1 : 0;

            int patchParamCount = patch.Parameters.Count;

            bool hasInstance = isInstance && patchParamCount > 0
                && patch.Parameters[0].Name == "__instance";
            if (hasInstance)
            {
                loads.Add(il.Create(OpCodes.Ldarg_0));
            }

            int startIdx = hasInstance ? 1 : 0;
            int count = Math.Min(target.Parameters.Count, patchParamCount - startIdx);

            for (int pi = 0; pi < count; pi++)
            {
                int argIdx = targetStartArg + pi;
                int patchPi = startIdx + pi;
                bool isByRef = patch.Parameters[patchPi].ParameterType.IsByReference;

                if (isByRef)
                {
                    loads.Add(il.Create(OpCodes.Ldarga_S, (byte)argIdx));
                }
                else
                {
                    switch (argIdx)
                    {
                        case 0: loads.Add(il.Create(OpCodes.Ldarg_0)); break;
                        case 1: loads.Add(il.Create(OpCodes.Ldarg_1)); break;
                        case 2: loads.Add(il.Create(OpCodes.Ldarg_2)); break;
                        case 3: loads.Add(il.Create(OpCodes.Ldarg_3)); break;
                        default: loads.Add(il.Create(OpCodes.Ldarg_S, (byte)argIdx)); break;
                    }
                }
            }

            return loads;
        }

        static void InjectGameEntryCctor(ModuleDefinition module)
        {
            var gameEntry = module.GetType("GameEntry");
            if (gameEntry == null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[WARN] GameEntry type not found, skipping Bootstrap injection");
                Console.ResetColor();
                return;
            }

            var existing = gameEntry.Methods.FirstOrDefault(m => m.Name == ".cctor");

            if (existing != null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[WARN] GameEntry already has .cctor, prepending Bootstrap init");
                Console.ResetColor();

                var il = existing.Body.GetILProcessor();
                var first = existing.Body.Instructions[0];
                var instrs = BuildBootstrapInitInstrs(module);
                for (int i = instrs.Count - 1; i >= 0; i--)
                    il.InsertBefore(first, instrs[i]);
            }
            else
            {
                var cctor = new MethodDefinition(".cctor",
                    Mono.Cecil.MethodAttributes.Private | Mono.Cecil.MethodAttributes.HideBySig |
                    Mono.Cecil.MethodAttributes.Static | Mono.Cecil.MethodAttributes.SpecialName |
                    Mono.Cecil.MethodAttributes.RTSpecialName,
                    module.TypeSystem.Void);

                var il = cctor.Body.GetILProcessor();
                foreach (var instr in BuildBootstrapInitInstrs(module))
                    il.Append(instr);
                il.Emit(OpCodes.Ret);

                gameEntry.Methods.Add(cctor);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[OK] Injected GameEntry .cctor (Bootstrap init)");
                Console.ResetColor();
            }
        }

        static List<Instruction> BuildBootstrapInitInstrs(ModuleDefinition module)
        {
            var instrs = new List<Instruction>();

            var asmLoad = module.ImportReference(
                typeof(Assembly).GetMethod("Load", new[] { typeof(string) }));
            var asmGetType = module.ImportReference(
                typeof(Assembly).GetMethod("GetType", new[] { typeof(string) }));
            var typeGetField = module.ImportReference(
                typeof(System.Type).GetMethod("GetField", new[] { typeof(string) }));
            var fiGetValue = module.ImportReference(
                typeof(System.Reflection.FieldInfo).GetMethod("GetValue", new[] { typeof(object) }));

            instrs.Add(Instruction.Create(OpCodes.Ldstr, "JinguMod"));
            instrs.Add(Instruction.Create(OpCodes.Call, asmLoad));
            instrs.Add(Instruction.Create(OpCodes.Ldstr, "JinguMod.Bootstrap"));
            instrs.Add(Instruction.Create(OpCodes.Callvirt, asmGetType));
            instrs.Add(Instruction.Create(OpCodes.Ldstr, "Loaded"));
            instrs.Add(Instruction.Create(OpCodes.Callvirt, typeGetField));
            instrs.Add(Instruction.Create(OpCodes.Ldnull));
            instrs.Add(Instruction.Create(OpCodes.Callvirt, fiGetValue));
            instrs.Add(Instruction.Create(OpCodes.Pop));

            return instrs;
        }
    }
}
