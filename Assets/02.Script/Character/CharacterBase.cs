using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.Pool;

public enum DamageType  //데미지타입
{
    물리, 마법
}

public enum AttackType  //공격방식타입
{
    근거리, 원거리
}

public enum HeroGradeType  //히어로등급
{
    일반, 고급, 희귀, 전설, 신화 
}

public enum HeroDefaultSpriteDir //스프라이트기본 방향
{   //스프라이트렌더러 플립 처리를 위한 enum 클래스
    Left, Right
}
[Serializable]
public class HeroInfo  //히어로 정보 클래스
{
    [Header ("데미지 타입")] public DamageType damageType;
    [Header ("공격방식 타입")] public AttackType attackType;
    [Header ("유닛 등급")]public HeroGradeType heroGradeType;
    [Header ("유닛 타입")]public UnitType unitType;
    [Header ("유닛 스프라이트")] public Sprite unitSprite;
    [Header ("공격력")] public float attackDamage;
    [Header ("공격 속도")] public float attackSpeed;
    [Header ("초기 렌더러 방향")] public HeroDefaultSpriteDir heroDefaultSpriteDir;
}
public class CharacterBase : MonoBehaviour
{
    [Header ("평타 이펙트 타입")] public WeaponEffect weaponEffect;
    private SpriteRenderer spriteRenderer; // 렌더러
    private Animator anim; // 애니메이션
    [HideInInspector] public bool isOnTarget; // 타겟이 있는지 체크
    public HeroInfo heroInfo; // 유닛 정보
    private static readonly int IsAttacking = Animator.StringToHash("isAttack");
    [HideInInspector] public Transform gunPointTrans; // 원거리 발사 위치
    private float prevAtkSpeed = 0; // 공속 계산용
    [HideInInspector] public float limitAtkSpeed; // 공속
    [HideInInspector] public Transform enemyTrans; // 타겟 몬스터 트랜스폼
    
    // 초기화
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        if (heroInfo.attackType == AttackType.근거리) gunPointTrans = null;
    }
    private void Start()
    {
        //인스펙터 초기 설정 공격력, 공격속도로 초기화
        limitAtkSpeed = heroInfo.attackSpeed;
        prevAtkSpeed = limitAtkSpeed;
    }

    // 공속 계산
    void Update()
    {
        if (prevAtkSpeed < limitAtkSpeed)
        {
            prevAtkSpeed += Time.deltaTime;
        }
    }

    // 타겟 공격
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") && (!isOnTarget || enemyTrans.GetComponent<EnemyBase>().isDead))
        {
            enemyTrans = other.gameObject.transform;
            isOnTarget = true;
        }
        if(enemyTrans != null) CalculateSpriteRen(enemyTrans);
        
        //공격범위안에들어왔을 때 적이 머물고 있다면
        if(other.gameObject.CompareTag("Enemy") && prevAtkSpeed >= limitAtkSpeed)  //적(태그)이고, 타게팅중이 아니라면
        {
            prevAtkSpeed = 0f;
            
            //이펙트 생성.
            if (heroInfo.attackType == AttackType.근거리)
            {
                anim.SetBool(IsAttacking,true); //공격 애니메이션 활성화
                GameObject go = PoolManager.instance.GetPool(PoolManager.instance.weaponEffectPool.queMap, weaponEffect);
                go.transform.position = enemyTrans.position;
                go.GetComponent<MeleeWeapon>().weaponEffect = weaponEffect;
                go.GetComponent<MeleeWeapon>().attackDamage = heroInfo.attackDamage;
                //transform을 바탕으로 해당위치에 밀리웨폰생성하기
            }
            else   //원거리 처리
            {
                GameObject go = PoolManager.instance.GetPool(PoolManager.instance.weaponEffectPool.queMap, weaponEffect);
                go.GetComponent<RangeWeapon>().weaponEffect = weaponEffect;
                go.GetComponent<RangeWeapon>().attackDamage = heroInfo.attackDamage;
                SetLastBulletPos(go,enemyTrans,gunPointTrans);
            }
        }
    }

    // 타겟 나감
    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.transform == enemyTrans) isOnTarget = false;
        if (heroInfo.attackType == AttackType.근거리)
        {
            anim.SetBool(IsAttacking, false); //공격 애니메이션 비활성화
        }
    }

    // 원거리 발사 방향 셋
    private Quaternion CalculateBulletRotation(Transform enemyTrans, Transform startTrans)
    {
        //발사되는 총알의 방향 구현 로직
        Vector2 bulletDirection = (enemyTrans.position - startTrans.position); //방향계산
        float angle = Mathf.Atan2(bulletDirection.y, bulletDirection.x) * Mathf.Rad2Deg; //방향을 기반으로 각도 계산
        Quaternion bulletRotation = Quaternion.AngleAxis(angle, Vector3.forward); //쿼터니언 계산
        return bulletRotation;
    }
    public void SetLastBulletPos(GameObject bullet,Transform enemyTrans, Transform gunTrans) //최종 총알 발사 입구 설정
    {
        bullet.transform.SetPositionAndRotation(gunTrans.position,CalculateBulletRotation(enemyTrans, transform));
    }
    
    // 스프라이트 플립
    private void CalculateSpriteRen(Transform enemyTrans) //적을 바라보는 스프라이트렌더러 교체함수
    {  
        Vector2 heroDirection = enemyTrans.position - transform.position;
        if (heroDirection.x > 0)
        {   //오른쪽바라보게 렌더러 교체
            if (heroInfo.heroDefaultSpriteDir == HeroDefaultSpriteDir.Left)
            {
                spriteRenderer.flipX = true;
            }
            else if (heroInfo.heroDefaultSpriteDir == HeroDefaultSpriteDir.Right)
            {
                spriteRenderer.flipX = false;
            }
        }
        else //왼쪽 바라보게 렌더러 교체
        {
            if (heroInfo.heroDefaultSpriteDir == HeroDefaultSpriteDir.Right)
            {
                spriteRenderer.flipX = true;
            }
            else if (heroInfo.heroDefaultSpriteDir == HeroDefaultSpriteDir.Left)
            {
                spriteRenderer.flipX = false;
            }
        }
    }
}
