using UnityEngine;

public class CameraTilt : MonoBehaviour
{
    [SerializeField] private Transform _cameraHolder;
    [SerializeField] private HeadBob _headBob;

    [Header("Tilt Settings")]
    [SerializeField] private float _horizontalTiltAngle = 10f;
    [SerializeField] private float _verticalTiltAngle = 5f;
    [SerializeField] private float _tiltSpeed = 10f; // чем больше, тем быстрее

    private PlayerController _playerController;

    private void Awake()
    {
        _playerController = GetComponentInParent<PlayerController>();
    }

    private void Update()
    {
        if (UIManager.Instance.AreModalWindowsOpened()) return;
        _headBob.DoHeadBob(_playerController.CurrentSpeedPercent);
        HandleCameraTilt();
    }

    private void HandleCameraTilt()
    {
        float horizontalInput = InputManager.Instance.Look.x;
        float verticalInput = InputManager.Instance.Look.y;

        float targetZRotation = -horizontalInput * _horizontalTiltAngle;
        float targetXRotation = verticalInput * _verticalTiltAngle;

        Quaternion targetRotation = Quaternion.Euler(targetXRotation, 0f, targetZRotation);
        
        float t = 1f - Mathf.Exp(-_tiltSpeed * Time.deltaTime);
        _cameraHolder.localRotation = Quaternion.Lerp(
            _cameraHolder.localRotation,
            targetRotation,
            t
        );
    }
}