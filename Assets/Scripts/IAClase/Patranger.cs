using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patranger : MonoBehaviour
{
    [SerializeField] bool _isChasing;
    [SerializeField] bool _isFighting;
    [SerializeField] bool _arrest;
    // Start is called before the first frame update
    public Lupinranger lupinranger;
    public bool IsChasing { get { return _isChasing; } }
    public bool IsFighting { get { return _isChasing; } }

    public bool Arrest { get { return _arrest; } }
    [SerializeField, Range(0f, 10f)] float _speed;
    public float Speed { get { return _speed; } }

    public Node firstNode;
    // Update is called once per frame
    void Update()
    {   
        if (Input.GetKey(KeyCode.J))
        {
            firstNode.Test(this);
        }

    }
}
/*
            if (lupinranger.IsStealing)
            {
                var dir = lupinranger.transform.position - transform.position;

                if (dir.magnitude < 2)
                {
                    if (lupinranger.IsArmed)
                    {
                        Debug.Log("By the Power bested in us by the Global Police we will handle you by force!");
                    }
                }
                else
                {
                    transform.position += dir.normalized * _speed * Time.deltaTime;
                }
            }
            else
            {
                Debug.Log("Damn you Lupinranger");
            }
        */