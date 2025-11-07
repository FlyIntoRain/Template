using UnityEngine;
[System.Serializable]
public class TreasureFoundMessage
{
    public string type; // 消息类型
    public int count;   // 碎片数量
}
public class TreasureFragment : MonoBehaviour
{
    public float collectDistance = 150f;

    void Update()
    {
        if (ActorMove.Instance == null)
        {
            Debug.LogWarning("ActorMove.Instance 为空，无法检测收集距离");
            return;
        }

        float distance = Vector3.Distance(transform.position, ActorMove.Instance.transform.position);
        if (distance <= collectDistance)
        {
            Debug.Log($"碎片 {gameObject.name} 检测到主角靠近，距离: {distance:F2}，执行收集");
            CollectFragment();
        }
    }

    private void CollectFragment()
    {
        gameObject.SetActive(false);
        Debug.Log($"碎片 {gameObject.name} 已收集（隐藏）");

        int collectedCount = GetCollectedCount();
        Debug.Log($"当前已收集碎片总数: {collectedCount}");

        TreasureFoundMessage messageObj = new TreasureFoundMessage
        {
            type = "treasure_found", // 固定类型
            count = collectedCount   // 实际收集数量
        };
        string json = JsonUtility.ToJson(messageObj);
        Debug.Log($"发送碎片收集消息: {json}");

        if (UnitySever.Instance == null)
        {
            Debug.LogError("UnitySever.Instance 为空，无法发送碎片收集消息");
            return;
        }
        UnitySever.Instance.BroadcastMessage(json);
    }

    private int GetCollectedCount()
    {
        int count = 0;
        var npcInteraction = FindObjectOfType<NPCInteraction>();

        if (npcInteraction == null)
        {
            Debug.LogError("未找到NPCInteraction组件，无法统计碎片数量");
            return 0;
        }

        if (npcInteraction.treasureFragments == null)
        {
            Debug.LogError("NPCInteraction中的treasureFragments数组为空");
            return 0;
        }

        foreach (var fragment in npcInteraction.treasureFragments)
        {
            if (fragment == null)
            {
                Debug.LogWarning("treasureFragments数组中存在空元素");
                continue;
            }
            if (!fragment.activeSelf)
                count++;
        }

        return count;
    }
}