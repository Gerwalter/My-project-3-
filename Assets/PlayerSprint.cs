using System.Collections.Generic;
using UnityEngine;

public class PlayerSprint: IObservable
{
    private readonly PlayerController _player;
    private float _currentStamina;
    private bool _isSprinting;

    public PlayerSprint(PlayerController player)
    {
        _player = player;
        _currentStamina = player.StaminaMax;
    }
    private List<IObserver> _observers = new List<IObserver>();
    public void Subscribe(IObserver x)
    {
        if (_observers.Contains(x)) return;

        _observers.Add(x);
    }

    public void Unsubscribe(IObserver x)
    {
        if (_observers.Contains(x)) return;

        _observers.Remove(x);
    }

    public void Update()
    {
        Vector3 dir = _player.Direction;
        bool isMoving = dir.x != 0 || dir.z != 0;

        if (Input.GetKey(KeyCode.LeftShift) && _currentStamina > 0 && isMoving)
        {
            _isSprinting = true;
            _currentStamina -= _player.StaminaDrainRate * Time.deltaTime;
            EventManager.Trigger("Float", "Sprinting", 0f); // activo
            foreach (var obs in _observers)
            {
                obs.Notify(_currentStamina, _player.StaminaMax);
            }
        }
        else
        {
            _isSprinting = false;
            RegenerateStamina();
            EventManager.Trigger("Float", "Sprinting", 1f); // no sprint
            foreach (var obs in _observers)
            {
                obs.Notify(_currentStamina, _player.StaminaMax);
            }
        }

        _currentStamina = Mathf.Clamp(_currentStamina, 0, _player.StaminaMax);
    }

    private void RegenerateStamina()
    {
        float regenRate = (_player.Direction == Vector3.zero) ? _player.StaminaRegenRate * 1.5f : _player.StaminaRegenRate;
        _currentStamina += regenRate * Time.deltaTime;
    }
}
