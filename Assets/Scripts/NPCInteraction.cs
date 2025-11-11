//using UnityEngine;
//[System.Serializable] // 必须添加此标记
//public class InteractionMessage
//{
//    public string type;
//    public string title;
//    public string content;
//}

//public class NPCInteraction : MonoBehaviour
//{
//    public GameObject[] treasureFragments;
//    public float interactionDistance = 300f;
//    private bool isTaskStarted = false;
//    private bool isTaskCompleted = false;
//    private bool isWaitingForTaskConfirm = false;

//    void Update()
//    {
//        if (ActorMove.Instance == null)
//        {
//            //Debug.LogWarning("主角Move.Instance 为空，无法检测距离");
//            return;
//        }

//        float distance = Vector3.Distance(transform.position, ActorMove.Instance.transform.position);
//        bool inRange = distance <= interactionDistance;

//        // 增加距离调试信息
//        //Debug.Log($"与主角距离: {distance:F2}，是否在交互范围内: {inRange}");

//        SendNPCStatus(inRange);
//    }

//    public void HandleConfirmation()
//    {
//        Debug.Log("收到客户端确认交互请求");

//        if (!isTaskStarted)
//        {
//            Debug.Log("任务未开始，发送任务确认对话框");
//            SendTaskConfirmDialog();
//            isWaitingForTaskConfirm = true;
//        }
//        else if (CheckAllFragmentsCollected() && !isTaskCompleted)
//        {
//            Debug.Log("所有碎片已收集且任务未完成，执行CompleteTask()");
//            CompleteTask();
//        }
//        else
//        {
//            Debug.Log($"无法交互 - 任务状态: 已开始={isTaskStarted}, 已完成={isTaskCompleted}, 碎片是否收集完成={CheckAllFragmentsCollected()}");
//        }
//    }

//    private void SendTaskConfirmDialog()
//    {
//        SendMessageToClient(
//            "接受任务？",
//            "是否接受寻找3个文物碎片的任务？\n点击确定开始，取消则放弃。"
//        );
//    }

//    public void HandleDialogResult(bool isConfirm)
//    {
//        if (!isWaitingForTaskConfirm) return; // 仅处理等待中的任务确认

//        if (isConfirm)
//        {
//            // 用户点击确定：开始任务，显示碎片
//            Debug.Log("用户确认接受任务，开始任务并显示碎片");
//            StartTask();
//        }
//        else
//        {
//            // 用户点击取消：拒绝任务，不显示碎片
//            Debug.Log("用户取消任务，不显示碎片");
//            SendMessageToClient("任务取消", "你已拒绝任务，可随时再来找我。");
//        }

//        isWaitingForTaskConfirm = false; // 重置等待状态
//    }

//    private void StartTask()
//    {
//        isTaskStarted = true;
//        Debug.Log("任务开始，显示所有文物碎片");

//        foreach (var fragment in treasureFragments)
//        {
//            if (fragment == null)
//            {
//                Debug.LogError("treasureFragments数组中存在空对象");
//                continue;
//            }
//            fragment.SetActive(true);
//            Debug.Log($"显示碎片: {fragment.name}");
//        }

//        SendMessageToClient("任务开始", "请收集所有3个文物碎片");
//    }

//    private void CompleteTask()
//    {
//        isTaskCompleted = true;
//        Debug.Log("任务完成");
//        SendMessageToClient("任务结束", "恭喜你完成了所有寻宝任务！");
//    }

//    private bool CheckAllFragmentsCollected()
//    {
//        int collectedCount = 0;
//        int totalCount = treasureFragments.Length;

//        foreach (var fragment in treasureFragments)
//        {
//            if (fragment == null) continue;
//            if (!fragment.activeSelf) collectedCount++;
//        }

//        Debug.Log($"碎片收集检查: 已收集 {collectedCount}/{totalCount}");
//        return collectedCount == totalCount;
//    }

//    private void SendNPCStatus(bool inRange)
//    {
//        bool canInteract = inRange && ((!isTaskStarted) || (isTaskStarted && CheckAllFragmentsCollected()));
//        //Debug.Log($"发送NPC状态 - 范围内: {inRange}, 可交互: {canInteract}");

//        var message = new
//        {
//            type = "npc_status",
//            inRange = inRange,
//            canInteract = canInteract
//        };

//        if (UnitySever.Instance == null)
//        {
//            Debug.LogError("UnitySever.Instance 为空，无法发送消息");
//            return;
//        }
//        UnitySever.Instance.BroadcastMessage(JsonUtility.ToJson(message));
//    }

//    private void SendMessageToClient(string title, string content)
//    {
//        Debug.Log($"向客户端发送消息 - 标题: {title}, 内容: {content}");

//        // 使用可序列化类创建消息（而非匿名对象）
//        InteractionMessage message = new InteractionMessage
//        {
//            type = "interaction",
//            title = title,
//            content = content
//        };

//        // 序列化消息（此时会生成正确的JSON）
//        string json = JsonUtility.ToJson(message);
//        Debug.Log($"发送的JSON: {json}"); // 新增日志，确认JSON格式

//        if (UnitySever.Instance == null)
//        {
//            Debug.LogError("UnitySever.Instance 为空，无法发送消息");
//            return;
//        }
//        UnitySever.Instance.BroadcastMessage(json);
//    }

//    // 在NPCInteraction.cs中添加
//    private void OnDrawGizmosSelected()
//    {
//        // 绘制NPC周围的交互范围（黄色圆圈）
//        Gizmos.color = Color.yellow;
//        Gizmos.DrawWireSphere(transform.position, interactionDistance);

//        // 若主角存在，绘制主角到NPC的连线（红色）
//        if (ActorMove.Instance != null)
//        {
//            Gizmos.color = Color.red;
//            Gizmos.DrawLine(transform.position, ActorMove.Instance.transform.position);
//        }
//    }
//}
