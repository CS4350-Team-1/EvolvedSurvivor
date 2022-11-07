using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace TeamOne.EvolvedSurvivor
{
    public class ZonalAbility : Ability
    {
        [Header("Generated stats - set minValue and maxValue only")]
        [SerializeField]
        private AbilityStat<float> damage;
        [SerializeField]
        private AbilityStat<float> duration;
        [SerializeField]
        private AbilityStat<float> aoeRadius;
        [SerializeField]
        private AbilityStat<int> targetNumber;
        [Header("Fixed stats")]
        [SerializeField]
        private float projectileSpawnInterval;

        [Header("The target detector for aiming")]
        [SerializeField]
        private TargetDetector targetDetector;

        protected override void Activate()
        {
            StartCoroutine(SpawnProjectiles(targetNumber.value));
        }

        private void SpawnZone(Transform target = null)
        {
            RecursableDamageArea damageArea = projectileObjectPool.GetPooledGameObject().GetComponent<RecursableDamageArea>();
            damageArea.transform.position = target.position;
            damageArea.SetActive(true);

            Damage damage = new Damage();
            damage.damage = this.damage.value;
            damage.effects = effects;
            damage = damageHandler.ProcessOutgoingDamage(damage);

            damageArea.SetDamage(damage);
            damageArea.SetSize(aoeRadius.value);

            damageArea.SetLifeTime(duration.value);

            if (hasRecursive)
            {
                Ability recursiveAbility = recursiveAbilityObjectPool.GetPooledGameObject().GetComponent<Ability>();
                recursiveAbility.gameObject.SetActive(true);
                damageArea.AddRecursiveAbility(recursiveAbility);
            }
        }

        private IEnumerator SpawnProjectiles(int projectileCount)
        {
            List<Transform> targets = targetDetector.ScanTargets();
            int targetIndex = 0;

            for (int i = 0; i < projectileCount; i++)
            {
                if (targets.Count > 0)
                {
                    SpawnZone(targets[targetIndex]);
                    targetIndex++;
                    if (targetIndex >= targets.Count)
                    {
                        targetIndex = 0;
                    }
                    sfxHandler.PlaySfx();
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
            duration.value = (duration.maxValue - duration.minValue) * traitChart.UptimeRatio + duration.minValue;

            // AOE
            aoeRadius.value = (aoeRadius.maxValue - aoeRadius.minValue) * traitChart.AoeRatio + aoeRadius.minValue;

            // Quantity
            targetNumber.value = Mathf.FloorToInt((targetNumber.maxValue - targetNumber.minValue) * traitChart.QuantityRatio + targetNumber.minValue);

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
            if (GetType() == other.GetType())
            {
                return 0f;
            }
            float points = other.traitChart.utility * debuffFactor;
            other.traitChart.utility -= points;
            return points;
        }

        protected override TraitChart CreateTraitChartForMerging(float pointsToAssign, bool isSameType)
        {
            float damageRatio = traitChart.damage;
            float uptimeRatio = traitChart.uptime;
            float aoeRatio = traitChart.aoe;
            float quantityRatio = traitChart.quantity;
            float utilityRatio = traitChart.utility;
            if (!isSameType)
            {
                utilityRatio = 0f;
            }
            pointsToAssign += traitChart.GetTotalPoints();
            float aoeBuff = pointsToAssign * buffFactor;
            pointsToAssign -= aoeBuff;
            float sum = damageRatio + uptimeRatio + aoeRatio + quantityRatio + utilityRatio;
            return new TraitChart(damageRatio / sum * pointsToAssign,
                uptimeRatio / sum * pointsToAssign,
                aoeRatio / sum * pointsToAssign + aoeBuff,
                quantityRatio / sum * pointsToAssign,
                utilityRatio / sum * pointsToAssign);
        }

        protected override void HandleRecursive()
        {
            Activate();
            Deactivate();
        }

        public override string GetDetails()
        {
            return $"{damage.value:0.0} damage every 0.5 seconds\n"
                + $"Fires every {coolDown.value:0.0} seconds\n"
                + $"Targets up to {targetNumber.value} enemies\n" 
                + $"Lasts {duration.value:0.0} seconds\n"
                + $"Pool radius: {aoeRadius.value:0.0} units\n"
                + "\n"
                + GetStatusEffects();
        }

        public override string GetComparedDetails(Ability other)
        {
            ZonalAbility o = (ZonalAbility)other;

            string details = "";
            details += GetComparedFloatString(o.damage.value, damage.value) + " damage every 0.5 seconds\n";
            details += "Fires every " + GetComparedFloatString(o.coolDown.value, coolDown.value) + " seconds\n";
            details += "Targets up to " + GetComparedIntString(o.targetNumber.value, targetNumber.value) + " enemies\n";
            details += "Lasts " + GetComparedFloatString(o.duration.value, duration.value) + " seconds\n";
            details += "Pool radius: " + GetComparedFloatString(o.aoeRadius.value, aoeRadius.value) + " units\n";
            details += "\n" + GetComparedStatusEffects(o);

            return details;
        }
    }
}
