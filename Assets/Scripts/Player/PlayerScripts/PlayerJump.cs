using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerJump
{
    private readonly PlayerController _player;

    public PlayerJump(PlayerController player)
    {
        _player = player;
    }
    public void Initialize()
    {
        if (_player.Rigidbody == null)
        {
            _player.Rigidbody = _player.GetComponent<Rigidbody>();
            Debug.LogWarning("PlayerJump tuvo que reasignar el Rigidbody manualmente.");
        }
    }
    public void Update()
    {
        bool grounded = IsGrounded();
        EventManager.Trigger("Bool", "isGrounded", grounded);

        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            EventManager.Trigger("Input", "onJump");

        }
    }
    public void Start()
    {
        EventManager.Subscribe("OnJump", JumpUp);
    }
    private void JumpUp(params object[] args)
    {
        Jump(Vector3.up);
    }
    public void Jump(Vector3 direction)
    {
        if (_player.Rigidbody == null)
        {
            Debug.LogError("Rigidbody del PlayerController no está inicializado antes del salto.");
            return;
        }

        _player.Rigidbody.AddForce(direction * _player.JumpForce, ForceMode.Impulse);
    }

    private bool IsGrounded()
    {
        Vector3 origin = _player.Transform.position + Vector3.up * 0.125f;
        return Physics.Raycast(origin, Vector3.down, _player.GroundCheckDistance, _player.GroundMask);
    }
}
