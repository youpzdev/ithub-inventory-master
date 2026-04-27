using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _walkSpeed = 3f;
    [SerializeField] private float _runSpeed = 6f;
    [SerializeField] private float _jumpHeight = 1.2f;
    [SerializeField] private float _gravity = -9.81f;

    [Header("Movement Smoothing")]
    [SerializeField] private float _accelerationTime = 0.1f;
    [SerializeField] private float _decelerationTime = 0.15f;

    [Header("Look Settings")]
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float _lookSpeed = 2f;
    [SerializeField] private float _maxLookAngle = 80f;

    private CharacterController _controller;

    private Vector3 _velocity;
    private Vector3 _horizontalVelocity;
    private Vector3 _horizontalVelocitySmoothDamp;

    private float _xRotation;
    private bool _isGrounded;

    public bool IsRunning { get; private set; }
    public bool IsJumping => !_isGrounded;
    public bool IsMoving { get; private set; }
    public float CurrentSpeedPercent { get; private set; }
    public Vector3 LastMoveDirection { get; private set; }

    [Space(10)]
    public bool CanMove = true;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (!CanMove)
            return;

        if (UIManager.Instance.AreModalWindowsOpened())
            return;

        HandleLook();
        HandleMovement();
    }

    private void HandleLook()
    {
        Vector2 lookInput = InputManager.Instance.Look;

        float mouseX = lookInput.x * _lookSpeed;
        float mouseY = lookInput.y * _lookSpeed;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -_maxLookAngle, _maxLookAngle);

        _cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleMovement()
    {
        _isGrounded = _controller.isGrounded;

        if (_isGrounded && _velocity.y < 0f)
            _velocity.y = -2f;

        Vector2 moveInput = InputManager.Instance.Move;
        bool jumpInput = InputManager.Instance.Jump;
        bool runInput = InputManager.Instance.Run;

        Vector3 inputDirection =
            transform.right * moveInput.x +
            transform.forward * moveInput.y;

        if (inputDirection.sqrMagnitude > 1f)
            inputDirection.Normalize();

        // запоминаем последнее ненулевое направление
        if (inputDirection.sqrMagnitude > 0.0001f)
            LastMoveDirection = inputDirection;

        IsRunning = runInput && inputDirection.sqrMagnitude > 0.01f;

        float targetSpeed = IsRunning ? _runSpeed : _walkSpeed;
        Vector3 targetHorizontalVelocity = inputDirection * targetSpeed;

        float smoothTime = inputDirection.sqrMagnitude > 0.01f
            ? _accelerationTime
            : _decelerationTime;

        _horizontalVelocity = Vector3.SmoothDamp(
            _horizontalVelocity,
            targetHorizontalVelocity,
            ref _horizontalVelocitySmoothDamp,
            smoothTime
        );

        if (jumpInput && _isGrounded)
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);

        _velocity.y += _gravity * Time.deltaTime;

        Vector3 finalMove = _horizontalVelocity + Vector3.up * _velocity.y;
        _controller.Move(finalMove * Time.deltaTime);

        float horizontalSpeed = new Vector3(_horizontalVelocity.x, 0f, _horizontalVelocity.z).magnitude;
        CurrentSpeedPercent = Mathf.Clamp01(horizontalSpeed / _runSpeed);

        IsMoving = horizontalSpeed > 0.05f;

        InputManager.Instance.ConsumeInputs();
    }
}
