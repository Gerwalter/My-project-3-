using UnityEngine;

public class PlayerCrouch
{
    private readonly PlayerController _player;
    private bool _isCrouching;

    [Header("Crouch Settings")]
    public float crouchSpeedMultiplier = 0.5f;
    public float crouchHeight = 1.2f;
    public float crouchCenterY = 0.57f; // Nuevo valor al agacharse

    private float originalHeight;
    private float originalCenterY;

    // Para IA
    public LayerMask normalLayer;
    public LayerMask stealthLayer;

    private CapsuleCollider _collider;

    public PlayerCrouch(PlayerController player)
    {
        _player = player;
        _collider = player.GetComponent<CapsuleCollider>();

        if (_collider != null)
        {
            originalHeight = _collider.height;
            originalCenterY = _collider.center.y; // Guardar el center original (0.8 en tu caso)
        }

        normalLayer = player.gameObject.layer;
        stealthLayer = LayerMask.NameToLayer("PlayerStealth");
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            StartCrouch();
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            StopCrouch();
        }

        EventManager.Trigger("Bool", "isCrouching", _isCrouching);
    }

    private void StartCrouch()
    {
        _isCrouching = true;
        EventManager.Trigger("Input", "onCrouch");

        if (_collider != null)
        {
            _collider.height = crouchHeight;
            _collider.center = new Vector3(_collider.center.x, crouchCenterY, _collider.center.z);
        }

        _player.gameObject.layer = stealthLayer;
    }

    private void StopCrouch()
    {
        _isCrouching = false;
        EventManager.Trigger("Input", "onStand");

        if (_collider != null)
        {
            _collider.height = originalHeight;
            _collider.center = new Vector3(_collider.center.x, originalCenterY, _collider.center.z);
        }

        _player.gameObject.layer = normalLayer;
    }

    public bool IsCrouching => _isCrouching;
}
