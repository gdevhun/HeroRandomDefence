using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "스킬/신화/유미")]
public class YumieAbility : AsyncAbilityBase, IHiddenAbility
{
    // 이동속도 30 감소 슬로우 장판 설치(3초 유지)
    public override IEnumerator CastAbility(CharacterBase characterBase)
    {
        instantAbilityEffect = PoolManager.instance.GetPool(PoolManager.instance.abilityEffectPool.queMap, abilityEffectType);
        instantAbilityEffect.transform.position = characterBase.enemyTrans.transform.position;
        
        Collider2D[] hits = Physics2D.OverlapCircleAll(instantAbilityEffect.transform.position, 1f);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                EnemyBase enemyBase = hit.GetComponent<EnemyBase>();
                enemyBase.moveSpeed -= 0.3f;
            }
        }
        yield return oneSecond;
        yield return oneSecond;
        yield return oneSecond;
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                hit.GetComponent<EnemyBase>().moveSpeed = hit.GetComponent<EnemyBase>().originMoveSpeed;
            }
        }

        PoolManager.instance.ReturnPool(PoolManager.instance.abilityEffectPool.queMap, instantAbilityEffect, abilityEffectType);
    }

    // 히든 스킬
    // 전설 유닛 5개 모으면 자동 소환
    [Header ("히든 스킬 UI 정보")] [SerializeField] private AbilityUiInfo hiddenAbilityUiInfo;
    public AbilityUiInfo HiddenAbilityUiInfo
    {
        get { return hiddenAbilityUiInfo; }
        set { hiddenAbilityUiInfo = value; }
    }
    public void CastHiddenAbility(CharacterBase characterBase)
    {
        
    }
}
