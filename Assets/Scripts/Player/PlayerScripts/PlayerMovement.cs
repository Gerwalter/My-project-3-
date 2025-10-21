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

        // Nuevo sistema simplificado
        float moveValue = isMoving ? 1f : 0f;
        EventManager.Trigger("Float", "Move", moveValue);
    }

    public void FixedUpdate()
    {
        Vector3 dir = _player.Direction.normalized;
        if (dir != Vector3.zero)
        {
            Vector3 camForward = Camera.main.transform.forward;
            Vector3 camRight = Camera.main.transform.right;
            camForward.y = 0;
            camRight.y = 0;

            // Dirección de movimiento basada en cámara
            Vector3 moveDir = (camRight * dir.x + camForward * dir.z).normalized;

            if (OnSlope(out RaycastHit slopeHit))
            {
                moveDir = GetSlopeDirection(moveDir, slopeHit.normal);
            }

            // Rotación suave hacia la dirección de movimiento (más estable visualmente)
            Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            _player.Transform.rotation = Quaternion.Slerp(
                _player.Transform.rotation,
                targetRotation,
                18f * Time.fixedDeltaTime // ajustá este valor para rotaciones más o menos rápidas
            );

            float speed = _player.MoveSpeed;

            if (_player.Stamina.IsSprinting)
                speed *= 1.8f;

            if (_player.Stamina.IsDashing)
            {
                Vector3 dashDir = _player.Transform.forward * 10f;
                _player.Rigidbody.AddForce(dashDir, ForceMode.VelocityChange);
            }

            if (_player.Crouch != null && _player.Crouch.IsCrouching)
                speed *= _player.Crouch.crouchSpeedMultiplier;

            _player.Rigidbody.MovePosition(
                _player.Transform.position + moveDir * speed * Time.fixedDeltaTime
            );
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
