using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gameplay : MonoBehaviour
{

    public GameObject m_EnemypFab, m_AttackerPfab, m_BallPfab;

    public GameObject Goal, Ball;
    public EnergyBar EnemyEnergy, AttackerEnergy;

    public Text timeText;
    private List<GameObject> Enemys;
    private List<AttackerBehavior> Attackers;

    private GameObject EnemysGroup, AttackersGroup;

    private float timeRemain;

    private void Awake()
    {
        timeRemain = 140;
    }

    // Start is called before the first frame update
    void Start()
    {
        EnemysGroup = GameObject.Find("EnemysGroup");
        AttackersGroup = GameObject.Find("AttackersGroup");
        Enemys = new List<GameObject>();
        Attackers = new List<AttackerBehavior>();
        Vector3 Ballposition = new Vector3(Random.Range(-4.73f, 4.73f), 0.165f, Random.Range(-7.2f, 7.2f));
        Ball.transform.position = Ballposition;

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
                Vector3 spawnPost = new Vector3(outRaycast.point.x, m_AttackerPfab.transform.position.y, outRaycast.point.z);
                if(outRaycast.point.z > 0)
                {
                    GameObject enemy = Instantiate(m_EnemypFab, spawnPost, Quaternion.identity, EnemysGroup.transform);
                    Enemys.Add(enemy);

                }
                else
                {
                    SpawnAttacker(spawnPost);
                }
            }
            else
            {
                Debug.Log("Can't Raycast. mouse: " + Input.mousePosition);
            }
        }

        timeRemain -= Time.deltaTime;
        timeText.text = string.Format ("{0:0}", timeRemain);
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
    public void DestroyAttacker(AttackerBehavior att)
    {
        Attackers.Remove(att);
        Destroy(att.gameObject, 0.1f);
    }


    private void SpawnAttacker(Vector3 spawnPost)
    {
        if(AttackerEnergy.SpawnPlayer(AttDef.EnergyCost))
        {
            GameObject attacker = Instantiate(m_AttackerPfab, spawnPost, Quaternion.identity, AttackersGroup.transform);
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
            att.Init(this, target, AttDef.SpawnTime);
            Attackers.Add(att);
            //Enemys.Add( Instantiate(AttackerPfab, newpoint, Quaternion.identity, EnemysGroup.transform));
        }
    }
}
