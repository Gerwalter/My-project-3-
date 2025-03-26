using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {
        transform.position += Velocity*Time.deltaTime;
        transform.forward = Velocity;

        AddForce(Flee(_target.position));
    }
    [SerializeField] float MaxVelocity;
    [SerializeField, Range(0,1)] float MaxForce;
    [SerializeField] Transform _target;
    Vector3 Seek(Vector3 target)
    {
        Vector3 desired =  target - transform.position;
        desired = desired.normalized;
        desired *= MaxVelocity;

        Vector3 steering = desired - Velocity;
        steering = Vector3.ClampMagnitude(steering, MaxForce);
        return steering;
    }

    public Vector3 Flee(Vector3 target)
    {
        return -Seek(target);
    }
    [SerializeField] Vector3 Velocity;
    void AddForce(Vector3 dir)
    {
        Velocity = Vector3.ClampMagnitude(Velocity + dir, MaxVelocity); 
    }
}
