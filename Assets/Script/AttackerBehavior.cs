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
    private bool m_isInit, m_hasBall, m_isActive, m_updateAddBall;
    private Transform m_Target, m_ball;
    private Animator m_animator;

    // property
//     public string Name
//   {
//     get { return name; }   // get method
//     set { name = value; }  // set method
//   }


    //
    // Summary:
    //     Target for Attack
    public Transform Target
    {

        set { m_Target = value; }
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
        m_updateAddBall = false;
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
                m_updateAddBall = false;
                m_ball.position = new Vector3(transform.position.x, m_ball.position.y, transform.position.z + 0.5f);
            }
            Move();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Ball"))
        {
            m_ball = other.gameObject.transform;
            PickBall();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            m_ball = collision.gameObject.transform;
            PickBall();
        }

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
            if(m_hasBall) speed = AttDef.CarryingSpeed;
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
        m_isActive = false;
        m_DeactiveTime -= AttDef.ReactivateTime;
        this.GetComponent<CapsuleCollider>().enabled = false;
        m_animator.SetTrigger(GL.ANIM_INACTIVE);
    }

    private void PickBall()
    {
        if(m_hasBall) return;
        if(m_GP.AttackersHasBall()) return;
        transform.LookAt(Vector3.forward);

        m_ball.LookAt(Vector3.forward);            
        m_ball.SetParent(transform);
        
        m_updateAddBall = true;
        transform.LookAt(m_GP.m_Goal.transform);

        m_hasBall = true;
        m_GP.OnAttackersGetBall();
        this.GetComponent<CapsuleCollider>().isTrigger = false;
    }    
}
