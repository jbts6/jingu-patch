using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoPatcher;

namespace JinguPatcher
{
    class Program
    {
        const int RetryCount = 5;
        const int RetryDelayMs = 1000;

        static int Main(string[] args)
        {
            string toolDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);
            var config = PatcherConfig.Load(Path.Combine(toolDir, "patcher.json"));

            string gameDir = args.Length > 0 ? args[0] : FindGameDirectory(config.GameExe);
            if (gameDir == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR] Cannot find game directory.");
                Console.WriteLine("        Place JinguPatcher folder inside the game root directory.");
                Console.ResetColor();
                PauseIfDoubleClick();
                return 1;
            }

            string managedDir = Path.Combine(gameDir, config.ManagedSubdir);
            string assemblyPath = Path.Combine(managedDir, config.TargetAssembly);
            string gameModsDir = Path.Combine(gameDir, config.ModsDir);
            string backupPath = assemblyPath + ".bak";

            if (!File.Exists(assemblyPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] {config.TargetAssembly} not found: {assemblyPath}");
                Console.ResetColor();
                PauseIfDoubleClick();
                return 1;
            }

            CheckGameNotRunning(config.GameProcessName);

            Console.WriteLine($"Game directory : {gameDir}");
            Console.WriteLine($"Tool directory : {toolDir}");
            Console.WriteLine();

            DeployFiles(toolDir, gameDir, managedDir, gameModsDir, config);

            var patchDlls = Directory.GetFiles(gameModsDir, "*.dll")
                .Where(f => !Path.GetFileName(f).Equals(config.ModAssembly, StringComparison.OrdinalIgnoreCase))
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
                                    var candidates = parsed.Value.candidates;

                                    TypeDefinition targetType = null;
                                    string targetMethodName = null;

                                    foreach (var (typeName, methodName) in candidates)
                                    {
                                        var t = gameModule.GetType(typeName);
                                        if (t == null)
                                        {
                                            t = gameModule.GetTypes()
                                                .FirstOrDefault(tt => tt.Name == typeName);
                                        }
                                        if (t != null)
                                        {
                                            var m = t.Methods.FirstOrDefault(mm => mm.Name == methodName);
                                            if (m != null)
                                            {
                                                targetType = t;
                                                targetMethodName = methodName;
                                                break;
                                            }
                                        }
                                    }

