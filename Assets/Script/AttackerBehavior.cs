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
    private float _StartTime, _speed;
    private bool _isInit, _hasBall;
    private Transform Target;



    ////////////////////
    // public Area
    ////////////////////

    // Start is called before the first frame update
    private void Awake()
    {
        _StartTime = _speed = 0;
        _hasBall = false;
        _isInit = false;
        
        _speed = AttDef.NormalSpeed;
    }
    
    public void Init(Gameplay gp, Transform target)
    {
        GP = gp;
        this.Target = target;
        _isInit = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!_isInit || GP == null) return;

        
        Move();
        _StartTime += Time.deltaTime;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Ball"))
        {
            GameObject ball = other.gameObject;
            ball.transform.LookAt(GP.Goal.transform);
            transform.LookAt(GP.Goal.transform);
            _hasBall = true;
            GP.OnAttackersGetBall();
        }
    }
    
    public bool HasBall()
    {
        return _hasBall;
    }

    public void SetTarget(Transform target)
    {
        Target = target;
    }


    ////////////////////
    // private Area
    ////////////////////
    private void Move()
    {
        Vector3 tarPos;
        if(Target != null)
        {
            tarPos = Target.position;
            transform.position = Vector3.MoveTowards(transform.position, tarPos, _speed * Time.deltaTime);

        }
        else
        {
            tarPos = new Vector3(transform.position.x, 0.0f, 10.0f);
            transform.position = Vector3.MoveTowards(transform.position, tarPos, _speed * Time.deltaTime);
        }
    }
}
