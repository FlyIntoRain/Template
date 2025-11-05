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
                Debug.Log($"广播消息: {message}");
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
        
        protected override void OnMessage(MessageEventArgs e)
        {
            try
            {
                string message = e.Data;
                Debug.Log($"收到移动端消息: {message}");
                
                // 转发移动命令给ActorMove处理
                if (message.Contains("\"type\":\"move\""))
                {
                    // 通知ActorMove处理移动命令
                    if (ActorMove.Instance != null)
                    {
                        ActorMove.Instance.ProcessMoveCommand(message);
                    }
                }
                else
                {
                    // 处理其他类型的消息
                    Debug.Log($"未处理的消息类型: {message}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"处理消息失败: {ex.Message}");
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