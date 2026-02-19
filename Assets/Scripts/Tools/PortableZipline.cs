using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D), typeof(LineRenderer))]
public class PortableZipline : ToolPlacedObject
{
    [SerializeField] private float travelSpeed = 7f;

    private EdgeCollider2D _edgeCollider;
    private LineRenderer _lineRenderer;
    private bool _isInUse;

    public Vector2 StartPoint { get; private set; }
    public Vector2 EndPoint { get; private set; }
    public float Length { get; private set; }
    public float TravelSpeed => travelSpeed;

    private void Awake()
    {
        _edgeCollider = GetComponent<EdgeCollider2D>();
        _lineRenderer = GetComponent<LineRenderer>();
    }

    public void Configure(Vector2 start, Vector2 end, float speed)
    {
        StartPoint = start;
        EndPoint = end;
        travelSpeed = Mathf.Max(2f, speed);
        Length = Vector2.Distance(StartPoint, EndPoint);

        Vector2 localStart = transform.InverseTransformPoint(StartPoint);
        Vector2 localEnd = transform.InverseTransformPoint(EndPoint);

        _edgeCollider.isTrigger = true;
        _edgeCollider.points = new[] { localStart, localEnd };

        _lineRenderer.positionCount = 2;
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.SetPosition(0, StartPoint);
        _lineRenderer.SetPosition(1, EndPoint);
        _lineRenderer.startWidth = 0.08f;
        _lineRenderer.endWidth = 0.08f;
    }

    public bool IsNearStart(Vector2 point, float radius)
    {
        return Vector2.Distance(StartPoint, point) <= radius;
    }

    public bool TryReserve()
    {
        if (_isInUse)
            return false;

        _isInUse = true;
        return true;
    }

    public void Release()
    {
        _isInUse = false;
    }

    public Vector2 GetPointByProgress(float progress)
    {
        return Vector2.Lerp(StartPoint, EndPoint, Mathf.Clamp01(progress));
    }
}
