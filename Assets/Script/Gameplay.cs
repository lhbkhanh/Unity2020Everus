using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameplay : MonoBehaviour
{

    public GameObject AttackerPfab, EnemypFab, BallPfab;
    private List<GameObject> Enemys, Attackers;

    private GameObject EnemysGroup, AttackersGroup, Ball;


    // Start is called before the first frame update
    void Start()
    {
        EnemysGroup = GameObject.Find("EnemysGroup");
        AttackersGroup = GameObject.Find("AttackersGroup");
        Enemys = Attackers = new List<GameObject>();
        Vector3 Ballposition = new Vector3(Random.Range(-4.73f, 4.73f), 0, Random.Range(-7.2f, 7.2f));
        Ball = Instantiate(BallPfab, Ballposition, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
    RaycastHit outRaycast;
        if (Input.GetMouseButtonDown(0))
        {
            int layerMask = 1 << 8;
            Ray ray = Camera.main. ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out outRaycast, 1000, layerMask) )           
            {
                Vector3 newpoint = new Vector3(outRaycast.point.x, AttackerPfab.transform.position.y, outRaycast.point.z);
                if(outRaycast.point.z > 0)
                {
                    Enemys.Add( Instantiate(EnemypFab, newpoint, Quaternion.identity, AttackersGroup.transform));
                }
                else
                {
                    Attackers.Add( Instantiate(AttackerPfab, newpoint, Quaternion.identity, EnemysGroup.transform));
                }
            }
            else
            {
                Debug.Log("Can't Raycast. mouse: " + Input.mousePosition);
            }
        }
    }
}
