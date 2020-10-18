using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AttackerBehavior : MonoBehaviour
{
    ////////////////////
    // public Area


    ////////////////////
    // private Area
    private Gameplay m_GP;
    private float m_DeactiveTime;
    private bool m_isInit, m_hasBall, m_isActive;
    private Transform m_Target, m_ball;
    private Animator m_animator;

    /////// property
//     public string Name
//   {
//     get { return name; }   // get method
//     set { name = value; }  // set method
//   }
    public Transform Target
    {
        set { m_Target = value; }
    }    
    public bool PlayActive
    {
        get { return m_Target;}
        //set { m_Target = value; }
    }



    ////////////////////
    // public Area


    // Start is called before the first frame update
    private void Awake()
    {
        m_DeactiveTime = 0.0f;
        m_hasBall = false;
        m_isInit = false;
        m_isActive = false;
        m_animator = GetComponent<Animator>();
    }
    
    public void Init(Gameplay gp, Transform target, float timeDeactive)
    {
        m_GP = gp;
        this.m_Target = target;
        m_isActive = false;
        m_DeactiveTime = -timeDeactive;
        this.GetComponent<CapsuleCollider>().isTrigger = true;

        m_isInit = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!m_isInit || m_GP == null) return;
        if(m_GP.PauseGame) return;
        if(!m_isActive)
        {
            m_DeactiveTime += Time.deltaTime;
            if(m_DeactiveTime>=0)            
            {
                m_isActive = true;
                Active();
            }
        }
        if(m_isActive)
        {
            if(m_hasBall)
            {
                m_ball.position = new Vector3(transform.position.x, m_ball.position.y, transform.position.z + 0.5f);
            }
            Move();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag(GL.TAG_BALL))
        {
            //m_ball = other.gameObject.transform;
            PickBall(other.gameObject.transform);
        }

        if(other.gameObject.CompareTag(GL.TAG_GOAL))
        {
            if(m_hasBall)
            {
                m_GP.GOALLLL();
            }
            else
            {
                m_GP.DestroyAttacker(this);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // if (collision.gameObject.CompareTag("Ball"))
        // {
        //     m_ball = collision.gameObject.transform;
        //     PickBall();
        // }

        if(collision.gameObject.CompareTag("Wall"))
        {
           if(!m_hasBall)
           {
               m_GP.DestroyAttacker(this);
           }
        }

        if(collision.gameObject.CompareTag(GL.TAG_DEFENDER))
        {
           if(!m_hasBall)
           {             
           }
        }
    }
    
    public bool HasBall()
    {
        return m_hasBall;
    }

    ////////////////////
    // private Area
    private void Move()
    {
        Vector3 tarPos;
        float speed = AttDef.NormalSpeed;
        if(m_Target != null)
        {
            tarPos = m_Target.position;
            tarPos.y = 0.5f;
            //speed = AttDef.BallSpeed;
            if(m_hasBall)
            {
                speed = AttDef.CarryingSpeed;
                tarPos.z -= 0.5f;// cheat: goal so big :D                
            }
            transform.position = Vector3.MoveTowards(transform.position, tarPos, speed * Time.deltaTime);

        }
        else
        {
            tarPos = new Vector3(transform.position.x, 0.5f, 10.0f);
            transform.position = Vector3.MoveTowards(transform.position, tarPos, speed * Time.deltaTime);
        }
    }

    private void Active()
    {
        //this.GetComponent<CapsuleCollider>().enabled = true;
        m_animator.SetTrigger(GL.ANIM_ATTACKERS);
    }
    public void Inactivated()
    {
        m_hasBall = false;
        m_isActive = false;
        m_Target = null;
        m_DeactiveTime -= AttDef.ReactivateTime;
        this.GetComponent<CapsuleCollider>().enabled = false;
        m_animator.SetTrigger(GL.ANIM_INACTIVE);
    }

    public void PickBall(Transform ball)
    {
        if(m_hasBall)
        {
            print("PickBall() return by hasBall");
            return;
        }
        //if(m_GP.AttackersHasBall()) return;
        m_ball = ball;
        transform.LookAt(Vector3.forward);
        Vector3 goalPos = m_GP.m_Goal.transform.position;
        goalPos.y = 0.5f;
        transform.LookAt(goalPos);
        m_ball.SetParent(transform);
        m_ball.position = new Vector3(0.0f, m_ball.position.y, 0.9f);

        m_hasBall = true;
        m_GP.OnAttackersGetBall();
        this.GetComponent<CapsuleCollider>().isTrigger = false;
    }    
}
