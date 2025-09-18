using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FOVAgent : FOVTarget
{
    [SerializeField] List<FOVTarget> _otherAgents;

    [SerializeField] LayerMask _obstacle;

    [SerializeField, Range(0.5f, 15)] float _viewRange; //Esfera
    [SerializeField, Range(15, 360)] float _viewAngle; //Cono
    public float ViewRange
    {
        get => _viewRange;
        set => _viewRange = Mathf.Clamp(value, 0.5f, 15f); // respeta el rango
    }

    public float ViewAngle
    {
        get => _viewAngle;
        set => _viewAngle = Mathf.Clamp(value, 15f, 360f); // respeta el rango
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
      //  ChangeColor(Color.green);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var agent in _otherAgents)
        {
         //   agent.ChangeColor(InFOV(agent.transform.position) ? Color.red : Color.white);

            //if(InFOV(agent.transform.position)) agent.ChangeColor(Color.red);
            //else agent.ChangeColor(Color.white);
        }
    }

    //Field of View
    public bool InFOV(Vector3 endPos)
    {
        Vector3 dir = endPos - transform.position;
        if(!InLOS(transform.position, endPos)) return false;
        if(dir.magnitude > _viewRange) return false;
        if(Vector3.Angle(transform.forward, dir) > _viewAngle / 2) return false;
        return true;
    }

    //Line of Sight
    bool InLOS(Vector3 start, Vector3 end)
    {
        Vector3 dir = end - start;

     //   Debug.DrawRay(start, dir, Color.red, dir.magnitude);

        return !Physics.Raycast(start, dir.normalized, dir.magnitude, _obstacle); 
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, _viewRange);

        Gizmos.color = Color.cyan;
        Vector3 dirA = GetAngleFromDir(_viewAngle / 2 + transform.eulerAngles.y);
        Vector3 dirB = GetAngleFromDir(-_viewAngle / 2 + transform.eulerAngles.y);
        Gizmos.DrawLine(transform.position, transform.position + dirA.normalized * _viewRange);
        Gizmos.DrawLine(transform.position, transform.position + dirB.normalized * _viewRange);
    }

    Vector3 GetAngleFromDir(float angleInDegrees) => new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
}
