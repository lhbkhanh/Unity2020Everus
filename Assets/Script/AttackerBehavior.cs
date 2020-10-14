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
    private float _DeactiveTime, _speed ;
    private bool _isInit, _hasBall, _isActive;
    private Transform tfmTarget;
    private Animator _animator;



    ////////////////////
    // public Area
    ////////////////////

    // Start is called before the first frame update
    private void Awake()
    {
        _DeactiveTime = _speed = 0.0f;
        _hasBall = false;
        _isInit = false;
        _isActive = false;
        _animator = GetComponent<Animator>();
        
        _speed = AttDef.NormalSpeed; // debug
    }
    
    public void Init(Gameplay gp, Transform target, float timeDeactive)
    {
        GP = gp;
        this.tfmTarget = target;
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
            Move();
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Ball"))
        {
            PickBall(other.gameObject);
        }
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            PickBall(collision.gameObject);
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
        tfmTarget = target;
    }


    ////////////////////
    // private Area
    ////////////////////
    private void Move()
    {
        Vector3 tarPos;
        if(tfmTarget != null)
        {
            tarPos = tfmTarget.position;
            tarPos.y = 0.5f;
            transform.position = Vector3.MoveTowards(transform.position, tarPos, _speed * Time.deltaTime);

        }
        else
        {
            tarPos = new Vector3(transform.position.x, 0.5f, 10.0f);
            transform.position = Vector3.MoveTowards(transform.position, tarPos, _speed * Time.deltaTime);
        }
    }

    private void Active()
    {
        this.GetComponent<CapsuleCollider>().enabled = true;
        _animator.SetTrigger("ToAttackers");

    }

    void PickBall(GameObject ball)
    {
        if(_hasBall) return;
        
        transform.LookAt(Vector3.forward);

        ball.transform.LookAt(Vector3.forward);            
        ball.transform.SetParent(transform);
        //ball.transform.position = new Vector3(0, ball.transform.position.y, transform.position.z + 0.5f);
        ball.transform.position = new Vector3(0.0f, 0.0f, 0.8f);
        transform.LookAt(GP.Goal.transform);

        _hasBall = true;
        GP.OnAttackersGetBall();
    }
}
