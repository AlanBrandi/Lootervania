using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerToolController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ToolResourceInventory inventory;
    [SerializeField] private PlayerToolTraversal traversal;

    [Header("General Rules")]
    [SerializeField] private LayerMask solidLayer;
    [SerializeField] private float placeOffset = 1.25f;
    [SerializeField] private float removeRange = 2f;
    [SerializeField] private float interactionRange = 1.2f;

    [Header("Ladder")]
    [SerializeField] private float ladderHeight = 3.5f;
    [SerializeField] private int ladderMaxPlaced = 4;
    [SerializeField] private ToolMaterialCost ladderCost = new ToolMaterialCost { wood = 4, rope = 2, stone = 0 };

    [Header("Zipline")]
    [SerializeField] private float ziplineHorizontalSpan = 5f;
    [SerializeField] private float ziplineMaxLength = 6f;
    [SerializeField] private float ziplineDrop = 2f;
    [SerializeField] private int ziplineMaxPlaced = 3;
    [SerializeField] private float ziplineSpeed = 7f;
    [SerializeField] private ToolMaterialCost ziplineCost = new ToolMaterialCost { wood = 1, rope = 5, stone = 0 };

    [Header("Bridge")]
    [SerializeField] private float bridgeLength = 3f;
    [SerializeField] private int bridgeMaxPlaced = 4;
    [SerializeField] private ToolMaterialCost bridgeCost = new ToolMaterialCost { wood = 5, rope = 1, stone = 1 };

    [Header("Crate")]
    [SerializeField] private int crateMaxPlaced = 4;
    [SerializeField] private float crateSupportedMass = 120f;
    [SerializeField] private float cratePushSpeedLimit = 3f;
    [SerializeField] private ToolMaterialCost crateCost = new ToolMaterialCost { wood = 3, rope = 0, stone = 2 };

    [Header("Harvest")]
    [SerializeField] private float harvestRange = 1.6f;
    [SerializeField] private LayerMask obstacleLayer;

    private PlayerMovement _movement;
    private readonly List<ToolPlacedObject> _placedObjects = new List<ToolPlacedObject>();
    private readonly List<PortableZipline> _ziplines = new List<PortableZipline>();
    private readonly Dictionary<Vector2Int, PortableLadder> _laddersBySurface = new Dictionary<Vector2Int, PortableLadder>();

    private ToolType _selectedTool = ToolType.PortableLadder;
    private HarvestMode _currentHarvestMode = HarvestMode.Pickaxe;

    private void Awake()
    {
        _movement = GetComponent<PlayerMovement>();

        if (inventory == null)
            inventory = GetComponent<ToolResourceInventory>();

        if (traversal == null)
            traversal = GetComponent<PlayerToolTraversal>();
    }

    private void Update()
    {
        HandleSelectionInput();
        HandleActionInput();
    }

    private void HandleSelectionInput()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            int next = (int)_selectedTool - 1;
            if (next < 0) next = System.Enum.GetValues(typeof(ToolType)).Length - 1;
            _selectedTool = (ToolType)next;
        }

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            int next = ((int)_selectedTool + 1) % System.Enum.GetValues(typeof(ToolType)).Length;
            _selectedTool = (ToolType)next;
        }
    }

    private void HandleActionInput()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (!TryAttachNearestZipline())
            {
                if (_selectedTool == ToolType.HarvestTool)
                    TryHarvest();
                else
                    TryPlaceSelectedTool();
            }
        }

        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            TryRemoveNearestTool();
        }

        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            _currentHarvestMode = _currentHarvestMode == HarvestMode.Pickaxe ? HarvestMode.Axe : HarvestMode.Pickaxe;
        }
    }

    private bool TryAttachNearestZipline()
    {
        if (traversal == null || _ziplines.Count == 0)
            return false;

        Vector2 origin = transform.position;
        float bestDistance = float.MaxValue;
        PortableZipline best = null;

        for (int i = _ziplines.Count - 1; i >= 0; i--)
        {
            if (_ziplines[i] == null)
            {
                _ziplines.RemoveAt(i);
                continue;
            }

            float distance = Vector2.Distance(origin, _ziplines[i].StartPoint);
            if (distance <= interactionRange && distance < bestDistance)
            {
                best = _ziplines[i];
                bestDistance = distance;
            }
        }

        return best != null && traversal.TryAttachZipline(best);
    }

    private void TryPlaceSelectedTool()
    {
        switch (_selectedTool)
        {
            case ToolType.PortableLadder:
                TryPlaceLadder();
                break;
            case ToolType.Zipline:
                TryPlaceZipline();
                break;
            case ToolType.PortableBridge:
                TryPlaceBridge();
                break;
            case ToolType.MovableCrate:
                TryPlaceCrate();
                break;
        }
    }

    private void TryPlaceLadder()
    {
        if (CountPlaced<PortableLadder>() >= ladderMaxPlaced || inventory == null)
            return;

        Vector2 basePoint = GetFacingPoint();
        Vector2 supportPoint = basePoint + Vector2.down * 0.6f;
        if (!HasSolid(supportPoint))
            return;

        Vector2Int key = ToSurfaceKey(basePoint);
        if (_laddersBySurface.ContainsKey(key))
            return;

        Collider2D blocked = Physics2D.OverlapBox(basePoint + Vector2.up * (ladderHeight * 0.5f), new Vector2(0.8f, ladderHeight), 0f, solidLayer);
        if (blocked != null)
            return;

        if (!inventory.TrySpend(ladderCost))
            return;

        GameObject go = new GameObject("PortableLadder");
        go.transform.position = basePoint + Vector2.up * (ladderHeight * 0.5f);
        PortableLadder ladder = go.AddComponent<PortableLadder>();
        ladder.Setup(ladderCost, 0.5f);
        ladder.Configure(ladderHeight, key);

        _laddersBySurface[key] = ladder;
        _placedObjects.Add(ladder);
    }

    private void TryPlaceZipline()
    {
        if (CountPlaced<PortableZipline>() >= ziplineMaxPlaced || inventory == null)
            return;

        Vector2 start = transform.position + Vector3.up * 2.1f;
        if (!HasSolid(start + Vector2.up * 0.35f))
            return;

        float dir = _movement != null && _movement.IsFacingRight ? 1f : -1f;
        Vector2 end = start + new Vector2(ziplineHorizontalSpan * dir, -Mathf.Abs(ziplineDrop));
        RaycastHit2D endGround = Physics2D.Raycast(end + Vector2.up, Vector2.down, 2.5f, solidLayer);
        if (!endGround.collider)
            return;

        if (Vector2.Distance(start, end) > ziplineMaxLength + 0.01f)
            return;

        if (!inventory.TrySpend(ziplineCost))
            return;

        GameObject go = new GameObject("PortableZipline");
        PortableZipline zipline = go.AddComponent<PortableZipline>();
        zipline.Setup(ziplineCost, 0.35f);
        zipline.Configure(start, end, ziplineSpeed);

        _ziplines.Add(zipline);
        _placedObjects.Add(zipline);
    }

    private void TryPlaceBridge()
    {
        if (CountPlaced<PortableBridge>() >= bridgeMaxPlaced || inventory == null)
            return;

        float dir = _movement != null && _movement.IsFacingRight ? 1f : -1f;
        Vector2 start = transform.position + new Vector3(dir, -0.15f, 0f);
        Vector2 end = start + new Vector2(bridgeLength * dir, 0f);

        if (!HasSolid(start + Vector2.down * 0.6f))
            return;
        if (!HasSolid(end + Vector2.down * 0.6f))
            return;

        if (!inventory.TrySpend(bridgeCost))
            return;

        GameObject go = new GameObject("PortableBridge");
        go.transform.position = (start + end) * 0.5f;
        PortableBridge bridge = go.AddComponent<PortableBridge>();
        bridge.Setup(bridgeCost, 0.5f);
        bridge.Configure(bridgeLength);

        _placedObjects.Add(bridge);
    }

    private void TryPlaceCrate()
    {
        if (CountPlaced<PortableCrate>() >= crateMaxPlaced || inventory == null)
            return;

        Vector2 spawnPoint = GetFacingPoint();
        RaycastHit2D ground = Physics2D.Raycast(spawnPoint + Vector2.up * 0.8f, Vector2.down, 2f, solidLayer);
        if (!ground.collider)
            return;

        if (!inventory.TrySpend(crateCost))
            return;

        GameObject go = new GameObject("PortableCrate");
        go.transform.position = ground.point + Vector2.up * 0.5f;
        PortableCrate crate = go.AddComponent<PortableCrate>();
        crate.Setup(crateCost, 0.3f);
        crate.Configure(crateSupportedMass, cratePushSpeedLimit);

        _placedObjects.Add(crate);
    }

    private void TryRemoveNearestTool()
    {
        Vector2 origin = transform.position;
        float bestDistance = float.MaxValue;
        ToolPlacedObject best = null;

        for (int i = _placedObjects.Count - 1; i >= 0; i--)
        {
            ToolPlacedObject placed = _placedObjects[i];
            if (placed == null)
            {
                _placedObjects.RemoveAt(i);
                continue;
            }

            float dist = Vector2.Distance(origin, placed.transform.position);
            if (dist <= removeRange && dist < bestDistance)
            {
                best = placed;
                bestDistance = dist;
            }
        }

        if (best == null)
            return;

        PortableLadder ladder = best as PortableLadder;
        if (ladder != null)
        {
            _laddersBySurface.Remove(ladder.SurfaceKey);
        }

        PortableZipline zipline = best as PortableZipline;
        if (zipline != null)
        {
            _ziplines.Remove(zipline);
        }

        _placedObjects.Remove(best);
        best.RemoveAndRefund(inventory);
    }

    private void TryHarvest()
    {
        Vector2 direction = _movement != null && _movement.IsFacingRight ? Vector2.right : Vector2.left;
        Vector2 origin = (Vector2)transform.position + Vector2.up * 0.4f;
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, harvestRange, obstacleLayer);
        if (!hit.collider)
            return;

        ToolObstacle obstacle = hit.collider.GetComponent<ToolObstacle>();
        if (obstacle == null)
            return;

        obstacle.TryHarvest(_currentHarvestMode, inventory);
    }

    private bool HasSolid(Vector2 point)
    {
        return Physics2D.OverlapCircle(point, 0.2f, solidLayer) != null;
    }

    private Vector2 GetFacingPoint()
    {
        float direction = _movement != null && _movement.IsFacingRight ? 1f : -1f;
        return (Vector2)transform.position + new Vector2(direction * placeOffset, 0f);
    }

    private static Vector2Int ToSurfaceKey(Vector2 surfacePoint)
    {
        return new Vector2Int(Mathf.RoundToInt(surfacePoint.x * 4f), Mathf.RoundToInt(surfacePoint.y * 4f));
    }

    private int CountPlaced<T>() where T : ToolPlacedObject
    {
        int count = 0;
        for (int i = 0; i < _placedObjects.Count; i++)
        {
            if (_placedObjects[i] is T)
                count++;
        }

        return count;
    }
}
