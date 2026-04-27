using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float _interactRange = 3f;
    [SerializeField] private LayerMask _interactLayer;
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private float _interactCooldown = 0.5f;

    [SerializeField] private GameObject _promptPanel;
    [SerializeField] private TextMeshProUGUI _promptText;
    [SerializeField] private GameObject _holdProgressPanel;
    [SerializeField] private Image _holdProgressBar;

    private IInteractable _currentInteractable;
    private float _holdTimer;
    private bool _isHolding;
    private float _cooldownTimer;

    private bool IsOnCooldown => _cooldownTimer > 0f;

    private void Update()
    {
        if (IsOnCooldown)
        {
            _cooldownTimer -= Time.deltaTime;
            if (_cooldownTimer < 0f) _cooldownTimer = 0f;
        }

        DetectInteractable();
        HandleInput();
        UpdateHoldUI();
    }

    private void DetectInteractable()
    {
        Ray ray = _playerCamera.ScreenPointToRay(
            new Vector3(Screen.width * 0.5f, Screen.height * 0.5f)
        );

        if (Physics.Raycast(ray, out RaycastHit hit, _interactRange, _interactLayer))
        {
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
            if (interactable != null)
            {
                if (_currentInteractable != interactable)
                {
                    ResetHold();
                }

                _currentInteractable = interactable;
                ShowPrompt(interactable);
                return;
            }
        }

        _currentInteractable = null;
        HidePrompt();
        ResetHold();
    }

    private void HandleInput()
    {
        if (_currentInteractable == null)
        {
            ResetHold();
            return;
        }

        if (!_currentInteractable.CanInteract || IsOnCooldown)
        {
            ResetHold();
            return;
        }

        if (_currentInteractable.RequiresHold)
        {
            if (InputManager.Instance.Interact)
            {
                _isHolding = true;
                _holdTimer += Time.deltaTime;

                if (_holdTimer >= _currentInteractable.HoldDuration)
                {
                    _currentInteractable.OnHoldInteract();
                    StartCooldown();
                    ResetHold();
                }
            }
            else
            {
                ResetHold();
            }
        }
        else
        {
            if (InputManager.Instance.Interact)
            {
                _currentInteractable.OnInteract();
                InputManager.Instance.ConsumeInteract();
                StartCooldown();
            }
        }
    }

    private void UpdateHoldUI()
    {
        if (_holdProgressPanel == null) return;

        bool showHoldUI =
            _isHolding &&
            _currentInteractable != null &&
            _currentInteractable.RequiresHold &&
            _currentInteractable.CanInteract &&
            !IsOnCooldown;

        _holdProgressPanel.SetActive(showHoldUI);

        if (showHoldUI && _holdProgressBar != null)
        {
            float duration = Mathf.Max(_currentInteractable.HoldDuration, 0.0001f);
            _holdProgressBar.fillAmount = _holdTimer / duration;
        }
    }

    private void ShowPrompt(IInteractable interactable)
    {
        if (_promptPanel != null) _promptPanel.SetActive(true);
        if (_promptText == null) return;

        if (IsOnCooldown)
        {
            _promptText.text = $"... ({_cooldownTimer:F1}с)";
            return;
        }

        string baseText = interactable.GetPromptText();

        if (!interactable.CanInteract)
        {
            _promptText.text = baseText;
            return;
        }

        if (interactable.RequiresHold)
        {
            _promptText.text =
                $"[Зажмите E]\n{baseText}\n({interactable.HoldDuration:0.0}с)";
        }
        else
        {
            _promptText.text = $"[E] {baseText}";
        }
    }

    private void HidePrompt()
    {
        if (_promptPanel != null) _promptPanel.SetActive(false);
    }

    private void ResetHold()
    {
        _holdTimer = 0f;
        _isHolding = false;
    }

    private void StartCooldown()
    {
        _cooldownTimer = _interactCooldown;
    }
}
