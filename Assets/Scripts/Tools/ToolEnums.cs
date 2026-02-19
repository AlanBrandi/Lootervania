public enum ToolType
{
    PortableLadder = 0,
    Zipline = 1,
    PortableBridge = 2,
    MovableCrate = 3,
    HarvestTool = 4
}

public enum ToolResourceType
{
    Wood = 0,
    Rope = 1,
    Stone = 2
}

public enum HarvestMode
{
    Pickaxe = 0,
    Axe = 1
}

[System.Serializable]
public struct ToolMaterialCost
{
    public int wood;
    public int rope;
    public int stone;
}
