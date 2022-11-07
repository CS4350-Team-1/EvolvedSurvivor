using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamOne.EvolvedSurvivor
{
    public class ExplosiveProjectileAbility : Ability
    {
        [Header("Generated stats - set minValue and maxValue only")]
        [SerializeField]
        private AbilityStat<float> damage;
        [SerializeField]
        private AbilityStat<float> aoeRadius;
        [SerializeField]
        private AbilityStat<int> projectileNumber;
        [SerializeField]
        private AbilityStat<float> projectileSize;

        [Header("Fixed stats")]
        [SerializeField]
        private float projectileSpeed;
        [SerializeField]
        private float projectileSpawnInterval;

        [Header("The target detector for aiming")]
        [SerializeField]
        private TargetDetector targetDetector;

        protected override void Activate()
        {
            StartCoroutine(SpawnProjectiles(projectileNumber.value));
        }

        private void SpawnExplosiveProjectile(Transform target=null)
        {
            ExplosiveProjectile projectile = projectileObjectPool.GetPooledGameObject().GetComponent<ExplosiveProjectile>();
            projectile.transform.position = transform.position;
            projectile.SetActive(true);

            // Set damage
            Damage damage = new Damage();
            damage.damage = this.damage.value;
            damage.effects = effects;
            damage = damageHandler.ProcessOutgoingDamage(damage);

            projectile.SetDamage(damage);
            projectile.SetExplosionRadius(aoeRadius.value);
            projectile.SetSize(projectileSize.value);

            // Add recursive ability if it is recursive
            if (hasRecursive)
            {
                Ability recursiveAbility = recursiveAbilityObjectPool.GetPooledGameObject().GetComponent<Ability>();
                recursiveAbility.gameObject.SetActive(true);
                projectile.AddRecursiveAbility(recursiveAbility);
            }

            // Set motion
            Vector2 direction;
            if (target == null)
            {
                direction = Random.insideUnitCircle;
            }
            else
            {
                direction = (target.position - transform.position).normalized;
            }
            Vector2 motion = direction * projectileSpeed;
            projectile.SetMotion(motion);
            sfxHandler.PlaySfx();
        }

        private IEnumerator SpawnProjectiles(int projectileCount)
        {
            List<Transform> targets = targetDetector.ScanTargets();
            int targetIndex = 0;

            for (int i = 0; i < projectileCount; i++)
            {
                if (targets.Count == 0)
                {
                    SpawnExplosiveProjectile();
                }
                else
                {
                    SpawnExplosiveProjectile(targets[targetIndex]);
                    targetIndex++;
                    if (targetIndex >= targets.Count)
                    {
                        targetIndex = 0;
                    }
                }

                yield return new WaitForSeconds(projectileSpawnInterval);
            }
        }

        protected override void Build()
        {
            // Damage
            damage.value = (damage.maxValue - damage.minValue) * traitChart.DamageRatio + damage.minValue;

            // Uptime
            coolDown.value = coolDown.maxValue - (coolDown.maxValue - coolDown.minValue) * traitChart.UptimeRatio;

            // AOE
            aoeRadius.value = (aoeRadius.maxValue - aoeRadius.minValue) * traitChart.AoeRatio + aoeRadius.minValue;
            projectileSize.value = (projectileSize.maxValue - projectileSize.minValue) * traitChart.AoeRatio + projectileSize.minValue;

            // Quantity
            projectileNumber.value = Mathf.FloorToInt((projectileNumber.maxValue - projectileNumber.minValue) * traitChart.QuantityRatio + projectileNumber.minValue);

            // Utility
            foreach (KeyValuePair<ElementType, int> el in element.elements)
            {
                if (el.Value > 0)
                {
                    effects.Add(GenerateEffect(el.Key, traitChart.UtilityRatio, elementMagnitudes[(int)el.Key]));
                }
            }
        }

        protected override float DebuffTraitsForMerging(Ability other)
        {
            return 0f;
        }

        protected override TraitChart CreateTraitChartForMerging(float pointsToAssign, bool isSameType)
        {
            TraitChart newChart = new TraitChart(traitChart);
            float pointsPerTrait = pointsToAssign / 5f;
            newChart.CombineWith(new TraitChart(pointsPerTrait, pointsPerTrait, pointsPerTrait, pointsPerTrait, pointsPerTrait));
            return newChart;
        }

        protected override void HandleRecursive()
        {
            Activate();
            Deactivate();
        }

        public override string GetDetails()
        {
            return $"{damage.value:0.0} damage on hit\n"
                + $"Fires every {coolDown.value:0.0} seconds\n"
                + $"Targets up to {projectileNumber.value} enemies\n"
                + $"Projectile radius: {projectileSize.value:0.0} units\n"
                + $"Explosion radius: {aoeRadius.value:0.0} units\n"
                + "\n"
                + GetStatusEffects();
        }

        public override string GetComparedDetails(Ability other)
        {
            ExplosiveProjectileAbility o = (ExplosiveProjectileAbility)other;

            string details = "";
            details += GetComparedFloatString(o.damage.value, damage.value) + " damage on hit\n";
            details += "Fires every " + GetComparedFloatString(o.coolDown.value, coolDown.value) + " seconds\n";
            details += "Targets up to " + GetComparedIntString(o.projectileNumber.value, projectileNumber.value) + " enemies\n";
            details += "Projectile radius: " + GetComparedFloatString(o.projectileSize.value, projectileSize.value) + " units\n";
            details += "Explosion radius: " + GetComparedFloatString(o.aoeRadius.value, aoeRadius.value) + " units\n";
            details += "\n" + GetComparedStatusEffects(o);

            return details;
        }
    }
}
