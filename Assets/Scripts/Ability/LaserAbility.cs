using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace TeamOne.EvolvedSurvivor
{
    public class LaserAbility : Ability
    {
        [SerializeField]
        private AbilityStat<float> damage;
        [SerializeField]
        private AbilityStat<float> duration;
        [SerializeField]
        private AbilityStat<int> laserNumber;
        [SerializeField]
        private AbilityStat<float> projectileSize;

        [Header("Fixed stats")]
        [SerializeField]
        private float laserSpawnInterval;

        [Header("The target detector for aiming")]
        [SerializeField]
        private TargetDetector targetDetector;

        protected override void Activate()
        {
            StartCoroutine(SpawnLasers(laserNumber.value));
        }

        private void SpawnLaser(Transform target = null)
        {
            if (target == null)
            {
                return;
            }
            Laser laser = projectileObjectPool.GetPooledGameObject().GetComponent<Laser>();
            laser.SetActive(true);

            // Set damage
            Damage damage = new Damage(this.damage.value, gameObject, effects);
            damage = damageHandler.ProcessOutgoingDamage(damage);

            laser.SetDamage(damage);

            Vector3 direction = (target.position - transform.position).normalized;
            laser.InitializeLaser(transform, direction, projectileSize.value);
            laser.SetLifeTime(duration.value);
            sfxHandler.PlaySfx();

            if (hasRecursive)
            {
                Ability recursiveAbility = recursiveAbilityObjectPool.GetPooledGameObject().GetComponent<Ability>();
                recursiveAbility.gameObject.SetActive(true);
                laser.AddRecursiveAbility(recursiveAbility);
            }
        }

        private IEnumerator SpawnLasers(int laserCount)
        {
            List<Transform> targets = targetDetector.ScanTargets();
            int targetIndex = 0;

            for (int i = 0; i < laserCount; i++)
            {
                if (targets.Count == 0)
                {
                    SpawnLaser();
                }
                else
                {
                    SpawnLaser(targets[targetIndex]);
                    targetIndex++;
                    if (targetIndex >= targets.Count)
                    {
                        targetIndex = 0;
                    }
                }

                yield return new WaitForSeconds(laserSpawnInterval);
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
            projectileSize.value = (projectileSize.maxValue - projectileSize.minValue) * traitChart.AoeRatio + projectileSize.minValue;

            // Quantity
            laserNumber.value = Mathf.FloorToInt((laserNumber.maxValue - laserNumber.minValue) * traitChart.QuantityRatio + laserNumber.minValue);

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
            float points = other.traitChart.uptime * debuffFactor;
            other.traitChart.uptime -= points;
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
                uptimeRatio = 0f;
            }
            pointsToAssign += traitChart.GetTotalPoints();
            float quantityBuff = pointsToAssign * buffFactor;
            pointsToAssign -= quantityBuff;
            float sum = damageRatio + uptimeRatio + aoeRatio + quantityRatio + utilityRatio;
            return new TraitChart(damageRatio / sum * pointsToAssign,
                uptimeRatio / sum * pointsToAssign,
                aoeRatio / sum * pointsToAssign,
                quantityRatio / sum * pointsToAssign + quantityBuff,
                utilityRatio / sum * pointsToAssign);
        }

        protected override void HandleRecursive()
        {
            if (!hasActivated)
            {
                Activate();
                hasActivated = true;
                Invoke("Deactivate", duration.value);
            }
        }

        public override string GetDetails()
        {
            return $"{damage.value:0.0} damage every 0.5 seconds\n"
                + $"Fires every {coolDown.value:0.0} seconds\n"
                + $"Targets up to {laserNumber.value} enemies\n"
                + $"Lasts {duration.value:0.0} seconds\n"
                + $"Laser thickness: {projectileSize.value:0.0} units\n"
                + "\n"
                + GetStatusEffects();
        }

        public override string GetComparedDetails(Ability other)
        {
            LaserAbility o = (LaserAbility)other;

            string details = "";
            details += GetComparedFloatString(o.damage.value, damage.value) + " damage every 0.5 seconds\n";
            details += "Fires every " + GetComparedFloatString(o.coolDown.value, coolDown.value) + " seconds\n";
            details += "Targets up to " + GetComparedIntString(o.laserNumber.value, laserNumber.value) + " enemies\n";
            details += "Lasts " + GetComparedFloatString(o.duration.value, duration.value) + " seconds\n";
            details += "Laser thickness: " + GetComparedFloatString(o.projectileSize.value, projectileSize.value) + " units\n";
            details += "\n" + GetComparedStatusEffects(o);

            return details;
        }
    }
}
