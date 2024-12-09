using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2Avatar : MonoBehaviour
{
    public Enemy2 _parent;

    private void Start()
    {
        _parent = GetComponentInParent<Enemy2>();
    }
    public void Attack()
    {
        _parent.Attack();
    }
}
