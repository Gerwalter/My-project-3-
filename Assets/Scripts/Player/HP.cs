using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public abstract class HP : MonoBehaviour, ILife
{
    [SerializeField] public float maxLife;
    [SerializeField] private float currentLife;



    public float GetLife
    {
        get { return currentLife; }
        set { currentLife = Mathf.Clamp(value, 0, maxLife); }
    }

    private void Start()
    {
        currentLife = maxLife;

    }

    [SerializeField] public VisualEffect _bloodVFX;

    public virtual void ReciveDamage(float damage)
    {

    }
    public virtual void Health(float amount)
    { }

}
