<<<<<<< Updated upstream
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
=======
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class InteractionMessage
{
    public string type;
    public string title;
    public string content;
}

public class NPCInteraction : MonoBehaviour
{
    public GameObject[] treasureFragments; // 全局碎片数组（0:碎片1，1:碎片2，2:碎片3）
    public float interactionDistance = 300f;
    // 存储每个玩家的任务状态：key=客户端ID，value=该玩家的状态
    internal Dictionary<string, PlayerTaskState> playerTaskStates = new Dictionary<string, PlayerTaskState>();

    // 单个玩家的任务状态
    internal class PlayerTaskState
    {
        public bool isTaskStarted = false; // 是否接受任务
        public bool isTaskCompleted = false; // 是否完成任务
        public bool isWaitingForTaskConfirm = false; // 是否在等待任务确认
        public HashSet<int> collectedFragmentIds = new HashSet<int>(); // 已收集的碎片ID（0,1,2）
    }

    void Update()
    {
        // 遍历所有在线玩家，更新每个玩家的交互状态
        if (UnitySever.Instance == null) return;
        foreach (var client in UnitySever.Instance.connectedClients)
        {
            string clientId = client.Key;
            GameObject playerObject = client.Value;

            // 计算玩家与NPC的距离
            float distance = Vector3.Distance(transform.position, playerObject.transform.position);
            bool inRange = distance <= interactionDistance;

            // 初始化玩家状态（若不存在）
            if (!playerTaskStates.TryGetValue(clientId, out var state))
            {
                state = new PlayerTaskState();
                playerTaskStates[clientId] = state;
            }

            // 判断玩家是否可交互（未开始任务 或 已开始且收集完所有碎片）
            bool canInteract = inRange && ((!state.isTaskStarted) || 
                                           (state.isTaskStarted && state.collectedFragmentIds.Count == treasureFragments.Length));

            // 向该玩家发送交互状态（单播）
            SendNPCStatusToPlayer(clientId, inRange, canInteract);
        }
    }

    // 向单个玩家发送NPC状态
    private void SendNPCStatusToPlayer(string clientId, bool inRange, bool canInteract)
    {
        var message = new
        {
            type = "npc_status",
            inRange = inRange,
            canInteract = canInteract
        };
        UnitySever.Instance?.SendMessageToClient(clientId, JsonUtility.ToJson(message));
    }

    public void HandleConfirmation(string clientId)
{
    if (!playerTaskStates.TryGetValue(clientId, out var state)) return;
    if (!UnitySever.Instance.connectedClients.TryGetValue(clientId, out var playerObject)) return;

    // ✅ 距离检查：必须在交互范围内才能触发
    float distance = Vector3.Distance(transform.position, playerObject.transform.position);
    if (distance > interactionDistance)
    {
        Debug.Log($"玩家 {clientId} 距离NPC太远，无法交互 ({distance:F2}/{interactionDistance})");
        return;
    }

    // 以下是原有逻辑
    if (!state.isTaskStarted)
    {
        SendTaskConfirmDialogToPlayer(clientId);
        state.isWaitingForTaskConfirm = true;
    }
    else if (!state.isTaskCompleted && state.collectedFragmentIds.Count == treasureFragments.Length)
    {
        CompleteTaskForPlayer(clientId);
    }
}


    // 向单个玩家发送任务确认对话框
    private void SendTaskConfirmDialogToPlayer(string clientId)
    {
        InteractionMessage message = new InteractionMessage
        {
            type = "interaction",
            title = "接受任务？",
            content = "是否接受寻找3个文物碎片的任务？每个碎片只能收集一次，3秒后会重生供其他玩家收集。"
        };
        UnitySever.Instance?.SendMessageToClient(clientId, JsonUtility.ToJson(message));
    }

    // 处理玩家的对话框结果（接受/拒绝任务）
    public void HandleDialogResult(string clientId, bool isConfirm)
    {
        if (!playerTaskStates.TryGetValue(clientId, out var state) || !state.isWaitingForTaskConfirm) return;

        if (isConfirm)
        {
            // 接受任务：标记任务开始，显示所有碎片
            state.isTaskStarted = true;
            ShowAllFragments(); // 全局显示所有碎片
            SendMessageToClient(clientId, "任务开始", "请收集所有3个文物碎片（每个只能收集一次）");
        }
        else
        {
            // 拒绝任务
            SendMessageToClient(clientId, "任务取消", "你已拒绝任务，可随时再来找我。");
        }
        state.isWaitingForTaskConfirm = false;
    }

    // 单个玩家完成任务
    private void CompleteTaskForPlayer(string clientId)
    {
        if (!playerTaskStates.TryGetValue(clientId, out var state)) return;

        state.isTaskCompleted = true;
        SendMessageToClient(clientId, "任务结束", "恭喜你收集了所有碎片，完成任务！");
    }

    // 全局显示所有碎片（初始或重生时）
    public void ShowAllFragments()
    {
        foreach (var fragment in treasureFragments)
        {
            if (fragment != null) fragment.SetActive(true);
        }
    }

    // 记录玩家收集的碎片（由碎片脚本调用）
    public void OnFragmentCollectedByPlayer(string clientId, int fragmentIndex)
    {
        if (!playerTaskStates.TryGetValue(clientId, out var state) || state.isTaskCompleted) return;

        // 只记录未收集过的碎片
        if (!state.collectedFragmentIds.Contains(fragmentIndex))
        {
            state.collectedFragmentIds.Add(fragmentIndex);
            Debug.Log($"玩家 {clientId} 收集了碎片 {fragmentIndex}，当前进度: {state.collectedFragmentIds.Count}/3");
        }
    }

    // 向单个玩家发送消息
    private void SendMessageToClient(string clientId, string title, string content)
    {
        InteractionMessage message = new InteractionMessage
        {
            type = "interaction",
            title = title,
            content = content
        };
        UnitySever.Instance?.SendMessageToClient(clientId, JsonUtility.ToJson(message));
    }

}
>>>>>>> Stashed changes
