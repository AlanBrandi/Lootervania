using UnityEngine;

public class ToolPlacedObject : MonoBehaviour
{
    [SerializeField] private ToolMaterialCost buildCost;
    [SerializeField] [Range(0.25f, 0.5f)] private float refundFactor = 0.5f;

    public ToolMaterialCost BuildCost => buildCost;
    public float RefundFactor => refundFactor;

    public void Setup(ToolMaterialCost cost, float refund)
    {
        buildCost = cost;
        refundFactor = Mathf.Clamp(refund, 0.25f, 0.5f);
    }

    public virtual void RemoveAndRefund(ToolResourceInventory inventory)
    {
        if (inventory != null)
        {
            inventory.Refund(buildCost, refundFactor);
        }

        Destroy(gameObject);
    }
}
