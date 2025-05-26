using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creator : MonoBehaviour
{
    ObjectPool<Bullet2> pool;
    public Bullet2 bullet;    // Start is called before the first frame update
    void Start()
    {

    }
    private void Awake()
    {
        //pool = new ObjectPool<Bullet2>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Instantiate(bullet, transform.position, transform.rotation);
        }
    }
}
