using System.Collections;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    [SerializeField] private CinemachineVirtualCamera[] _virtualCameras;

    [Header("Controls for lerping the Y damping during player jump/fall")]
    [SerializeField] private float _fallPanAmount = .25f;
    [SerializeField] private float _fallYPanTime = .35f;
    public float _fallSpeedYDampingChangeThreshold = 15f;

    public bool IsLerpingYDamping { get; private set; }
    public bool LerpedFromPlayerFalling { get; set; }

    private int _lerpYDampingTweenId;
    private int _panCameraTweenId;

    private CinemachineVirtualCamera _currentCamera;
    private CinemachineFramingTransposer _framingTransposer;

    private Coroutine _lerpYPanCoroutine;

    private float _normYPanAmount;
    private Vector2 _startingTrackedObjectOffset;

    private void Awake()
    {
        if (instance == null) instance = this;

        for (int i = 0; i < _virtualCameras.Length; i++)
        {
            if (_virtualCameras[i].enabled)
            {
                _currentCamera = _virtualCameras[i];
                _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            }
        }
 
        _framingTransposer.m_TrackedObjectOffset = new Vector3(1f, 0f, 0f);
        _normYPanAmount = _framingTransposer.m_YDamping;
        _startingTrackedObjectOffset = _framingTransposer.m_TrackedObjectOffset;
    }

    #region Lero the Y Damping

    public void LerpYDamping(bool isPlayerFalling)
    {
        _lerpYPanCoroutine = StartCoroutine(LerpYAction(isPlayerFalling));
    }

    private IEnumerator LerpYAction(bool isPlayerFalling)
    {
        IsLerpingYDamping = true;

        float startDampAmount = _framingTransposer.m_YDamping;
        float endDampAmount = 0f;

        if (isPlayerFalling)
        {
            endDampAmount = _fallPanAmount;
            LerpedFromPlayerFalling = true;
        }
        else
        {
            endDampAmount = _normYPanAmount;
        }

        float elapsedTime = 0f;
        while (elapsedTime < _fallYPanTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, elapsedTime / _fallYPanTime);
            _framingTransposer.m_YDamping = lerpedPanAmount;

            yield return null;
        }
        IsLerpingYDamping = false;
    }
    #endregion

    #region Pan Camera

    public void PanCameraOnContact(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
    {
        Vector2 endPos = Vector2.zero;
        Vector2 startingPos = Vector2.zero;

        if (!panToStartingPos)
        {
            switch (panDirection)
            {
                case PanDirection.Up:
                    endPos = Vector2.up;
                    break;
                case PanDirection.Down:
                    endPos = Vector2.down;
                    break;
                case PanDirection.Right:
                    endPos = Vector2.right;
                    break;
                case PanDirection.Left:
                    endPos = Vector2.left;
                    break;
            }

            endPos *= panDistance;
            startingPos = _startingTrackedObjectOffset;
            endPos += startingPos;
        }
        else
        {
            startingPos = _framingTransposer.m_TrackedObjectOffset;
            endPos = _startingTrackedObjectOffset;
        }

        LeanTween.cancel(_panCameraTweenId);

        _panCameraTweenId = LeanTween.value(gameObject, startingPos, endPos, panTime)
            .setEase(LeanTweenType.easeInOutSine)
            .setOnUpdate((Vector2 value) =>
            {
                _framingTransposer.m_TrackedObjectOffset = value;
            }).id;
    }

    #endregion

    #region Swap Cameras

    public void SwapCamera(CinemachineVirtualCamera cameraFromLeft, CinemachineVirtualCamera cameraFromRight, Vector2 triggerExitDirection)
    {
        if(_currentCamera == cameraFromLeft && triggerExitDirection.x > 0f)
        {
            cameraFromRight.enabled = true;

            cameraFromLeft.enabled = false;

            _currentCamera = cameraFromRight;

            _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
        else if(_currentCamera == cameraFromRight && triggerExitDirection.x > 0f)
        {
            cameraFromLeft.enabled = true;

            cameraFromRight.enabled = false;

            _currentCamera = cameraFromLeft;

            _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
    }

    #endregion
}
