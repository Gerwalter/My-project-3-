using UnityEngine;

public class PlayerMovement
{
    private readonly PlayerController _player;

    public PlayerMovement(PlayerController player)
    {
        _player = player;
    }

    public void Update()
    {
        Vector3 dir = _player.Direction;

        bool isMoving = dir.x != 0 || dir.z != 0;
        EventManager.Trigger("Float", "xAxis", dir.x);
        EventManager.Trigger("Float", "zAxis", dir.z);
        EventManager.Trigger("Bool", "isMoving", isMoving);
    }

    public void FixedUpdate()
    {
        Vector3 dir = _player.Direction.normalized;
        if (dir != Vector3.zero)
        {
            Vector3 forward = Camera.main.transform.forward;
            Vector3 right = Camera.main.transform.right;
            forward.y = 0;
            right.y = 0;
            Vector3 moveDir = (right * dir.x + forward * dir.z).normalized;

            _player.Transform.forward = forward; // Optional: rotate player

            _player.Rigidbody.MovePosition(_player.Transform.position + moveDir * _player.MoveSpeed * Time.fixedDeltaTime);
        }
    }
}
