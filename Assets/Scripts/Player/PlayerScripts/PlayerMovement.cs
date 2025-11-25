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

        // Feedback de animación
        float moveValue = isMoving ? 1f : 0f;
        EventManager.Trigger("Float", "Move", moveValue);
    }

    public void FixedUpdate()
    {
        Vector3 dir = _player.Direction.normalized;
        if (dir == Vector3.zero) return;

        // Dirección relativa a la cámara
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;
        camForward.y = 0;
        camRight.y = 0;

        Vector3 moveDir = (camRight * dir.x + camForward * dir.z).normalized;

        // Ajuste en pendientes
        if (OnSlope(out RaycastHit slopeHit))
        {
            moveDir = GetSlopeDirection(moveDir, slopeHit.normal);
        }

        // Rotación suave hacia la dirección de movimiento
        Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
        _player.Transform.rotation = Quaternion.Slerp(
            _player.Transform.rotation,
            targetRotation,
            18f * Time.fixedDeltaTime
        );

        float speed = _player.MoveSpeed;

     //   if (_player.Stamina.IsSprinting)
       //     speed *= 1.8f;

        if (_player.Crouch != null && _player.Crouch.IsCrouching)
            speed *= _player.Crouch.crouchSpeedMultiplier;

        Vector3 targetPos = _player.Transform.position + moveDir * speed * Time.fixedDeltaTime;

        // -----------------------------
        // -----------------------------
        float radius = 0.22f; // Ajustar según tu collider
        float height = 1.8f;
        Vector3 point1 = _player.Transform.position + Vector3.up * 0.1f;
        Vector3 point2 = point1 + Vector3.up * (height - 0.2f);

        bool blocked = Physics.CapsuleCast(
            point1,
            point2,
            radius,
            moveDir,
            out RaycastHit wallHit,
            speed * Time.fixedDeltaTime,
            _player.WallMask
        );

        if (!blocked)
        {
            _player.Rigidbody.MovePosition(targetPos);
        }

        if (_player.Direction == Vector3.zero)
        {
            _player.Rigidbody.velocity = Vector3.zero;
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