                                    if (targetType == null)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Yellow;
                                        Console.WriteLine($"  [SKIP] Type not found for: {patchMethod.Name}");
                                        Console.ResetColor();
                                        continue;
                                    }

                                    var targetMethod = targetType.Methods
                                        .FirstOrDefault(m => m.Name == targetMethodName);
                                    if (targetMethod == null)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Yellow;
                                        Console.WriteLine($"  [SKIP] Method not found: {targetType.Name}.{targetMethodName}");
                                        Console.ResetColor();
                                        continue;
                                    }

                                    var importedPatch = gameModule.ImportReference(patchMethod);
                                    InjectPatch(gameModule, targetMethod, importedPatch, kind);

                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine($"  [OK] {kind}_{targetType.Name}_{targetMethodName}");
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

                InjectGameEntryCctor(gameModule, config);

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

        static void DeployFiles(string toolDir, string gameDir, string managedDir, string gameModsDir, PatcherConfig config)
        {
            Console.WriteLine("--- Deploying files ---");

            string toolModDll = Path.Combine(toolDir, config.ModAssembly);
            if (File.Exists(toolModDll))
            {
                string dest = Path.Combine(managedDir, config.ModAssembly);
                Console.WriteLine($"  {config.ModAssembly} -> {dest}");
                RetryFileOp(() => File.Copy(toolModDll, dest, overwrite: true));
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

            string toolConfig = Path.Combine(toolDir, "patcher.json");
            if (File.Exists(toolConfig))
            {
                string destConfig = Path.Combine(gameDir, "patcher.json");
                Console.WriteLine($"  patcher.json -> {destConfig}");
                RetryFileOp(() => File.Copy(toolConfig, destConfig, overwrite: true));
            }

            Console.WriteLine();
        }

        static void CheckGameNotRunning(string processName)
        {
            var procs = Process.GetProcessesByName(processName);
            if (procs.Length > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] {processName}.exe is still running! Please close the game first.");
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

        static string FindGameDirectory(string gameExe)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            while (dir != null)
            {
                if (File.Exists(Path.Combine(dir, gameExe)))
                    return dir;
                dir = Path.GetDirectoryName(dir);
            }
            return null;
        }

        static (string kind, List<(string typeName, string methodName)> candidates)? ParsePatchMethodName(string name)
        {
            int i1 = name.IndexOf('_');
            if (i1 < 0) return null;
            string kind = name.Substring(0, i1);
            if (kind != "Prefix" && kind != "Postfix") return null;

            string rest = name.Substring(i1 + 1);
            string[] parts = rest.Split('_');
            if (parts.Length < 2) return null;

            var candidates = new List<(string typeName, string methodName)>();
            for (int i = parts.Length - 1; i >= 1; i--)
            {
                string typeName = string.Join(".", parts, 0, i);
                string methodName = string.Join("_", parts, i, parts.Length - i);
                candidates.Add((typeName, methodName));
            }

            return (kind, candidates);
        }

        static void InjectPatch(ModuleDefinition module, MethodDefinition target,
            MethodReference patch, string kind)
        {
            var il = target.Body.GetILProcessor();

            var argLoads = BuildArgLoads(il, target, patch);
            var callInstr = il.Create(OpCodes.Call, patch);

            if (kind == "Prefix")
            {
                FixCoroutineIL(target);

                var insertPoint = target.Body.Instructions[0];
                il.InsertBefore(insertPoint, callInstr);
                for (int i = 0; i < argLoads.Count; i++)
                    il.InsertBefore(callInstr, argLoads[i]);
            }
            else
            {
                InjectPostfixAllReturns(il, target, argLoads, callInstr);
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

        static void FixCoroutineIL(MethodDefinition target)
        {
            if (target.ReturnType.FullName == "System.Collections.IEnumerator")
            {
                target.Body.InitLocals = true;
            }
        }

        static void InjectPostfixAllReturns(ILProcessor il, MethodDefinition target,
            List<Instruction> argLoads, Instruction callInstr)
        {
            var rets = target.Body.Instructions
                .Where(i => i.OpCode == OpCodes.Ret)
                .ToList();

            foreach (var ret in rets)
            {
                var loads = new List<Instruction>();
                foreach (var load in argLoads)
                {
                    var newLoad = CloneInstruction(il, load);
                    loads.Add(newLoad);
                }
                var newCall = il.Create(callInstr.OpCode, (MethodReference)callInstr.Operand);

                il.InsertBefore(ret, newCall);
                for (int i = 0; i < loads.Count; i++)
                    il.InsertBefore(newCall, loads[i]);
            }
        }

        static Instruction CloneInstruction(ILProcessor il, Instruction src)
        {
            if (src.Operand == null)
                return il.Create(src.OpCode);
            if (src.Operand is byte b)
                return il.Create(src.OpCode, b);
            if (src.Operand is sbyte sb)
                return il.Create(src.OpCode, sb);
            if (src.Operand is int i32)
                return il.Create(src.OpCode, i32);
            if (src.Operand is float f)
                return il.Create(src.OpCode, f);
            if (src.Operand is double d)
                return il.Create(src.OpCode, d);
            if (src.Operand is string s)
                return il.Create(src.OpCode, s);
            if (src.Operand is Instruction target)
                return il.Create(src.OpCode, target);
            if (src.Operand is Instruction[] targets)
                return il.Create(src.OpCode, targets);
            if (src.Operand is VariableDefinition var)
                return il.Create(src.OpCode, var);
            if (src.Operand is MethodReference mr)
                return il.Create(src.OpCode, mr);
            if (src.Operand is TypeReference tr)
                return il.Create(src.OpCode, tr);
            if (src.Operand is FieldReference fr)
                return il.Create(src.OpCode, fr);
            if (src.Operand is ParameterDefinition pd)
                return il.Create(src.OpCode, pd);
            if (src.Operand is CallSite cs)
                return il.Create(src.OpCode, cs);
            return il.Create(src.OpCode);
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

            bool hasInstance = false;
            if (isInstance && patchParamCount > 0)
            {
                var firstParam = patch.Parameters[0];
                var firstParamType = firstParam.ParameterType;
                if (firstParamType.IsByReference)
                    firstParamType = ((ByReferenceType)firstParamType).ElementType;

                hasInstance = firstParam.Name == "__instance"
                    || TypesMatch(firstParamType, target.DeclaringType);
            }

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

        static bool TypesMatch(TypeReference a, TypeReference b)
        {
            if (a.Name != b.Name) return false;
            var nsA = a.Namespace ?? "";
            var nsB = b.Namespace ?? "";
            return nsA == nsB;
        }

        static void InjectGameEntryCctor(ModuleDefinition module, PatcherConfig config)
        {
            var gameEntry = module.GetType(config.EntryType);
            if (gameEntry == null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[WARN] {config.EntryType} type not found, skipping Bootstrap injection");
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
                var instrs = BuildBootstrapInitInstrs(module, config);
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
                foreach (var instr in BuildBootstrapInitInstrs(module, config))
                    il.Append(instr);
                il.Emit(OpCodes.Ret);

                gameEntry.Methods.Add(cctor);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[OK] Injected GameEntry .cctor (Bootstrap init)");
                Console.ResetColor();
            }
        }

        static List<Instruction> BuildBootstrapInitInstrs(ModuleDefinition module, PatcherConfig config)
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

            instrs.Add(Instruction.Create(OpCodes.Ldstr, config.ModAssemblyName));
            instrs.Add(Instruction.Create(OpCodes.Call, asmLoad));
            instrs.Add(Instruction.Create(OpCodes.Ldstr, config.BootstrapType));
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
