using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyBehavior : MonoBehaviour
{
    ////////////////////
    // public Area


    ////////////////////
    // private Area
    private Gameplay m_GP;
    private float m_DeactiveTime;
    private bool m_isInit, m_isActive, m_isChasing;
    private Animator m_animator;
    private Transform m_ball;


    ////////////////////
    // public Area

    // Start is called before the first frame update
    private void Awake()
    {
        m_DeactiveTime = 0.0f;
        m_isInit = false;
        m_isActive = false;
        m_isChasing = false;
        m_animator = GetComponent<Animator>();
    }
    
    public void Init(Gameplay gp, float timeDeactive)
    {
        m_GP = gp;
        m_isActive = false;
        m_DeactiveTime = -timeDeactive;
        m_ball = m_GP.getBallTransform();
        this.GetComponent<CapsuleCollider>().enabled = false;

        m_isInit = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!m_isInit || m_GP == null) return;
        if(!m_isActive)
        {
            m_DeactiveTime += Time.deltaTime;
            if(m_DeactiveTime>=0)            
            {
                m_isActive = true;
                Active();
            }
        }


        if(!m_isChasing)
        {
            if(!m_GP.AttackersHasBall()) return;

            Vector3 ballPos = m_ball.position;
            ballPos.y = 0.5f;
            float dist = Vector3.Distance(ballPos, transform.position);
            if(dist < m_GP.GetEnemyDetectRange())
            {
                transform.LookAt(ballPos);
                m_isChasing = true;
            }
        }

        if(m_isActive && m_isChasing)
        {
            Move();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag(GL.TAG_BALL))
        {
            AttackerBehavior att = other.gameObject.GetComponentInParent<AttackerBehavior>();
           if(att.HasBall())
           {
               m_GP.Arrested(this, att);
           }
        }
      
        
    }

    void OnCollisionEnter(Collision collision)
    {
        // if(collision.gameObject.CompareTag(GL.TAG_BALL))
        // {
        //     AttackerBehavior att = collision.gameObject.GetComponentInParent<AttackerBehavior>();
        //    if(att.HasBall())
        //    {
        //        m_GP.Arrested(this, att);
        //    }
        // }
    }
    
    public bool HasBall()
    {
        // return m_hasBall;
        return false;
    }

    ////////////////////
    // private Area
    private void Move()
    {        
        if(m_isChasing)
        {
            Vector3 ballPos = m_ball.position;
            ballPos.y = 0.5f;
            float speed = EneDef.NormalSpeed;
            transform.position = Vector3.MoveTowards(transform.position, ballPos, speed * Time.deltaTime);
        }
        
    }

    private void Active()
    {
        this.GetComponent<CapsuleCollider>().enabled = true;
        m_animator.SetTrigger(GL.ANIM_ENEMY);
    }
    public void Inactivated()
    {
        m_isActive = false;
        m_DeactiveTime -= EneDef.ReactivateTime;
        this.GetComponent<CapsuleCollider>().enabled = false;
        m_animator.SetTrigger(GL.ANIM_INACTIVE);
    }
    private void TouchBall()
    {
        
    }
}
