using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAvatar : MonoBehaviour
{
    public Boss _parent;

    private void Start()
    {
        _parent = GetComponentInParent<Boss>();
    }

}
