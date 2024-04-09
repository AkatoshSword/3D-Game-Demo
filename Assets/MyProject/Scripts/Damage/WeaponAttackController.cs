using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CheckPoint
{
    public Transform point;
    public float radius;
}

public class WeaponAttackController : MonoBehaviour
{
    #region 字段
    public CheckPoint[] checkPoints;
    public Color color;
    private RaycastHit[] results = new RaycastHit[10];
    public LayerMask layerMask;
    private bool attack = false;
    public int damage;
    public GameObject mySelf;
    private List<GameObject> attackList = new List<GameObject>();
    #endregion

    #region unity生命周期
    void Update()
    {
        CheckGameObject();
    }
    #endregion

    #region 方法
    public void BeginAttack()
    {
        attack = true;
    }

    public void EndAttack()
    {
        attack = false;
        attackList.Clear();
    }

    public void CheckGameObject()
    {
        if (!attack)
        {
            return;
        }
        for (int i = 0; i < checkPoints.Length; i++)
        {
            int count = Physics.SphereCastNonAlloc(checkPoints[i].point.position, checkPoints[i].radius, Vector3.forward, results, 0, layerMask.value);
            for (int j = 0; j < count; j++)
            {
                CheckDamage(results[j].transform.gameObject);
            }
        }
    }

    public void CheckDamage(GameObject obj)
    {
        //判断游戏物体是否可以受伤
        DamageAble damageAble = obj.GetComponent<DamageAble>();
        if (damageAble == null) 
        {
            return;
        }
        //判断是否是自己
        if (obj==mySelf)
        {
            return;
        }
        if (attackList.Contains(obj))
        {
            return;
        }
        //进行攻击
        DamageMessage data = new DamageMessage();
        data.damage = damage;
        data.damagePosition = mySelf.transform.position;
        damageAble.OnDamage(data);
        attackList.Add(obj);
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < checkPoints.Length; i++)
        {
            Gizmos.color = color;
            Gizmos.DrawSphere(checkPoints[i].point.position, checkPoints[i].radius);
        }
    }
    #endregion
}
