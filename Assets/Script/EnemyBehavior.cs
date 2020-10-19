using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnemyBehavior : MonoBehaviour
{
    ////////////////////
    // public Area

    ////////////////////
    // SerializeField Area
    [SerializeField] private GameObject m_Range, m_Indicator;

    ////////////////////
    // private Area
    private Gameplay m_GP;
    private float m_DeactiveTime;
    private bool m_isInit, m_isActive, m_isChasing, m_isReturning;
    private Animator m_animator;
    private Transform m_ball;
    private Vector3 m_initPos;


    ////////////////////
    // public Area

    // Start is called before the first frame update
    private void Awake()
    {
        m_DeactiveTime = 0.0f;
        m_isInit = false;
        m_isActive = false;
        m_isChasing = false;
        m_isReturning = false;
        m_initPos = new Vector3();
        m_animator = GetComponent<Animator>();
    }
    
    public void Init(Gameplay gp, float timeDeactive)
    {
        m_GP = gp;
        m_isActive = false;
        m_DeactiveTime = -timeDeactive;
        m_ball = m_GP.getBallTransform();
        m_initPos = transform.position;
        this.GetComponent<CapsuleCollider>().enabled = false;

        m_isInit = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_GP.PauseGame) return;
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
            if(m_ball == null) return;

            Vector3 ballPos = m_ball.position;
            ballPos.y = 0.5f;
            float dist = Vector3.Distance(ballPos, transform.position);
            //print("m_EnemyDetectRange: " + m_GP.GetEnemyDetectRange());
            if(dist < m_GP.GetEnemyDetectRange())
            {
                transform.LookAt(ballPos);
                m_isChasing = true;
            }
        }

        if(m_isReturning && transform.position.Equals(m_initPos))
        {
            m_isReturning = false;            
        }

        if(m_isActive && (m_isChasing || m_isReturning))
        {
            Move();
        }
        
        m_Range.SetActive(m_isActive && !m_isChasing && !m_isReturning);
        m_Indicator.SetActive(m_isActive && m_isChasing && m_isReturning);
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

        if(other.gameObject.CompareTag(GL.TAG_ATTACKER))
        {
           AttackerBehavior att = other.gameObject.GetComponent<AttackerBehavior>();
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
            if(m_ball == null) return;
            Vector3 ballPos = m_ball.position;
            ballPos.y = 0.5f;
            float speed = EneDef.NormalSpeed;
            transform.position = Vector3.MoveTowards(transform.position, ballPos, speed * Time.deltaTime);
        }
        if(m_isReturning)
        {            
            float speed = EneDef.ReturnSpeed;
            transform.position = Vector3.MoveTowards(transform.position, m_initPos, speed * Time.deltaTime);
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
        m_isChasing = false;
        m_DeactiveTime -= EneDef.ReactivateTime;
        this.GetComponent<CapsuleCollider>().enabled = false;
        m_animator.SetTrigger(GL.ANIM_INACTIVE);
        m_isReturning = true;
    }
    private void TouchBall()
    {
        
    }
}
