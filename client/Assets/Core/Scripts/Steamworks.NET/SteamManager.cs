// SteamManager 专为与 Steamworks.NET 协作而设计
// 本文件已释放到公共领域
// 在未被承认此奉献的地方，您被授予永久、
// 不可撤销的许可，可以随意复制和修改此文件
//
// 版本：1.0.13

#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
#define DISABLESTEAMWORKS
#endif

using UnityEngine;
#if !DISABLESTEAMWORKS
using System.Collections;
using Steamworks;
#endif

//
// SteamManager 提供了 Steamworks.NET 的基础实现，您可在此基础上进行构建
// 它处理了启动和关闭 SteamAPI 供使用的基本流程
//
[DisallowMultipleComponent]
public class SteamManager : MonoBehaviour
{
#if !DISABLESTEAMWORKS
    protected static bool s_EverInitialized = false;

    protected static SteamManager s_instance;

    protected static SteamManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                return new GameObject("SteamManager").AddComponent<SteamManager>();
            }
            else
            {
                return s_instance;
            }
        }
    }

    protected bool m_bInitialized = false;

    public static bool Initialized
    {
        get { return Instance.m_bInitialized; }
    }

    protected SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;

    [AOT.MonoPInvokeCallback(typeof(SteamAPIWarningMessageHook_t))]
    protected static void SteamAPIDebugTextHook(int nSeverity, System.Text.StringBuilder pchDebugText)
    {
        Debug.LogWarning(pchDebugText);
    }

#if UNITY_2019_3_OR_NEWER
    // 在禁用域重新加载的情况下，在进入播放模式前重置静态成员
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void InitOnPlayMode()
    {
        s_EverInitialized = false;
        s_instance = null;
    }
#endif

    protected virtual void Awake()
    {
        // 同一时间只能有一个 SteamManager 实例！
        if (s_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        s_instance = this;

        if (s_EverInitialized)
        {
            // 这几乎总是一个错误
            // 最常见的情况是：SteamManager 因 Application.Quit() 被销毁，
            // 然后其他某些 OnDestroy 中的某些 Steamworks 代码在此之后被调用，创建了一个新的 SteamManager
            // 您不应在 OnDestroy 中调用 Steamworks 函数，应尽可能优先使用 OnDisable
            throw new System.Exception("尝试在同一会话中两次初始化 SteamAPI！");
        }

        // 我们希望 SteamManager 实例在场景之间持久存在
        DontDestroyOnLoad(gameObject);

        if (!Packsize.Test())
        {
            Debug.LogError("[Steamworks.NET] Packsize 测试返回 false，在此平台上运行了错误版本的 Steamworks.NET。", this);
        }

        if (!DllCheck.Test())
        {
            Debug.LogError("[Steamworks.NET] DllCheck 测试返回 false，一个或多个 Steamworks 二进制文件版本似乎有误。", this);
        }

        try
        {
            // 如果 Steam 未运行或游戏不是通过 Steam 启动的，SteamAPI_RestartAppIfNecessary 会启动
            // Steam 客户端，并且如果用户拥有此游戏，也会再次启动本游戏。这可以作为一种基础的 DRM 形式。
            // 请注意，这将运行您在 Steam 中安装的任何版本。这可能不是我们当前正在运行的精确可执行文件。

            // 一旦您获得 Valve 分配的 Steam AppID，您需要将 AppId_t.Invalid 替换为它，并
            // 从游戏部署中移除 steam_appid.txt。例如："(AppId_t)480" 或 "new AppId_t(480)"。
            // 更多信息请参阅 Valve 文档：https://partner.steamgames.com/doc/sdk/api#initialization_and_shutdown
            if (SteamAPI.RestartAppIfNecessary(AppId_t.Invalid))
            {
                Debug.Log("[Steamworks.NET] 正在关闭，因为 RestartAppIfNecessary 返回 true。Steam 将重启应用程序。");

                Application.Quit();
                return;
            }
        }
        catch (System.DllNotFoundException e)
        {
            // 我们在此捕获此异常，因为这将是它的首次出现。
            Debug.LogError("[Steamworks.NET] 无法加载 [lib]steam_api.dll/so/dylib。它可能不在正确的位置。更多详情请参考 README。\n" + e, this);

            Application.Quit();
            return;
        }

        // 初始化 Steamworks API。
        // 如果返回 false，则表示以下条件之一：
        // [*] Steam 客户端未运行。需要运行 Steam 客户端来提供各种 Steamworks 接口的实现。
        // [*] Steam 客户端无法确定游戏的 App ID。如果您直接从可执行文件或调试器运行应用程序，那么您必须在游戏目录中、紧邻可执行文件的位置放置一个 [code-inline]steam_appid.txt[/code-inline] 文件，其中包含您的应用 ID 且没有其他内容。Steam 将在当前工作目录中查找此文件。如果您从其他目录运行可执行文件，则可能需要重新定位 [code-inline]steam_appid.txt[/code-inline] 文件。
        // [*] 您的应用程序与 Steam 客户端运行在不同的操作系统用户上下文下，例如不同的用户或管理访问级别。
        // [*] 请确保您在当前活跃的 Steam 帐户上拥有该 App ID 的许可。您的游戏必须显示在您的 Steam 库中。
        // [*] 您的 App ID 未完全设置，例如处于"发布状态：不可用"，或者缺少默认包。
        // Valve 的相关文档位于：   
        // https://partner.steamgames.com/doc/sdk/api#initialization_and_shutdown
        m_bInitialized = SteamAPI.Init();
        if (!m_bInitialized)
        {
            Debug.LogError("[Steamworks.NET] SteamAPI_Init() 失败。更多信息请参考 Valve 的文档或此行上方的注释。", this);

            return;
        }

        s_EverInitialized = true;
    }

    // 这应该只在首次加载和程序集重新加载后调用，您不应自行禁用 Steamworks Manager。
    protected virtual void OnEnable()
    {
        if (s_instance == null)
        {
            s_instance = this;
        }

        if (!m_bInitialized)
        {
            return;
        }

        if (m_SteamAPIWarningMessageHook == null)
        {
            // 设置我们的回调以接收来自 Steam 的警告消息。
            // 您必须在启动参数中加入 "-debug_steamapi" 才能接收警告。
            m_SteamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamAPIDebugTextHook);
            SteamClient.SetWarningMessageHook(m_SteamAPIWarningMessageHook);
        }
    }

    // OnApplicationQuit 被调用得太早，无法关闭 SteamAPI。
    // 因为 SteamManager 应该是持久存在的且从不被禁用或销毁，我们可以在此处关闭 SteamAPI。
    // 因此，不建议在其他 OnDestroy 函数中执行任何 Steamworks 工作，因为在关闭时无法保证执行顺序。请优先使用 OnDisable()。
    protected virtual void OnDestroy()
    {
        if (s_instance != this)
        {
            return;
        }

        s_instance = null;

        if (!m_bInitialized)
        {
            return;
        }

        SteamAPI.Shutdown();
    }

    protected virtual void Update()
    {
        if (!m_bInitialized)
        {
            return;
        }

        // 运行 Steam 客户端回调
        SteamAPI.RunCallbacks();
    }
#else
    public static bool Initialized {
        get {
            return false;
        }
    }
#endif // !DISABLESTEAMWORKS
}