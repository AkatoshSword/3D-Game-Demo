using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region 字段
    public float moveSpeed = 0;
    public float maxMoveSpeed = 8;
    public float jumpSpeed = 10;
    public float gravity = 20f;
    public bool isGrounded = true;
    public float verticalSpeed = 0;
    public float minAngleSpeed = 400;
    public float maxAngleSpeed = 1200;
    public float accelerateSpeed = 5;
    public bool isCanAttack = false;

    public GameObject weapon;
    public CharacterController characterController;
    private PlayerInput playerInput;
    public Vector3 move;
    public Transform renderCamera;
    private Animator animator;
    private WeaponAttackController attackController;
    private AnimatorStateInfo currentStateInfo;
    public Vector3 rebornPosition;
    public RandomAudioPlayer jumpPlayer;
    public RandomAudioPlayer runPlayer;
    public RandomAudioPlayer attackPlayer;
    public RandomAudioPlayer hurtPlayer;
    public RandomAudioPlayer deathPlayer;
    public RandomAudioPlayer rollPlayer;
    public RandomAudioPlayer getWeaponPlayer;
    private ChekGroundMaterial groundMaterial;

    private int DeathHash = Animator.StringToHash("Death");
    #endregion

    #region unity生命周期
    private void Awake()
    {
        characterController = transform.GetComponent<CharacterController>();
        playerInput = transform.GetComponent<PlayerInput>();
        animator = transform.GetComponent<Animator>();
        groundMaterial = transform.GetComponent<ChekGroundMaterial>();
        attackController = weapon.GetComponent<WeaponAttackController>();
    }

    private void Update()
    {
        currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        CaculateMove();
        CaculateVerticalSpeed();
        CaculateForwardSpeed();
        animator.SetFloat("normalizedTime", Mathf.Repeat(currentStateInfo.normalizedTime, 1));
        animator.ResetTrigger("lightAttack");
        if (playerInput.LightAttack && isCanAttack) 
        {
            animator.SetTrigger("lightAttack");
        }
        animator.ResetTrigger("heavyAttack");
        if (playerInput.HeavyAttack && isCanAttack)
        {
            animator.SetTrigger("heavyAttack");
        }
        animator.ResetTrigger("roll");
        if (playerInput.Roll)
        {
            animator.SetTrigger("roll");
        }
    }
    #endregion

    #region 方法
    private void CaculateMove()
    {
        move.Set(playerInput.Move.x, 0, playerInput.Move.y);
        move *= moveSpeed * Time.deltaTime;
        move = renderCamera.TransformDirection(move);
        if (move.x != 0 || move.z != 0) 
        {
            move.y = 0;
            float turnSpeed = Mathf.Lerp(maxAngleSpeed, minAngleSpeed, moveSpeed / maxAngleSpeed) * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(move), turnSpeed);
        }
        move += Vector3.up * verticalSpeed * Time.deltaTime;
        characterController.Move(move);
        isGrounded = characterController.isGrounded;
        animator.SetBool("isGrounded", isGrounded);
    }

    private void CaculateVerticalSpeed()
    {
        if (isGrounded)
        {
            verticalSpeed = -gravity * 0.3f;
            if (playerInput.Jump)
            {
                verticalSpeed = jumpSpeed;
                isGrounded = false;
                jumpPlayer.PlayRandomAudio();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space) && verticalSpeed > 0) 
            {
                verticalSpeed -= gravity * Time.deltaTime;
            }
            verticalSpeed -= gravity * Time.deltaTime;
        }
        animator.SetFloat("verticalSpeed", verticalSpeed);
    }

    private void CaculateForwardSpeed()
    {
        moveSpeed = Mathf.MoveTowards(moveSpeed, maxMoveSpeed * playerInput.Move.normalized.magnitude, accelerateSpeed * Time.deltaTime);
        animator.SetFloat("moveSpeed", moveSpeed);
    }

    public void SetCanAttack(bool isAttack)
    {
        isCanAttack = isAttack;
        getWeaponPlayer.PlayRandomAudio();
    }

    public void ShowWeapon()
    {
        CancelInvoke("HideWeaponExcute");
        weapon.SetActive(true);
    }

    public void HideWeapon()
    {
        Invoke("HideWeaponExcute", 1);
    }

    public void HideWeaponExcute()
    {
        weapon.SetActive(false);
    }

    public void OnHurt(DamageAble dmageAble, DamageMessage data)
    {     
        Vector3 direction = data.damagePosition - transform.position;
        direction.y = 0;
        Vector3 localDirection = transform.InverseTransformDirection(direction);
        animator.SetFloat("hurtX", localDirection.x);
        animator.SetFloat("hurtY", localDirection.z);
        animator.SetTrigger("hurt");
        hurtPlayer.PlayRandomAudio();
    }

    public void OnDeath(DamageAble dmageAble, DamageMessage data)
    {
        animator.SetTrigger("death");
        StartCoroutine(IEReborn());
    }

    //重生
    public IEnumerator IEReborn()
    {
        //判断是否正在播放死亡动画
        while (currentStateInfo.shortNameHash != DeathHash) 
        {
            yield return null;
        }
        yield return null;
        //屏幕变黑
        yield return StartCoroutine(BlackMaskView.Instance.FadeOut());
        //重置玩家位置
        transform.position = rebornPosition;
        //播放重生动画
        animator.SetTrigger("reborn");
        yield return new WaitForSeconds(1f);
        //屏幕变亮
        yield return StartCoroutine(BlackMaskView.Instance.FadeIn());
        //重置血量
        transform.GetComponent<DamageAble>().ResetDamage();
        //给予控制权
        playerInput.GainControl();

    }

    public void PlayDeathSound()
    {
        deathPlayer.PlayRandomAudio(); ;
    }

    public void SetRebornPosition(Vector3 position)
    {
        rebornPosition = position;
    }
    #endregion

    #region 动画事件
    public void OnIdleStart()
    {
        animator.SetInteger("randomIdle", -1);
    }

    public void OnIdleEnd()
    {
        animator.SetInteger("randomIdle", Random.Range(-3, 4));
    }

    public void LightAttackStart()
    {
        attackController.BeginAttack();
        attackPlayer.PlayRandomAudio();
    }

    public void LightAttackEnd()
    {
        attackController.EndAttack();
    }

    public void HeavyAttackStart()
    {
        attackController.BeginAttack();
        attackController.damage += 2;
        attackPlayer.PlayRandomAudio();
    }

    public void HeavyAttackEnd()
    {
        attackController.EndAttack();
        attackController.damage -= 2;
    }

    public void FootL()
    {
        runPlayer.PlayRandomSound(groundMaterial.currentMaterial);
    }

    public void FootR()
    {
        runPlayer.PlayRandomSound(groundMaterial.currentMaterial);
    }

    public void RollStart()
    {
        transform.GetComponent<DamageAble>().SetInvincibleTrue();
        rollPlayer.PlayRandomAudio();
    }

    public  void RollEnd()
    {
        transform.GetComponent<DamageAble>().SetInvincibleFalse();
    }
    #endregion
}
