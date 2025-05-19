using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : HP
{
    public override void Health(float amount)
    {
        throw new System.NotImplementedException();
    }

    

    // Start is called before the first frame update
    void Start()
    {
        GetLife = maxLife;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
