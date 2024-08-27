using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAnim : MonoBehaviour
{
    public GameObject sword;
    public bool canAttack = true;
    public float attackCooldown = 1.0f;
    public bool isAttacking = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SwordAttack();
        }
    }

    private void SwordAttack()
    {
        canAttack = false;
    }

    IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
