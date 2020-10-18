using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public Gameplay m_GP;
    public GameObject target, gold;

    // private Ray ray ;


    // Start is called before the first frame update
    void Start()
    {
    //    ray = new Ray(target.transform.position, gold.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        // Vector3 targetDir = gold.transform.position - transform.position;
        // targetDir.y = 0;
        // Vector3 forward = transform.forward;
        // float angle = Vector3.SignedAngle(targetDir, forward, Vector3.up);
        
    }

     public void OnTriggerEnter(Collider other)
    {        
        if(other.gameObject.CompareTag(GL.TAG_GOAL))
        {
            m_GP.GOALLLL();
        }
    }
}
