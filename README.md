# 今古群侠传 Mod 技术分析报告

## 零、快速总结

> **TL;DR：** 经过大量测试，该游戏 Unity 2021.3.15f1 的 Mono 运行时做了高强度托管代码裁剪，导致 BepInEx、MelonLoader、Harmony、MonoMod 等所有主流动态 Mod 框架全部无法使用。最终方案为自建**静态 IL 注入框架（JinguMod）**，利用 `Assembly.Load` + Mono.Cecil 在游戏启动前修改 Assembly-CSharp.dll，已验证可成功修改游戏数值。

直接修改 `src/Patch/PatchEntry.cs` 即可。

---

## 一、游戏基本信息

| 项 | 值 |
|---|---|
| 游戏名 | 今古群侠传 / JinGu |
| 开发商 | 金十四工作室 |
| 平台 | Steam |
| Unity 版本 | 2021.3.15f1c1 |
| 运行时 | Mono (MonoBleedingEdge) 6.12 |
| 架构 | x64 (64-bit) |
| C# 后端 | Mono（非 IL2CPP），存在 Assembly-CSharp.dll |

---

## 二、主流 Mod 框架实验结论

### BepInEx — 全部失败

| 版本 | 代理 DLL | 结果 |
|---|---|---|
| 5.3.0 / 5.4.19 / 5.4.20 / 5.4.21 / 5.4.23 | winhttp.dll | 游戏正常启动，BepInEx 完全不加载，无 LogOutput.log |
| 同上全部版本 | version.dll | 同上，完全不加载 |
| 所有版本 | x86 变体 | 不兼容（游戏为 x64） |

**失败原因：** 该游戏的 UnityPlayer.dll 对 `winhttp.dll` 和 `version.dll` 使用了**延迟加载（delay-load）**，实际运行时从不调用这些 DLL 的函数，导致 Windows 不会加载本地拷贝。Doorstop 代理 DLL 根本没有执行机会。

> ⚠️ 此问题在其他 Unity 2021 游戏中未复现，系该游戏特有的构建配置。

### MelonLoader — 各版本表现不一

| 版本 | 结果 |
|---|---|
| 0.4.3 | 闪退：`MissingMethodException: AmbiguousMatchException..ctor` |
| 0.5.4 | 闪退：`MissingMethodException: ServicePointManager.set_Expect100Continue` |
| 0.5.7 | 闪退：`MissingMethodException: ServicePointManager.get_ServerCertificateValidationCallback` |
| 0.6.5 | 安装器失败：`Internal Failure: Failed to find MelonLoader Bootstrap` |
| 0.6.6 | 手动安装报错找不到 dobby.dll；闪退但生成了 Mods/ 目录 |
| 0.7.3 | 代理注入成功，但 Bootstrap 初始化失败：`CustomAttributeFormatException: Could not find a field with name CharSet` |

**关键发现：** MelonLoader 0.7.x 的 `version.dll` 代理确实能成功注入（与 BepInEx 不同），说明注入通道存在。失败发生在注入后的托管代码初始化阶段，被 Unity Mono 的 API 缺失所阻断。

### 尝试修复 MelonLoader 的过程

针对 MelonLoader 0.7.3，尝试用 Mono.Cecil 修补其 DLL 来绕过 CharSet 报错：

- 清空 `NativeLibrary<T>..ctor`（导致后续 `ldfld` 空引用）→ ❌ 失败
- 在 `BootstrapInterop.Initialize` 的 catch 块中返回 null（跳过错误）→ 游戏能进主菜单，但 `Core.Initialize` 未被调用 → Mods 不加载
- 插入 `br` 指令直接跳到 `Core.Initialize` → 闪退（方法元数据损坏）
- 将所有 `NativeLibrary` 的 `newobj` 替换为 `ldnull` → 空引用异常

针对 MelonLoader 0.5.4 和 0.4.3，同样遇到一连串缺失 API：

- `ServicePointManager.set_Expect100Continue` — 清空 `ServerCertificateValidation.Install` 方法
- `Directory.SetCurrentDirectory` — 清空 `MelonUtils.SetCurrentDomainBaseDirectory`
- `AmbiguousMatchException..ctor(string, Exception)` — 清空 `InvariantCurrentCulture.Install`
- `FileSystemWatcher..ctor(string, string)` — 清空后仍旧失败，链式报错无止境

**结论：** 每一处补丁修完都会暴露下一个缺失的 API，属于**系统性不兼容**，无法通过逐个修补解决。

---

## 三、深层原因：Mono 托管代码裁剪

Unity 在打包游戏时会分析哪些 .NET 类型被实际使用，未引用的都会被自动裁剪以减小包体。该游戏设置了高等级裁剪（**High**），导致以下关键 API 全部缺失：

| 缺失 API | 影响 |
|---|---|
| `System.Threading.ReaderWriterLockSlim` | Harmony / HarmonyX 类型初始化失败 |
| `RuntimeHelpers.PrepareMethod` | MonoMod 动态 Detour 无法执行 JIT 方法准备 |
| `ServicePointManager` 方法 | MelonLoader 0.4.x / 0.5.x 网络层崩溃 |
| `DllImportAttribute.CharSet` 字段 | MelonLoader 0.7.x 原生方法扫描失败 |
| `Directory.SetCurrentDirectory` | MelonLoader 0.5.x 工作目录设置失败 |
| `AmbiguousMatchException..ctor(string,Exception)` | MelonLoader 0.4.x 异常处理失败 |
| `FileSystemWatcher..ctor(string,string)` | MelonLoader 0.4.x 配置文件监视失败 |

