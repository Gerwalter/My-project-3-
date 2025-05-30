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

            if (OnSlope(out RaycastHit slopeHit))
            {
                moveDir = GetSlopeDirection(moveDir, slopeHit.normal);
            }

            _player.Transform.forward = forward; // orienta al jugador
            _player.Rigidbody.MovePosition(_player.Transform.position + moveDir * _player.MoveSpeed * Time.fixedDeltaTime);
        }
    }

    private bool OnSlope(out RaycastHit hit)
    {
        Vector3 origin = _player.Transform.position;
        float checkDistance = _player.GroundCheckDistance + 0.1f;
        if (Physics.Raycast(origin, Vector3.down, out hit, checkDistance, _player.GroundMask))
        {
            float angle = Vector3.Angle(Vector3.up, hit.normal);
            return angle > 0f && angle < 45f;
        }
        return false;
    }

    private Vector3 GetSlopeDirection(Vector3 direction, Vector3 slopeNormal)
    {
        return Vector3.ProjectOnPlane(direction, slopeNormal).normalized;
    }
}
