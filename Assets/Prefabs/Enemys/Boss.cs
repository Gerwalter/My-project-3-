using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : HP
{
    [Header("<color=red>AI</color>")]
    [SerializeField] private float _chaseDist = 6.0f;
    [SerializeField] private float _atkDist = 2.0f;
    [SerializeField] public int _speed;
    [SerializeField] public EnemyType enemyType;

    [Header("<color=red>Behaviours</color>")]
    [SerializeField] private Animator _animator;
    [Header("<color=#6A89A7>UI</color>")]
    [SerializeField] private Image healthBar;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private float liftForce = 10.0f;

    [SerializeField]
    private Transform _target;

    private void Start()
    {
        _target = GameManager.Instance.Player.gameObject.transform;
        GetLife = maxLife;
    }

    public void ApplyLiftImpulse()
    {
        _groundCheckDistance = 0;
        rb.isKinematic = false;
        rb.AddForce(Vector3.up * liftForce, ForceMode.Impulse);
        StartCoroutine(CheckDistance());
    }

    IEnumerator CheckDistance()
    {
        yield return new WaitForSeconds(1);
        _groundCheckDistance = 1.1f;
    }

    [SerializeField] private float _groundCheckDistance = 1.1f;

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, _groundCheckDistance);
    }

    private void groundcheck()
    {
        if (IsGrounded())
        {

            rb.isKinematic = true;
        }
    }

    private void Update()
    {
        UpdateHealthBar();
        groundcheck();

        float distanceToPlayer = Vector3.Distance(transform.position, _target.position);
    }

    private void UpdateHealthBar()
    {
        float lifePercent = GetLife / maxLife;

        healthBar.fillAmount = lifePercent;

        healthBar.color = Color.Lerp(Color.red, Color.green, lifePercent);
    }
    [Header("<color=yellow>Attack</color>")]
    [SerializeField] private Transform _atkOrigin;
    [SerializeField] private float _atkRayDist = 1.0f;
    [SerializeField] private LayerMask _atkMask;
    [SerializeField] private int _atkDmg = 2;

    private Ray _atkRay;
    private RaycastHit _atkHit;
    public void Attack()
    {
        _atkRay = new Ray(_atkOrigin.position, transform.forward);

        if (Physics.Raycast(_atkRay, out _atkHit, _atkRayDist, _atkMask))
        {
            if (_atkHit.collider.TryGetComponent<Player>(out Player player))
            {
                player.ReciveDamage(_atkDmg);
            }
        }
    }

    public void ReciveDamage(int dmg)
    {

        GetLife -= dmg;
        if (GetLife <= 0)
        {
            Die();
        }
        else
        {
            _bloodVFX.SendEvent("OnTakeDamage");
        }

        //SFXManager.instance.PlayRandSFXClip(clips, transform, 1f);
    }

    private void Die()
    {
        LootData loot = WaveManager.Instance.GetLoot(enemyType);

        if (FindObjectOfType<PlayerStats>() is PlayerStats playerStats)
        {
            playerStats.AddLoot(loot);
        }

        Destroy(gameObject);
    }
}
