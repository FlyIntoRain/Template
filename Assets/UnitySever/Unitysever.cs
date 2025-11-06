using UnityEngine;
using System.Collections;
using WebSocketSharp;
using WebSocketSharp.Server;
using System;
using System.Text;

public class UnitySever : MonoBehaviour
{
    // WebSocket服务器配置
    private WebSocketServer wssv;
    public int port = 8888;
    private bool isServerRunning = false;

    // 单例模式
    private static UnitySever _instance;
    public static UnitySever Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    void Start()
    {
        // 启动WebSocket服务器
        StartWebSocketServer();
    }

    void Update()
    {
        // 服务器状态监控
    }

    private void StartWebSocketServer()
    {
        try
        {
            // 创建WebSocket服务器
            wssv = new WebSocketServer(port);

            // 添加WebSocket行为
            wssv.AddWebSocketService<GameWebSocketBehavior>("/");

            // 启动服务器
            wssv.Start();
            isServerRunning = true;

            Debug.Log($"WebSocket服务器已启动，监听端口: {port}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"启动WebSocket服务器失败: {ex.Message}");
            isServerRunning = false;
        }
    }

    private void StopWebSocketServer()
    {
        if (wssv != null && wssv.IsListening)
        {
            wssv.Stop();
            isServerRunning = false;
            Debug.Log("WebSocket服务器已停止");
        }
    }

    public new void BroadcastMessage(string message)
    {
        if (isServerRunning && wssv != null)
        {
            try
            {
                // 向所有连接的客户端广播消息
                wssv.WebSocketServices["/"].Sessions.Broadcast(message);
                //Debug.Log($"广播消息: {message}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"广播消息失败: {ex.Message}");
            }
        }
    }

    private void OnApplicationQuit()
    {
        // 应用退出时停止服务器
        StopWebSocketServer();
    }

    // WebSocket行为类，处理客户端连接和消息
    public class GameWebSocketBehavior : WebSocketBehavior
    {
        protected override void OnOpen()
        {
            Debug.Log("移动端客户端已连接");
            // 发送欢迎消息给客户端
            Send("{\"type\":\"connected\",\"message\":\"成功连接到Unity服务器\"}");
        }

        // 修改OnMessage方法，增加更多类型判断的调试
        // 在Unitysever.cs的OnMessage方法中修改
        protected override void OnMessage(MessageEventArgs e)
        {
            try
            {
                string message = e.Data;
                //Debug.Log($"收到移动端消息: {message}");

                if (message.Contains("\"type\":\"move\""))
                {
                    // 移动命令处理（同理，若有线程问题也需主线程执行）
                    if (ActorMove.Instance != null)
                    {
                        ActorMove.Instance.ProcessMoveCommand(message);
                    }
                }
                else if (message.Contains("\"type\":\"confirm_interaction\""))
                {
                    Debug.Log("检测到确认交互命令，准备在主线程处理");

                    // 关键修改：将交互逻辑放入主线程队列
                    MainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        // 现在在主线程中执行，可以安全调用FindObjectOfType
                        var npc = FindObjectOfType<NPCInteraction>();
                        if (npc != null)
                        {
                            npc.HandleConfirmation();
                            Debug.Log("主线程中调用HandleConfirmation成功");
                        }
                        else
                        {
                            Debug.LogError("主线程中未找到NPCInteraction组件！");
                        }
                    });
                }
                else if (message.Contains("\"type\":\"dialog_confirm\""))
                {
                    MainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        var npc = FindObjectOfType<NPCInteraction>();
                        if (npc != null)
                        {
                            npc.HandleDialogResult(true); // 传递确定结果
                        }
                    });
                }
                else if (message.Contains("\"type\":\"dialog_cancel\""))
                {
                    MainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        var npc = FindObjectOfType<NPCInteraction>();
                        if (npc != null)
                        {
                            npc.HandleDialogResult(false); // 传递取消结果
                        }
                    });
                }
                else
                {
                    Debug.Log($"未处理的消息类型: {message}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"处理消息失败: {ex.Message}\n堆栈跟踪: {ex.StackTrace}");
            }
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Debug.Log("移动端已断开连接");
            // 当客户端断开连接时，通知ActorMove停止角色移动
            if (ActorMove.Instance != null)
            {
                ActorMove.Instance.StopMovement();
            }
        }

        protected override void OnError(ErrorEventArgs e)
        {
            Debug.LogError($"WebSocket错误: {e.Message}");
        }
    }
}