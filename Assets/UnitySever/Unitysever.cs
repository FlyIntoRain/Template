using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;
using WebSocketSharp;
using WebSocketSharp.Server;

public class UnitySever : MonoBehaviour
{
    // WebSocket服务器配置
    private WebSocketServer wssv;
    public int port = 8888;
    private bool isServerRunning = false;
    public GameObject playerPrefab;
    public GameObject spawnPos;

    public Dictionary<string, GameObject> connectedClients = new Dictionary<string, GameObject>();

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
    //向特定客户端发消息的方法
    public void SendMessageToClient(string clientId, string message)
    {
        if (isServerRunning && wssv != null)
        {
            try
            {
                wssv.WebSocketServices["/"].Sessions.SendTo(message, clientId);
            }
            catch (Exception ex)
            {
                Debug.LogError($"向客户端 {clientId} 发送消息失败: {ex.Message}");
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
            Debug.Log($"移动端客户端已连接, Session ID: {ID}");
            MainThreadDispatcher.Instance.Enqueue(() => {
                if (UnitySever.Instance.playerPrefab != null)
                {
                    GameObject newPlayer = Instantiate(UnitySever.Instance.playerPrefab, UnitySever.Instance.spawnPos.GetComponent<Transform>().position, Quaternion.identity);
                    newPlayer.name = $"Player_{ID}"; // 给玩家起个有意义的名字
<<<<<<< Updated upstream

                    // 将新玩家与客户端ID关联起来
                    UnitySever.Instance.connectedClients.Add(ID, newPlayer);
                    Debug.Log($"为ID {ID} 创建了玩家: {newPlayer.name}");
                }
                else
                {
                    Debug.LogError("Player Prefab 未在 UnitySever 中设置！");
                }
            });
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
                if (!UnitySever.Instance.connectedClients.TryGetValue(ID, out GameObject playerObject))
                {
                    Debug.LogWarning($"收到来自未关联玩家的客户端消息, ID: {ID}");
                    return;
                }

                if (message.Contains("\"type\":\"move\""))
                {
                    // 使用Enqueue将任务派发到主线程
                    MainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        // ----- 这部分代码现在会在主线程中安全地执行 -----

                        // 重新获取一次玩家对象，确保在主线程执行时玩家仍然连接
                        if (UnitySever.Instance.connectedClients.TryGetValue(ID, out GameObject playerObject))
                        {
                            // 现在在这里调用GetComponent是绝对安全的！
                            ActorMove moveScript = playerObject.GetComponent<ActorMove>();
                            if (moveScript != null)
                            {
                                // 调用移动方法也是安全的
                                moveScript.ProcessMoveCommand(message);
                            }
                        }
                        // ----- 主线程任务结束 -----
                    });
=======

                    // 将新玩家与客户端ID关联起来
                    UnitySever.Instance.connectedClients.Add(ID, newPlayer);
                    Debug.Log($"为ID {ID} 创建了玩家: {newPlayer.name}");
>>>>>>> Stashed changes
                }
                //else if (message.Contains("\"type\":\"confirm_interaction\""))
                //{
                //    Debug.Log("检测到确认交互命令，准备在主线程处理");

                //    // 关键修改：将交互逻辑放入主线程队列
                //    MainThreadDispatcher.Instance.Enqueue(() =>
                //    {
                //        // 现在在主线程中执行，可以安全调用FindObjectOfType
                //        var npc = FindObjectOfType<NPCInteraction>();
                //        if (npc != null)
                //        {
                //            npc.HandleConfirmation();
                //            Debug.Log("主线程中调用HandleConfirmation成功");
                //        }
                //        else
                //        {
                //            Debug.LogError("主线程中未找到NPCInteraction组件！");
                //        }
                //    });
                //}
                //else if (message.Contains("\"type\":\"dialog_confirm\""))
                //{
                //    MainThreadDispatcher.Instance.Enqueue(() =>
                //    {
                //        var npc = FindObjectOfType<NPCInteraction>();
                //        if (npc != null)
                //        {
                //            npc.HandleDialogResult(true); // 传递确定结果
                //        }
                //    });
                //}
                //else if (message.Contains("\"type\":\"dialog_cancel\""))
                //{
                //    MainThreadDispatcher.Instance.Enqueue(() =>
                //    {
                //        var npc = FindObjectOfType<NPCInteraction>();
                //        if (npc != null)
                //        {
                //            npc.HandleDialogResult(false); // 传递取消结果
                //        }
                //    });
                //}
                else
                {
                    Debug.LogError("Player Prefab 未在 UnitySever 中设置！");
                }
            });
            // 发送欢迎消息给客户端
            Send("{\"type\":\"connected\",\"message\":\"成功连接到Unity服务器\"}");
            

        }

        protected override void OnMessage(MessageEventArgs e)
{
    try
    {
        string message = e.Data;
        // 确保玩家已关联（获取当前客户端ID对应的玩家对象）
        if (!UnitySever.Instance.connectedClients.TryGetValue(ID, out GameObject playerObject))
        {
            Debug.LogWarning($"收到来自未关联玩家的客户端消息, ID: {ID}");
            return;
        }

        if (message.Contains("\"type\":\"move\""))
        {
            // 移动逻辑（保持不变）
            MainThreadDispatcher.Instance.Enqueue(() =>
            {
                if (UnitySever.Instance.connectedClients.TryGetValue(ID, out GameObject player))
                {
                    ActorMove moveScript = player.GetComponent<ActorMove>();
                    if (moveScript != null)
                    {
                        moveScript.ProcessMoveCommand(message);
                    }
                }
            });
        }
        // 新增：处理玩家点击NPC的交互确认（如手机端点击“交互”按钮）
        else if (message.Contains("\"type\":\"confirm_interaction\""))
        {
            Debug.Log($"玩家 {ID} 请求与NPC交互");
            MainThreadDispatcher.Instance.Enqueue(() =>
            {
                var npc = FindObjectOfType<NPCInteraction>();
                if (npc != null)
                {
                    // 关键：传递当前客户端ID，确保NPC处理该玩家的状态
                    npc.HandleConfirmation(ID); 
                }
                else
                {
                    Debug.LogError("未找到NPCInteraction组件！");
                }
            });
        }
        // 新增：处理玩家点击对话框的“确定”按钮（接受任务）
        else if (message.Contains("\"type\":\"dialog_confirm\""))
        {
            Debug.Log($"玩家 {ID} 确认接受任务");
            MainThreadDispatcher.Instance.Enqueue(() =>
            {
                var npc = FindObjectOfType<NPCInteraction>();
                if (npc != null)
                {
                    // 传递客户端ID和确认结果（true）
                    npc.HandleDialogResult(ID, true); 
                }
            });
        }
        // 新增：处理玩家点击对话框的“取消”按钮（拒绝任务）
        else if (message.Contains("\"type\":\"dialog_cancel\""))
        {
            Debug.Log($"玩家 {ID} 取消任务");
            MainThreadDispatcher.Instance.Enqueue(() =>
            {
                var npc = FindObjectOfType<NPCInteraction>();
                if (npc != null)
                {
                    // 传递客户端ID和确认结果（false）
                    npc.HandleDialogResult(ID, false); 
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
            MainThreadDispatcher.Instance.Enqueue(() => {
                // 检查该客户端是否有关联的玩家
                if (UnitySever.Instance.connectedClients.TryGetValue(ID, out GameObject playerObject))
                {
                    // 销毁玩家对象
                    Destroy(playerObject);
                    // 从字典中移除该客户端
                    UnitySever.Instance.connectedClients.Remove(ID);
                    Debug.Log($"ID {ID} 对应的玩家已被销毁");
                }
            });
        }

        protected override void OnError(ErrorEventArgs e)
        {
            Debug.LogError($"WebSocket错误: {e.Message}");
        }
    }
}