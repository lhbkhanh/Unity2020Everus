using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AttackerBehavior : MonoBehaviour
{
    ////////////////////
    // public Area
    ////////////////////


    ////////////////////
    // private Area
    ////////////////////
    private Gameplay GP;
    private float _DeactiveTime;
    private bool _isInit, _hasBall, _isActive, _updateAddBall;
    private Transform trfTarget;
    private Animator _animator;
    private GameObject ball;



    ////////////////////
    // public Area
    ////////////////////

    // Start is called before the first frame update
    private void Awake()
    {
        _DeactiveTime = 0.0f;
        _hasBall = false;
        _isInit = false;
        _isActive = false;
        _updateAddBall = false;
        _animator = GetComponent<Animator>();
    }
    
    public void Init(Gameplay gp, Transform target, float timeDeactive)
    {
        GP = gp;
        this.trfTarget = target;
        _isActive = false;
        _DeactiveTime = -timeDeactive;
        this.GetComponent<CapsuleCollider>().enabled = false;

        _isInit = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!_isInit || GP == null) return;
        if(!_isActive)
        {
            _DeactiveTime += Time.deltaTime;
            if(_DeactiveTime>=0)            
            {
                _isActive = true;
                Active();
            }
        }
        if(_isActive)
        {
            if(_hasBall)
            {
                _updateAddBall = false;
                ball.transform.position = new Vector3(transform.position.x, ball.transform.position.y, transform.position.z + 0.5f);
            }
            Move();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Ball"))
        {
            ball = other.gameObject;
            PickBall();
        }
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            ball = collision.gameObject;
            PickBall();
        }

        if(collision.gameObject.CompareTag("Wall"))
        {
           if(!_hasBall)
           {
               GP.DestroyAttacker(this);
           }
        }
    }
    
    public bool HasBall()
    {
        return _hasBall;
    }

    public void SetTarget(Transform target)
    {
        trfTarget = target;
    }


    ////////////////////
    // private Area
    ////////////////////
    private void Move()
    {
        Vector3 tarPos;
        float speed = AttDef.NormalSpeed;
        if(trfTarget != null)
        {
            tarPos = trfTarget.position;
            tarPos.y = 0.5f;
            if(_hasBall) speed = AttDef.BallSpeed;
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
        this.GetComponent<CapsuleCollider>().enabled = true;
        _animator.SetTrigger("ToAttackers");

    }

    void PickBall()
    {
        if(_hasBall) return;
        
        transform.LookAt(Vector3.forward);

        ball.transform.LookAt(Vector3.forward);            
        ball.transform.SetParent(transform);
        
        _updateAddBall = true;
        transform.LookAt(GP.Goal.transform);

        _hasBall = true;
        GP.OnAttackersGetBall();
    }
}
