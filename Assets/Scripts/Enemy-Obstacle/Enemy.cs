using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : Entity
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Image healthBar;
    private float lastHitTime;
    private float hitCooldown = 0.5f;

    private Rigidbody rb;


    protected override void Awake()
    {
        base.Awake();
        GameManager.Instance.Enemies.Add(this);

        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }


    protected override void HandleDamage(float damage)
    {
        base.HandleDamage(damage);
        if (_animator != null && Time.time >= lastHitTime + hitCooldown)
        {
            _animator.SetTrigger("Hit");
            lastHitTime = Time.time;  // Actualizar el tiempo del último hit
        }
        // Actualización de barra de vida específica del enemigo, si es necesario
    }

    protected override void HandleDeath()
    {
        base.HandleDeath();
   //     _agent.isStopped = true;
        _animator.SetTrigger("Die");
        GameManager.Instance.Enemies.Remove(this);
        Destroy(gameObject, 2.0f); // Destruir después de la animación de muerte
    }



    private void FixedUpdate()
    {
        UpdateHealthBar();
    }
      
    private void UpdateHealthBar()
    {
        float lifePercent = GetLife / maxLife;

        healthBar.fillAmount = lifePercent;

        healthBar.color = Color.Lerp(Color.red, Color.green, lifePercent);
    }
}
