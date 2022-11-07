using UnityEngine;

namespace TeamOne.EvolvedSurvivor
{
    // This class is used for enemies which have a secondary attack e.g., ranged enemies.
    public class SecondaryAttackEnemy : Enemy
    {
        [SerializeField] private float initialSecondaryDamage = 20f;
        [SerializeField] private DamageArea secDamageArea;

        private float currentSecondaryDamage;

        public float SecondaryDamage => currentSecondaryDamage;

        public override void ScaleStats(float enemyLevel)
        {
            base.ScaleStats(enemyLevel);

            this.currentSecondaryDamage = initialSecondaryDamage * 0.6f * (1f + enemyLevel);

            if (this.secDamageArea != null)
            {
                Damage damage = new Damage();
                damage.damage = this.currentSecondaryDamage;

                damage = this.damageHandler.ProcessOutgoingDamage(damage);
                this.secDamageArea.SetDamage(damage);
            }
        }
    }
}
