using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gameplay : MonoBehaviour
{
    ////////////////////
    // public Area
    public GameObject m_EnemyPfab, m_AttackerPfab, m_BallPfab;
    public GameObject m_Goal, m_BattleField;
    public EnergyBar m_EnemyEnergy, m_AttackerEnergy;
    public Text m_timeText;

    ////////////////////
    // private Area
    private List<AttackerBehavior> m_Attackers;
    private List<EnemyBehavior> m_Enemyers;
    private GameObject m_EnemysGroup, m_AttackersGroup;
    private GameObject m_Ball;

    private float m_timeRemain;

    //// Enenmy
    private float m_EnemyDetectRange;

    private void Awake()
    {
        m_Attackers = new List<AttackerBehavior>();
        m_Enemyers = new List<EnemyBehavior>();
        m_timeRemain = 140;
        
    }

    // Start is called before the first frame update
    void Start()
    {
        m_EnemysGroup = GameObject.Find("EnemysGroup");
        m_AttackersGroup = GameObject.Find("AttackersGroup");
        Vector3 Ballposition = new Vector3(Random.Range(-4.73f, 4.73f), 0.165f, Random.Range(-7.2f, 7.2f));
        m_Ball = Instantiate(m_BallPfab, Ballposition, Quaternion.identity);

        Bounds bounds = m_BattleField.GetComponent<MeshFilter>().mesh.bounds;
        m_EnemyDetectRange = bounds.size.x * EneDef.DetectionRange;


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
                    SpawnEnemy(spawnPost);
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

        m_timeRemain -= Time.deltaTime;
        m_timeText.text = string.Format ("{0:0}", m_timeRemain);
    }


    // User define method

    public Transform getBallTransform()
    {
        return m_Ball.transform;
    }

    public bool AttackersHasBall()
    {
        foreach (var att in m_Attackers)
        {
            if(att.HasBall())
                return true;
        }
        return false;
    }
    public void OnAttackersGetBall()
    {
        foreach (var att in m_Attackers)
        {
            if(!att.HasBall())
            {
                att.Target = null;
            }
            else
            {
                att.Target = m_Goal.transform;
            }
        }
    }

    public void DestroyAttacker(AttackerBehavior att)
    {
        m_Attackers.Remove(att);
        Destroy(att.gameObject, 0.1f);
    }

    public float GetEnemyDetectRange()
    {
        return m_EnemyDetectRange;
    }

    public List<EnemyBehavior> GetEnemyers()
    {
        return m_Enemyers;
    }
    public List<AttackerBehavior> GetAttackers()
    {
        return m_Attackers;
    }

    public AttackerBehavior FindAttackersNear(AttackerBehavior attacker)
    {
        AttackerBehavior near = null;
        float nearDist = 0.0f;
        foreach (var att in GetAttackers())
        {
            if(att == attacker) continue;
            if(near == null)
            {
                near = att;
                nearDist = Vector3.Distance(attacker.transform.position, att.transform.position);
                continue;
            }
            float dist = Vector3.Distance(attacker.transform.position, att.transform.position);
            if(dist < nearDist)
            {
                nearDist = dist;
                near = att;
            }
        }
        return near;
    }

    public bool PassBall(AttackerBehavior attKeeper)
    {
        AttackerBehavior near = FindAttackersNear(attKeeper);
        if (near != null)
        {
            m_Ball.transform.SetParent(near.transform, true);
            return true;
        }
        return false;
    }

    public void Arrested(EnemyBehavior enemy, AttackerBehavior attBallKeeper)
    {
        if(!attBallKeeper.HasBall()) return;

        if(PassBall(attBallKeeper))
        {
            attBallKeeper.Inactivated();
            enemy.Inactivated();
        }
        else
        {
            attBallKeeper.Inactivated();
            GameOver();
        }        
    }

    private void GameOver()
    {
        print("GameOver!!!");
    }

    



    private void SpawnAttacker(Vector3 spawnPost)
    {
        if(m_AttackerEnergy.SpawnPlayer(AttDef.EnergyCost))
        {
            GameObject attacker = Instantiate(m_AttackerPfab, spawnPost, Quaternion.identity, m_AttackersGroup.transform);
            Transform target;
            if(AttackersHasBall())
            {
                target = null;
            }
            else
            {
                target = m_Ball.transform;
            }
            AttackerBehavior att = attacker.GetComponent<AttackerBehavior>();
            att.Init(this, target, AttDef.SpawnTime);
            m_Attackers.Add(att);
        }
    }
    private void SpawnEnemy(Vector3 spawnPost)
    {
        if(m_EnemyEnergy.SpawnPlayer(EneDef.EnergyCost))
        {
            GameObject enemy = Instantiate(m_EnemyPfab, spawnPost, Quaternion.identity, m_EnemysGroup.transform);

            EnemyBehavior eb = enemy.GetComponent<EnemyBehavior>();
            eb.Init(this, EneDef.SpawnTime);
            m_Enemyers.Add(eb);
        }
    }
}
