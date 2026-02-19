using UnityEngine;

public class ToolObstacle : MonoBehaviour
{
    [SerializeField] private HarvestMode requiredMode = HarvestMode.Pickaxe;
    [SerializeField] private int durability = 3;
    [SerializeField] private ToolResourceType rewardType = ToolResourceType.Stone;
    [SerializeField] private int rewardAmount = 3;

    public bool TryHarvest(HarvestMode mode, ToolResourceInventory inventory)
    {
        if (mode != requiredMode)
            return false;

        durability -= 1;
        if (durability > 0)
            return true;

        if (inventory != null)
        {
            inventory.AddResource(rewardType, rewardAmount);
        }

        Destroy(gameObject);
        return true;
    }
}
