using UnityEngine;

public class Sway : MonoBehaviour
{
    [Header("Основные настройки")]
    [SerializeField] private float _baseIntensity = 1f;
    [SerializeField] private float _runIntensityMultiplier = 1.5f;
    [SerializeField] private float _smoothing = 10f;

    [Header("Ограничения")]
    [SerializeField] private float _maxPositionOffset = 0.09f;
    [SerializeField] private float _maxRotationAngle = 15f;

    [Header("Движение / прыжок")]
    [SerializeField] private float _moveSwayAmount = 0.05f;
    [SerializeField] private float _jumpSwayAmount = 0.08f;
    [SerializeField] private float _moveSwaySpeed = 6f;

    [Header("Внутренние параметры")]
    [SerializeField] private float _rotationMultiplier = 1.0f;
    [SerializeField] private float _velocityDamping = 8f;
    [SerializeField] private float _stopThreshold = 0.1f;
    [SerializeField] private float _movementBlendSpeed = 6f; // насколько быстро разгоняется/затухает тряска

    private Vector3 _originPosition;
    private Quaternion _originRotation;
    private Quaternion _lastCameraRotation;
    private Vector2 _rotationVelocity;
    private Vector2 _smoothedVelocity;
    private Transform _cameraTransform;
    private PlayerController _playerController;

    // плавный коэффициент, чтобы не было удара при старте
    private float _movementBlend;

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
        _playerController = GetComponentInParent<PlayerController>();
    }

    private void Start()
    {
        _originPosition = transform.localPosition;
        _originRotation = transform.localRotation;
        _lastCameraRotation = _cameraTransform.rotation;
    }

    private void Update()
    {
        if (!_playerController.CanMove)
            return;

        if (UIManager.Instance.AreModalWindowsOpened())
            return;

        HandleRotationSway();
        HandlePositionSway();
    }

    private void HandlePositionSway()
    {
        float dt = Time.deltaTime;
        float lerpFactor = 1f - Mathf.Exp(-_smoothing * dt);

        // плавно разгоняем/гасим «силу движения» (0..1)
        float targetBlend = _playerController.IsMoving || _playerController.IsJumping ? 1f : 0f;
        _movementBlend = Mathf.MoveTowards(
            _movementBlend,
            targetBlend,
            _movementBlendSpeed * dt
        );

        // позиционный sway от поворота мыши
        Vector2 input = _smoothedVelocity * 0.003f * _baseIntensity;
        input.x = Mathf.Clamp(input.x, -_maxPositionOffset, _maxPositionOffset);
        input.y = Mathf.Clamp(input.y, -_maxPositionOffset, _maxPositionOffset);

        Vector3 targetPosition = new Vector3(-input.x, -input.y, 0f);

        // sway от движения, умножаем на _movementBlend, чтобы не было резкого старта
        Vector3 movementSway = GetMovementSway() * _movementBlend;

        Vector3 finalTarget = _originPosition + targetPosition + movementSway;

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            finalTarget,
            lerpFactor
        );
    }

    private Vector3 GetMovementSway()
    {
        float speedPercent = _playerController.CurrentSpeedPercent;
        if (speedPercent <= 0.001f && !_playerController.IsJumping)
            return Vector3.zero;

        // немного уменьшаем влияние скорости, чтобы не раздувало амплитуду
        float intensityFactor = Mathf.Lerp(1f, _runIntensityMultiplier, speedPercent);
        float t = Time.time * _moveSwaySpeed;

        Vector3 moveDir = _playerController.LastMoveDirection;
        if (moveDir.sqrMagnitude > 0.0001f)
            moveDir.Normalize();

        float x = Mathf.Sin(t) * _moveSwayAmount * intensityFactor * _baseIntensity;
        float y = Mathf.Cos(t * 0.5f) * _moveSwayAmount * 0.8f * intensityFactor * _baseIntensity;

        float forwardSwing = Mathf.Sin(t * 0.5f) * _moveSwayAmount * 0.4f * intensityFactor * _baseIntensity;

        Vector3 sway = new Vector3(
            x + moveDir.x * forwardSwing,
            y,
            moveDir.z * forwardSwing
        );

        if (_playerController.IsJumping)
        {
            sway.y += Mathf.Sin(Time.time * _moveSwaySpeed) * _jumpSwayAmount * 0.7f * intensityFactor * _baseIntensity;
        }

        sway.x = Mathf.Clamp(sway.x, -_maxPositionOffset, _maxPositionOffset);
        sway.y = Mathf.Clamp(sway.y, -_maxPositionOffset, _maxPositionOffset);
        sway.z = Mathf.Clamp(sway.z, -_maxPositionOffset, _maxPositionOffset);

        return sway;
    }

    private void HandleRotationSway()
    {
        float dt = Time.deltaTime;

        Quaternion currentRotation = _cameraTransform.rotation;
        Quaternion deltaRotation = currentRotation * Quaternion.Inverse(_lastCameraRotation);
        _lastCameraRotation = currentRotation;

        deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
        if (angle > 180f)
            angle -= 360f;

        Vector3 localAxis = Quaternion.Inverse(currentRotation) * axis;
        float yawDelta = localAxis.y * angle;
        float pitchDelta = -localAxis.x * angle;

        if (dt > 0f)
            _rotationVelocity = new Vector2(yawDelta, pitchDelta) / dt * _rotationMultiplier * _baseIntensity;

        float inputMagnitude = _rotationVelocity.magnitude;

        if (inputMagnitude < _stopThreshold && !_playerController.IsMoving && !_playerController.IsJumping)
        {
            _rotationVelocity = Vector2.zero;
            _smoothedVelocity = Vector2.zero;
        }
        else
        {
            float dampFactor = 1f - Mathf.Exp(-_velocityDamping * dt);
            _smoothedVelocity = Vector2.Lerp(_smoothedVelocity, _rotationVelocity, dampFactor);
        }

        float clampedYaw = Mathf.Clamp(_smoothedVelocity.x, -_maxRotationAngle, _maxRotationAngle);
        float clampedPitch = Mathf.Clamp(_smoothedVelocity.y, -_maxRotationAngle, _maxRotationAngle);

        Quaternion targetRotation = Quaternion.Euler(
            clampedPitch,
            clampedYaw,
            clampedYaw
        );

        float rotLerpFactor = 1f - Mathf.Exp(-_smoothing * dt);
        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            _originRotation * targetRotation,
            rotLerpFactor
        );
    }
}
