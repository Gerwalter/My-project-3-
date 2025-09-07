using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerStamina : IStaminaObservable
{
    private readonly PlayerController _player;

    public float MaxStamina = 100f;
    public float CurrentStamina;
    public float SprintDrain = 15f;   // stamina/seg
    public float DashCost = 25f;      // stamina por dash
    public float RegenRate = 10f;     // stamina/seg

    public bool IsSprinting { get; private set; }
    public bool IsDashing { get; private set; }

    private float dashCooldown = 0.5f;
    private float lastDashTime;

    public PlayerStamina(PlayerController player)
    {
        _player = player;
        CurrentStamina = MaxStamina;
    }

    public void Update()
    {
        HandleSprint();
        HandleDash();
        Regenerate();

        NotifyObservers();
    }

    private bool sprintLocked;
    public float SprintUnlockThreshold = 0.5f; //  porcentaje necesario para volver a sprintar

    private void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time > lastDashTime + dashCooldown && CurrentStamina >= DashCost)
        {
            IsDashing = true;
            lastDashTime = Time.time;
            CurrentStamina -= DashCost;
        }
        else
        {
            IsDashing = false;
        }
    }
    private void HandleSprint()
    {
        if (sprintLocked)
        {
            IsSprinting = false;
            return;
        }

        if (Input.GetKey(KeyCode.LeftShift) && _player.Direction.magnitude > 0.1f && CurrentStamina > 0)
        {
            IsSprinting = true;
            CurrentStamina = Mathf.Max(0, CurrentStamina - SprintDrain * Time.deltaTime);

            if (CurrentStamina <= 0)
            {
                sprintLocked = true;
                IsSprinting = false;
            }
        }
        else
        {
            IsSprinting = false;
        }
    }

    private void Regenerate()
    {
        if (!IsSprinting && !IsDashing && CurrentStamina < MaxStamina)
        {
            CurrentStamina = Mathf.Min(MaxStamina, CurrentStamina + RegenRate * Time.deltaTime);

            // ✅ Desbloquear sprint al llegar al umbral
            if (CurrentStamina >= MaxStamina * SprintUnlockThreshold)
            {
                sprintLocked = false;
            }
        }
    }
    [SerializeField] List<IStaminaObserver> observers = new List<IStaminaObserver>();
    public void Subscribe(IStaminaObserver x)
    {
        if (!observers.Contains(x)) observers.Add(x);
    }

    public void Unsubscribe(IStaminaObserver x)
    {
        if (observers.Contains(x)) observers.Remove(x);
    }

    private void NotifyObservers()
    {
        foreach (var obs in observers)
        {
            obs.Notify(CurrentStamina, MaxStamina);
        }
    }
}
