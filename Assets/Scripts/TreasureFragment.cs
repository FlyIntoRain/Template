<<<<<<< Updated upstream
//using UnityEngine;
//[System.Serializable]
//public class TreasureFoundMessage
//{
//    public string type; // 消息类型
//    public int count;   // 碎片数量
//}
//public class TreasureFragment : MonoBehaviour
//{
//    public float collectDistance = 150f;

//    void Update()
//    {
//        if (ActorMove.Instance == null)
//        {
//            //Debug.LogWarning("ActorMove.Instance 为空，无法检测收集距离");
//            return;
//        }

//        float distance = Vector3.Distance(transform.position, ActorMove.Instance.transform.position);
//        if (distance <= collectDistance)
//        {
//            Debug.Log($"碎片 {gameObject.name} 检测到主角靠近，距离: {distance:F2}，执行收集");
//            CollectFragment();
//        }
//    }

//    private void CollectFragment()
//    {
//        gameObject.SetActive(false);
//        Debug.Log($"碎片 {gameObject.name} 已收集（隐藏）");

//        int collectedCount = GetCollectedCount();
//        Debug.Log($"当前已收集碎片总数: {collectedCount}");

//        TreasureFoundMessage messageObj = new TreasureFoundMessage
//        {
//            type = "treasure_found", // 固定类型
//            count = collectedCount   // 实际收集数量
//        };
//        string json = JsonUtility.ToJson(messageObj);
//        Debug.Log($"发送碎片收集消息: {json}");

//        if (UnitySever.Instance == null)
//        {
//            Debug.LogError("UnitySever.Instance 为空，无法发送碎片收集消息");
//            return;
//        }
//        UnitySever.Instance.BroadcastMessage(json);
//    }

//    private int GetCollectedCount()
//    {
//        int count = 0;
//        var npcInteraction = FindObjectOfType<NPCInteraction>();

//        if (npcInteraction == null)
//        {
//            Debug.LogError("未找到NPCInteraction组件，无法统计碎片数量");
//            return 0;
//        }

//        if (npcInteraction.treasureFragments == null)
//        {
//            Debug.LogError("NPCInteraction中的treasureFragments数组为空");
//            return 0;
//        }

//        foreach (var fragment in npcInteraction.treasureFragments)
//        {
//            if (fragment == null)
//            {
//                Debug.LogWarning("treasureFragments数组中存在空元素");
//                continue;
//            }
//            if (!fragment.activeSelf)
//                count++;
//        }

//        return count;
//    }
//}
=======
using UnityEngine;
using System.Collections;

[System.Serializable]
public class TreasureFoundMessage
{
    public string type = "treasure_found"; // 消息类型
    public string clientId; // 收集者ID
    public int fragmentIndex; // 碎片索引
    public int count; // 该玩家当前收集总数
}

public class TreasureFragment : MonoBehaviour
{
    public float collectDistance = 150f; // 收集距离
    public int fragmentIndex; // 碎片索引（0:1号，1:2号，2:3号，需在Inspector手动赋值）
    private bool isRespawning = false; // 是否正在重生冷却

    void Update()
    {
        if (isRespawning || UnitySever.Instance == null) return;

        // 遍历所有玩家，检测是否有玩家进入收集范围
        foreach (var client in UnitySever.Instance.connectedClients)
        {
            string clientId = client.Key;
            GameObject playerObject = client.Value;

            // 计算距离
            float distance = Vector3.Distance(transform.position, playerObject.transform.position);
            if (distance <= collectDistance)
            {
                // 尝试让该玩家收集碎片
                TryCollectByPlayer(clientId);
                break; // 一次只允许一个玩家收集
            }
        }
    }

    // 尝试让指定玩家收集碎片
    private void TryCollectByPlayer(string clientId)
    {
        var npc = FindObjectOfType<NPCInteraction>();
        if (npc == null) return;

        // 检查玩家是否已收集过该碎片
        if (npc.playerTaskStates.TryGetValue(clientId, out var state) 
            && state.collectedFragmentIds.Contains(fragmentIndex))
        {
            // 玩家已收集过，不允许再次收集
            Debug.Log($"玩家 {clientId} 已收集过碎片 {fragmentIndex}，无法重复收集");
            return;
        }

        // 允许收集：隐藏碎片，记录玩家收集记录，3秒后重生
        CollectFragment(clientId);
    }

private void CollectFragment(string clientId)
{
    isRespawning = true;
    Debug.Log($"玩家 {clientId} 收集了碎片 {fragmentIndex}，3秒后重生");

    var npc = FindObjectOfType<NPCInteraction>();
    npc?.OnFragmentCollectedByPlayer(clientId, fragmentIndex);
    SendTreasureFoundMessage(clientId, npc, fragmentIndex);

    Debug.Log($"准备启动重生协程，碎片 {fragmentIndex}");
    
    // ✅ 改为通过服务器对象启动协程
    UnitySever.Instance.StartCoroutine(RespawnAfterDelay(3f));
    
    Debug.Log($"协程已启动，碎片 {fragmentIndex}");
    gameObject.SetActive(false);
}


private IEnumerator RespawnAfterDelay(float delay)
{
    yield return new WaitForSeconds(delay);

    if (this != null && gameObject != null)
    {
        gameObject.SetActive(true);
        isRespawning = false;
        Debug.Log($"碎片 {fragmentIndex} 已重生");
    }
}



    // 向所有客户端发送碎片收集消息（用于更新UI）
    private void SendTreasureFoundMessage(string clientId, NPCInteraction npc, int index)
    {
        if (npc.playerTaskStates.TryGetValue(clientId, out var state))
        {
            TreasureFoundMessage message = new TreasureFoundMessage
            {
                clientId = clientId,
                fragmentIndex = index,
                count= state.collectedFragmentIds.Count
            };
            string json = JsonUtility.ToJson(message);
            UnitySever.Instance?.BroadcastMessage(json); // 广播给所有玩家
        }
    }
}
>>>>>>> Stashed changes
