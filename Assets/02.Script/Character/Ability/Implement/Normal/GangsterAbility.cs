using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "스킬/일반/갱스터")]
public class GangsterAbility : SyncAbilityBase
{
    // 150% 데미지, 0.5초 스턴
    public override void CastAbility(CharacterBase characterBase)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(characterBase.enemyTrans.transform.position, 1f);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                EnemyBase enemyBase = hit.GetComponent<EnemyBase>();
                enemyBase.SetStunTime = 0.5f;
                enemyBase.TakeDamage(characterBase.GetApplyAttackDamage(characterBase.heroInfo.attackDamage) * 1.5f, DamageType.물리);
            }
        }
    }
}
