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
    public Text m_timeText, m_WinText, m_enemyLabel, m_playerLabel;

    ////////////////////
    // private Area
    private List<AttackerBehavior> m_Attackers;
    private List<EnemyBehavior> m_Enemyers;
    private GameObject m_EnemysGroup, m_AttackersGroup;
    private GameObject m_Ball;

    private float m_timeRemain;

    private bool m_pause;
    private float m_timeRestart;
    private int m_enemyWin, m_attackWin;

    //// Enenmy
    private float m_EnemyDetectRange;

    private string m_strRetart;

    /////////////////
    /////// property
    public bool PauseGame
    {
        get { return m_pause;}
        //set { m_Target = value; }
    }      

    private void Awake()
    {
        m_Attackers = new List<AttackerBehavior>();
        m_Enemyers = new List<EnemyBehavior>();
        m_timeRemain = 140;
        m_strRetart = "";
        m_enemyWin = m_attackWin = 0;
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
        m_WinText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(!m_pause) 
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
            if(m_timeRemain <=0) Protected();
            m_timeText.text = string.Format ("{0:0}", m_timeRemain);
        }
        else
        {
            m_timeRestart -= Time.deltaTime;
            if(m_timeRestart <= 0)
            {
                restart();
            }

            string str = m_strRetart + "\nRestart in " + (int)m_timeRestart;
            m_WinText.text = str;
        }
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
        foreach (var att in m_Attackers)
        {
            if(att == attacker || att.PlayActive) continue;
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
            //m_Ball.transform.SetParent(near.transform, false);
            attKeeper.LostBall();
            near.PickBall(m_Ball.transform);
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
            Protected();
        }        
    }

    public void GOALLLL()
    {
        print("GOALLLL!!!");   
        m_strRetart = "GOALLLL!!!!";
        EndRound(false);
    }

    private void Protected()
    {
        print("Protected!!!");
        m_strRetart = "Protected!!!!";
        EndRound(true);
    }

    private void EndRound(bool isProtected)
    {
        if(isProtected)
            m_enemyWin++;
        else
            m_attackWin++;
        m_pause = true;
        m_timeRestart = 3.0f;
        m_WinText.gameObject.SetActive(true);

        m_enemyLabel.text = "Enemy (Defender) - " + m_enemyWin;
        m_playerLabel.text = "Player (Attacker) - " + m_attackWin;
    }
    

    private void restart()
    {
        foreach (var att in m_Attackers)
        {
            Destroy(att.gameObject, 0.1f);
        }
        foreach (var ene in m_Enemyers)
        {
            Destroy(ene.gameObject, 0.1f);
        }
        m_Attackers = new List<AttackerBehavior>();
        m_Enemyers = new List<EnemyBehavior>();

        if(m_Ball != null) Destroy(m_Ball, 0.1f);
         
        Vector3 Ballposition = new Vector3(Random.Range(-4.73f, 4.73f), 0.165f, Random.Range(-7.2f, 7.2f));
        m_Ball = Instantiate(m_BallPfab, Ballposition, Quaternion.identity);

        m_timeRemain = 140;
        m_EnemyEnergy.reset();
        m_AttackerEnergy.reset();
        m_WinText.gameObject.SetActive(false);
        m_pause = false;

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
