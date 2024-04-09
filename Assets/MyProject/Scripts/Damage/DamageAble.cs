using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageMessage
{
    public int damage;
    public Vector3 damagePosition;
}
[System.Serializable]
public class DamageEvent : UnityEvent<DamageAble, DamageMessage> { }

public class DamageAble : MonoBehaviour
{
    #region 字段
    public int hp;
    public int maxHp;
    public float invincibleTime = 0;
    private float invincibleTimer = 0;
    private bool isInvincible = false;

    public DamageEvent onHurt;
    public DamageEvent onDeath;
    public DamageEvent onReset;

    public int CurrentHp
    {
        get
        {
            return hp;
        }
    }
    public bool IsAlive
    {
        get
        {
            return CurrentHp > 0;
        }
    }
    #endregion

    #region unity生命周期
    private void Start()
    {
        hp = maxHp;
    }

    private void Update()
    {
        if (isInvincible)
        {
            invincibleTimer += Time.deltaTime;
            if (invincibleTimer >= invincibleTime)
            {
                isInvincible = false;
                invincibleTimer = 0;
            }
        }
    }
    #endregion

    #region 方法
    public void OnDamage(DamageMessage data)
    {
        if (hp <= 0) 
        {
            return;
        }
        if (isInvincible)
        {
            return;
        }
        hp -= data.damage;
        isInvincible = true;
        if (hp <= 0) 
        {
            onDeath?.Invoke(this, data);
        }
        else
        {
            onHurt?.Invoke(this, data);
        }
    }

    public void ResetDamage()
    {
        hp = maxHp;
        isInvincible = false;
        invincibleTimer = 0;
        onReset?.Invoke(this, null);
    }

    public void SetInvincibleTrue()
    {
        isInvincible = true;
    }

    public void SetInvincibleFalse()
    {
        isInvincible = false;
    }
    #endregion
}
