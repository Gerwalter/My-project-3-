using UnityEngine;

public class PlayerWallRun
{
    private readonly PlayerController _player;
    private bool _isWallRunning;
    private bool _wasWallDetectedLastFrame;

    private Vector3[] directions = new Vector3[]
    {
        Vector3.forward, Vector3.back, Vector3.left, Vector3.right,
        (Vector3.forward + Vector3.right).normalized,
        (Vector3.forward + Vector3.left).normalized,
        (Vector3.back + Vector3.right).normalized,
        (Vector3.back + Vector3.left).normalized
    };

    public PlayerWallRun(PlayerController player)
    {
        _player = player;
    }

    public void Update()
    {
        bool isGrounded = Physics.Raycast(_player.Transform.position, Vector3.down, _player.GroundCheckDistance, _player.GroundMask);
        bool wallDetected = IsNearWall();

        if (!isGrounded && wallDetected)
        {
            StartWallRun();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                EventManager.Trigger("Input", "onJump");
                Vector3 normal = GetWallNormal();
                _player.Rigidbody.AddForce((normal + Vector3.up).normalized * _player.JumpForce, ForceMode.Impulse);
            }
        }
        else if (_isWallRunning && (isGrounded || !wallDetected))
        {
            StopWallRun();
        }

        _wasWallDetectedLastFrame = wallDetected;
    }

    private bool IsNearWall()
    {
        foreach (var dir in directions)
        {
            if (Physics.Raycast(_player.Transform.position, dir, _player.WallCheckDistance, _player.WallMask))
                return true;
        }
        return false;
    }

    private Vector3 GetWallNormal()
    {
        Vector3 wallNormal = Vector3.zero;
        int count = 0;

        foreach (var dir in directions)
        {
            if (Physics.Raycast(_player.Transform.position, dir, out RaycastHit hit, _player.WallCheckDistance, _player.WallMask))
            {
                wallNormal += -dir;
                count++;
            }
        }

        return (count > 0) ? (wallNormal / count).normalized : Vector3.zero;
    }

    private void StartWallRun()
    {
        if (_isWallRunning) return;

        _isWallRunning = true;
        _player.Rigidbody.useGravity = false;
        Debug.Log("Wall Run Activado!");
    }

    private void StopWallRun()
    {
        if (!_isWallRunning) return;

        _isWallRunning = false;
        _player.Rigidbody.useGravity = true;
        Debug.Log("Wall Run Desactivado.");
    }
}
