using UnityEngine;

public class ToolResourceInventory : MonoBehaviour
{
    [Header("Starting Resources")]
    [SerializeField] private int wood = 20;
    [SerializeField] private int rope = 12;
    [SerializeField] private int stone = 16;

    public int Wood => wood;
    public int Rope => rope;
    public int Stone => stone;

    public bool TrySpend(ToolMaterialCost cost)
    {
        if (wood < cost.wood || rope < cost.rope || stone < cost.stone)
            return false;

        wood -= cost.wood;
        rope -= cost.rope;
        stone -= cost.stone;
        return true;
    }

    public void Refund(ToolMaterialCost cost, float refundFactor)
    {
        float clamped = Mathf.Clamp01(refundFactor);
        wood += Mathf.RoundToInt(cost.wood * clamped);
        rope += Mathf.RoundToInt(cost.rope * clamped);
        stone += Mathf.RoundToInt(cost.stone * clamped);
    }

    public void AddResource(ToolResourceType type, int amount)
    {
        if (amount <= 0)
            return;

        switch (type)
        {
            case ToolResourceType.Wood:
                wood += amount;
                break;
            case ToolResourceType.Rope:
                rope += amount;
                break;
            case ToolResourceType.Stone:
                stone += amount;
                break;
        }
    }
}
