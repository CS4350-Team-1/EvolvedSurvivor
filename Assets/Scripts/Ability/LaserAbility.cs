using System.Collections;
using System.Collections.Generic;
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
            Damage damage = new Damage();
            damage.damage = this.damage.value;
            damage = damageHandler.ProcessOutgoingDamage(damage);

            laser.SetDamage(damage);

            Vector3 direction = (target.position - transform.position).normalized;
            laser.InitializeLaser(transform, direction, projectileSize.value);
            laser.SetLifeTime(duration.value);
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

        }

        protected override void HandleRecursive()
        {
            throw new System.NotImplementedException();
        }
    }
}
