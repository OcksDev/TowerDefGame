using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explodie : MonoBehaviour
{
    public float Life = 1;
    public float Size = 1;
    float z = 1;
    private void Awake()
    {
        z = Life;
    }
    void FixedUpdate()
    {
        z = Mathf.Max(z-Time.deltaTime,0);
        transform.localScale = Vector3.one * Size * (z/Life);
        if(z <= 0) Destroy(gameObject);
    }
}
