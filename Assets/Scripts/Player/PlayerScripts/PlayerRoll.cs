using UnityEngine;
using System.Collections;

public class PlayerRoll
{
    private readonly PlayerController _player;

    [Header("Roll Settings")]
    public float rollForce = 8f;
    public float rollDuration = 0.4f;
    public float rollCooldown = 1f;

    private bool _isRolling;
    private bool _canRoll = true;

    public PlayerRoll(PlayerController player)
    {
        _player = player;
    }

    public void TryRoll()
    {
        if (!_canRoll || _isRolling) return;

        _player.StartCoroutine(RollCoroutine());
    }

    private IEnumerator RollCoroutine()
    {
        _canRoll = false;
        _isRolling = true;

        EventManager.Trigger("Input", "onRoll");

        Vector3 dir = _player.Transform.forward;
        _player.Rigidbody.AddForce(dir * rollForce, ForceMode.VelocityChange);

        yield return new WaitForSeconds(rollDuration);

        _isRolling = false;

        yield return new WaitForSeconds(rollCooldown);
        _canRoll = true;
    }

    public bool IsRolling => _isRolling;
}

