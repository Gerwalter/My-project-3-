using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public float attackCooldown = 0.2f;
    public float dodgeCooldown = 1.0f;
    public float witchTimeDuration = 2.0f;
    public float dodgeInvulnerabilityTime = 0.5f;
    public float timeSlowFactor = 0.3f;
    private float comboTimer = 0.5f;
    private float lastAttackTime;
    private List<string> currentCombo = new List<string>();
    public Material mat;
    private Dictionary<string, List<string>> comboSequences = new Dictionary<string, List<string>>()
    {
        {"LLH", new List<string>{ "LightAttack1", "LightAttack2", "HeavyAttackFinish" }},
        {"HHL", new List<string>{ "HeavyAttack1", "HeavyAttack2", "LightAttackFinish" }},
        {"LHL", new List<string>{ "LightAttack1", "HeavyAttack1", "LightAttackFinish" }},
        {"HLH", new List<string>{ "HeavyAttack1", "LightAttack2", "HeavyAttackFinish" }}
    };

    //private Animator anim;
    private bool canAttack = true;
    private bool canDodge = true;

    void Start()
    {
        mat.color = Color.white;
        //anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && canAttack) // Click Izquierdo: Ataque ligero
        {
            RegisterAttack("L");
        }
        if (Input.GetKeyDown(KeyCode.Mouse1) && canAttack) // Click Derecho: Ataque pesado
        {
            RegisterAttack("H");
        }
        if (Input.GetKeyDown(KeyCode.LeftControl) && canDodge) // Espacio: Esquivar
        {
            StartCoroutine(Dodge());
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ActivateWitchTime();
        }
    }
    void RegisterAttack(string attackType)
    {
        if (Time.time - lastAttackTime > comboTimer)
        {
            currentCombo.Clear();
        }

        currentCombo.Add(attackType);
        string comboKey = string.Join("", currentCombo);

        if (comboSequences.ContainsKey(comboKey))
        {
            ExecuteCombo(comboSequences[comboKey]);
        }
        else
        {
            Attack(attackType == "L" ? "LightAttack1" : "HeavyAttack1");
        }

        lastAttackTime = Time.time;
    }

    void ExecuteCombo(List<string> combo)
    {
        StartCoroutine(PerformCombo(combo));
    }

    IEnumerator PerformCombo(List<string> combo)
    {
        canAttack = false;
        foreach (string attack in combo)
        {
            //anim.SetTrigger(attack);
            yield return new WaitForSeconds(attackCooldown);
        }
        canAttack = true;
        currentCombo.Clear();
    }

    void Attack(string attackType)
    {
        canAttack = false;
        //anim.SetTrigger(attackType);
        //print(attackType);
        StartCoroutine(ResetAttackCooldown());
    }

    IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    IEnumerator Dodge()
    {
        canDodge = false;
        //anim.SetTrigger("Dodge");
        gameObject.layer = LayerMask.NameToLayer("Invulnerable"); // Hacer invulnerable temporalmente
        yield return new WaitForSeconds(dodgeInvulnerabilityTime);
        gameObject.layer = LayerMask.NameToLayer("Player"); // Restaurar capa normal
        yield return new WaitForSeconds(dodgeCooldown - dodgeInvulnerabilityTime);
        canDodge = true;
    }

    public void ActivateWitchTime()
    {
        StartCoroutine(WitchTime());
    }

    IEnumerator WitchTime()
    {
        mat.color = Color.yellow;
        Time.timeScale = timeSlowFactor;
        GameObject player = this.gameObject;
        float originalPlayerSpeed = player.GetComponent<PlayerMovement>()._movSpeed;
        player.GetComponent<PlayerMovement>()._movSpeed = originalPlayerSpeed / timeSlowFactor; // Mantener velocidad del jugador

        yield return new WaitForSecondsRealtime(witchTimeDuration);

        Time.timeScale = 1.0f;
        player.GetComponent<PlayerMovement>()._movSpeed = originalPlayerSpeed;
        mat.color = Color.white;
    }
}
