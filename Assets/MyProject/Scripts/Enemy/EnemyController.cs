using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
//[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    #region 字段
    public float checkDistance;
    public float maxHeightDiff;//最大高度差
    [Range(0,180)]
    public float lookAngle; //视野范围
    public float followDistance;
    public float attackDistance;
    public float runSpeed = 6;
    public float walkSpeed = 3;
    protected float moveSpeed;
    protected bool isCanAttack = true;
    public float attackTime;//攻击时间间隔
    private float attackTimer;

    RaycastHit[] results = new RaycastHit[10];
    public LayerMask layerMask;
    public GameObject target;
    protected NavMeshAgent meshAgent;
    protected Vector3 startPosition;
    protected Animator animator;
    public RandomAudioPlayer runPlayer;
    #endregion

    #region unity生命周期
    protected virtual void Start()
    {
        meshAgent = transform.GetComponent<NavMeshAgent>();
        animator = transform.GetComponent<Animator>();
        startPosition = transform.position;
    }

    protected virtual void Update()
    {
        CheckTarget();
        FollowTarget();
        if (!isCanAttack)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer>=attackTime)
            {
                isCanAttack = true;
                attackTimer = 0;
            }
        }
    }

    protected virtual void OnDrawGizmosSelected()
    {
        //画出检测范围
        Gizmos.color = new Color(Color.blue.r, Color.blue.g, Color.blue.b, 0.4f);
        Gizmos.DrawSphere(transform.position, checkDistance);
        //画出追踪范围
        Gizmos.color = new Color(Color.green.r, Color.green.g, Color.green.b, 0.4f);
        Gizmos.DrawSphere(transform.position, followDistance);
        //画出攻击距离
        Gizmos.color = new Color(Color.red.r, Color.red.g, Color.red.b, 0.4f);
        Gizmos.DrawSphere(transform.position, attackDistance);
        //画出最大高度差
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * maxHeightDiff);
        Gizmos.DrawLine(transform.position, transform.position - Vector3.up * maxHeightDiff);
        //画出视野范围
        //Handles.color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.4f);
        //Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, lookAngle, checkDistance);
        //Handles.DrawSolidArc(transform.position, Vector3.down, transform.forward, lookAngle, checkDistance);
    }
    #endregion

    #region 方法
    public virtual void CheckTarget()
    {
        int count = Physics.SphereCastNonAlloc(transform.position, checkDistance, Vector3.forward, results, 0, layerMask.value);
        for (int i = 0; i < count; i++)
        {
            //判断是否为可攻击游戏物体
            if (results[i].transform.GetComponent<DamageAble>()==null)
            {
                continue;
            }
            //判断高度差
            if (Mathf.Abs(results[i].transform.position.y - transform.position.y) > maxHeightDiff) 
            {
                continue;
            }
            //判断是否在视野范围
            if (Vector3.Angle(transform.forward, results[i].transform.position - transform.position) > lookAngle) 
            {
                continue;
            }
            //判断目标是否还活着
            if (!results[i].transform.GetComponent<DamageAble>().IsAlive)
            {
                continue;
            }
            //找到最近的目标
            if (target != null) 
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);
                float currentDistance = Vector3.Distance(transform.position, results[i].transform.position);
                if (currentDistance < distance) 
                {
                    target = results[i].transform.gameObject;
                }
            }
            else
            {
                target = results[i].transform.gameObject;
            }
            runPlayer.PlayRandomAudio();
        }
    }

    public virtual void MoveToTarget()
    {
        if (target != null) 
        {
            meshAgent.SetDestination(target.transform.position);
        }
    }

    public virtual void FollowTarget()
    {
        //监听速度
        ListenSpeed();
        if (target!=null)
        {
            try
            {
                //向目标移动
                MoveToTarget();
                //判断是否可达到
                if (meshAgent.pathStatus == NavMeshPathStatus.PathPartial || meshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
                {
                    //目标丢失
                    LoseTarget();
                    return;
                }
                //是否在可追踪距离
                if (Vector3.Distance(transform.position, target.transform.position) > followDistance)
                {
                    //目标丢失
                    LoseTarget();
                    return;
                }
                //判断目标是否还活着
                if (!target.transform.GetComponent<DamageAble>().IsAlive)
                {
                    //目标丢失
                    LoseTarget();
                    return;
                }
                //是否在攻击范围
                if (Vector3.Distance(transform.position, target.transform.position) <= attackDistance)
                {
                    if (isCanAttack)
                    {
                        Attack();
                        isCanAttack = false;
                    }
                }
            }
            catch (System.Exception)
            {
                //目标丢失
                LoseTarget();
                throw;
            }
        }
    }

    public virtual void LoseTarget()
    {
        target = null;
        //回到初始位置
        meshAgent.SetDestination(startPosition);
        moveSpeed = walkSpeed;
    }

    public virtual void ListenSpeed()
    {
        if (target!=null)
        {
            moveSpeed = runSpeed;
        }
        meshAgent.speed = moveSpeed;
        animator.SetFloat("moveSpeed", meshAgent.velocity.magnitude);
    }

    public virtual void Attack()
    {

    }

    public virtual void OnDeath(DamageAble damageAble, DamageMessage data)
    {

    }
    #endregion
}
