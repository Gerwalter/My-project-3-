using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAvatar : MonoBehaviour
{
    public Enemy _parent;

    private void Start()
    {
        _parent = GetComponentInParent<Enemy>();
    }
    public void Attack()
    {
        _parent.Attack();
    }
}
