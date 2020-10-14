using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameplay : MonoBehaviour
{

    public GameObject EnemypFab, AttackerPfab, BallPfab;

    public GameObject Goal;
    private List<GameObject> Enemys;
    private List<AttackerBehavior> Attackers;

    private GameObject EnemysGroup, AttackersGroup, Ball;


    // Start is called before the first frame update
    void Start()
    {
        EnemysGroup = GameObject.Find("EnemysGroup");
        AttackersGroup = GameObject.Find("AttackersGroup");
        Enemys = new List<GameObject>();
        Attackers = new List<AttackerBehavior>();
        Vector3 Ballposition = new Vector3(Random.Range(-4.73f, 4.73f), 0, Random.Range(-7.2f, 7.2f));
        //Ball = Instantiate(BallPfab, Ballposition, Quaternion.identity);
        Ball = GameObject.Find("Soccer Ball");
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
                    GameObject enemy = Instantiate(EnemypFab, newpoint, Quaternion.identity, EnemysGroup.transform);
                    Enemys.Add(enemy);

                }
                else
                {
                    GameObject attacker = Instantiate(AttackerPfab, newpoint, Quaternion.identity, AttackersGroup.transform);
                    Transform target;
                    if(AttackersHasBall())
                    {
                        target = null;
                    }
                    else
                    {
                        target = Ball.transform;
                    }
                    AttackerBehavior att = attacker.GetComponent<AttackerBehavior>();
                    att.Init(this, target);
                    Attackers.Add(att);
                    //Enemys.Add( Instantiate(AttackerPfab, newpoint, Quaternion.identity, EnemysGroup.transform));
                }
            }
            else
            {
                Debug.Log("Can't Raycast. mouse: " + Input.mousePosition);
            }
        }
    }


    // User define method

    public Transform getBallTransform()
    {
        return Ball.transform;
    }

    public bool AttackersHasBall()
    {
        foreach (var att in Attackers)
        {
            if(att.HasBall())
                return true;
        }
        return false;
    }
    public void OnAttackersGetBall()
    {
        foreach (var att in Attackers)
        {
            if(!att.HasBall())
            {
                att.SetTarget(null);
            }
            else
            {
                att.SetTarget(Goal.transform);
            }
        }
    }
}