> ⚠️ 这些 API 属于 .NET 运行时的内部实现或底层同步原语，无法通过简单的 DLL 文件替换来补齐。`PrepareMethod` 更是 JIT 编译器层面的功能，仅存在于 C++ 编写的 Mono 运行时引擎中。

---

## 四、为何其他方案也不可行？

### 修改 ScriptingAssemblies.json

`ScriptingAssemblies.json` 仅用于 Unity Editor 构建，游戏运行时忽略此文件。在其中添加 DLL 名称不会导致该 DLL 被加载。

### Unity Subsystems

游戏日志虽然打印了 `Discovering subsystems at path .../UnitySubsystems`，但该目录实际不存在，无法利用。

### IL2CPP 转换

游戏是 Mono 后端而非 IL2CPP，不存在 `GameAssembly.dll`。Mono 后端的优势是托管代码以原始 IL 字节码存储，可以用 Mono.Cecil 直接读写。

---

## 五、最终方案：静态 IL 注入框架

### 核心思路

放弃运行时动态 Hook，改为**启动前用 Mono.Cecil 直接修改 Assembly-CSharp.dll 的 IL 字节码**。Mono.Cecil 是纯 .NET 字节码读写库，不依赖任何游戏运行时 API。

### 实现架构

```
Mods/YourPatch.dll          ← 你写的补丁（C# 代码，命名约定）
        │
JinguPatcher.exe             ← IL 注入工具（启动前运行一次）
        │
Assembly-CSharp.dll          ← 游戏代码被直接修改
        │
JinguMod.dll (Bootstrap)     ← 运行时引导器（Unity 通过 GameEntry 自动加载）
        │
游戏正常运行，补丁生效
```

### 工作流程

1. **GameEntry 静态构造函数**（自动注入，无需用户操作）：

```csharp
static GameEntry()
{
   var asm = Assembly.Load("JinguMod");
   var type = asm.GetType("JinguMod.Bootstrap");
   type.GetField("Loaded").GetValue(null);  // 触发 Bootstrap 初始化
}
```

2. **Bootstrap 初始化**：扫描 `Mods/` 目录，加载补丁 DLL
3. **JinguPatcher.exe**：扫描 Mods 补丁，生成 IL 注入到 Assembly-CSharp.dll
4. **补丁编写**：使用命名约定 `Prefix_类名_方法名` 或 `Postfix_类名_方法名`

### 补丁编写示例

```csharp
namespace JinguModPatch;

public static class PatchEntry
{
    // 拦截 ChangeItem，阻止扣钱
    public static void Prefix_SaveData_ChangeItem(int itemId, ref int changeNum)
    {
        if (itemId == 10001 && changeNum < 0)
            changeNum = 0;
    }
}
```

### 核心代码

**Bootstrap 加载器**（JinguMod/Bootstrap.cs）：

```csharp
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
                    var patchAsm = Assembly.LoadFrom(dllPath);
                    var entryType = patchAsm.GetType("JinguModPatch.PatchEntry");
                    if (entryType != null)
                    {
                        var loadedField = entryType.GetField("Loaded");
                        if (loadedField != null)
                            loadedField.GetValue(null); // 触发补丁静态构造函数
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
```

**IL 注入工具**（JinguPatcher/Program.cs）：

1. 使用 Mono.Cecil 读取 Assembly-CSharp.dll
2. 扫描 `Mods/` 中的补丁 DLL
3. 解析 `Prefix_类名_方法名` 命名约定
4. 将补丁方法的 `call` 指令注入目标方法开头

> 完整源码见项目仓库。

---

## 六、优缺点对比

| 特性 | BepInEx / MelonLoader | 本方案（静态 IL 注入） |
|---|---|---|
| 安装方式 | 解压到游戏目录 | 解压到游戏目录 |
| 是否依赖运行时 API | 是（被裁剪后失效） | 否 |
| 补丁语法 | `[HarmonyPatch]` 特性 | `Prefix_类名_方法名` 约定 |
| 入参修改 | 支持 | 支持（`ref` 参数） |
| 返回值修改 | 支持（`__result`） | 实验性支持（ref 返回值参数） |
| 动态热重载 | 支持 | 不支持（需重启 + 重新注入） |
| 兼容性 | 取决于 Mono 裁剪等级 | 任意 Mono 游戏通用 |
| 需要 dnSpy | 否 | 仅查找方法签名时需要 |
| 玩家使用门槛 | 极低（解压即用） | 极低（解压即用） |

---

## 七、对未来的建议

如果有开发者想从根本上解决此问题，可以考虑以下方向：

1. **Unity 重新打包**：在用 Unity 2021.3.15f1 导出项目时，把 Managed Stripping Level 改为 Disabled，重新 Build 后提取完整的 System 类 DLL（mscorlib.dll、System.Core.dll 等），覆盖到原游戏中。这样做可以补齐所有缺失的 .NET API，使 BepInEx/MelonLoader 全部恢复可用。**这是唯一可能让动态框架重回战场的方法。** 但没有原始项目文件难以复现。

2. **Assembly Publicizing**：使用工具将游戏所有程序集的 internal 成员公开化，为 mod 提供最大灵活性。

3. **Mono Runtime 注入**：深入 `mono-2.0-bdwgc.dll` 层面，通过 C++ 直接向 Mono 注册缺失类型——但这是极其复杂且风险很高的方案。

---

*本报告编写于 2026 年 5 月 15 日，基于对游戏版本构建号 080beb55ecda（Unity 2021.3.15f1c1）【v1.0.0】的实际测试。——酸菜鱼真菜*
