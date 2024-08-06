using UnityEngine;

[CreateAssetMenu(menuName = "스킬/희귀/통키")]
public class TonkeyAbility : SyncAbilityBase
{
    // 다이아 1개 추가
    public override void CastAbility(CharacterBase characterBase)
    {
        instantAbilityEffect = PoolManager.instance.GetPool(PoolManager.instance.abilityEffectPool.queMap, abilityEffectType);
        instantAbilityEffect.transform.position = characterBase.enemyTrans.transform.position;
        
        CurrencyManager.instance.AcquireCurrency(1, false);

        PoolManager.instance.ReturnPool(PoolManager.instance.abilityEffectPool.queMap, instantAbilityEffect, abilityEffectType);
    }
}
