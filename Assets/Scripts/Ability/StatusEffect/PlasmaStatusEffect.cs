using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace TeamOne.EvolvedSurvivor
{
    public class PlasmaStatusEffect : StatusEffect
    {
        [SerializeField]
        private float radius = 0.8f;
        private float damage;
        private float damageMultiplier;

        public PlasmaStatusEffect(float damage)
        {
            this.damage = damage;
        }

        public override void Build(int tier, float utilityRatio, float maxMagnitude)
        {
            
        }

        public override void Apply(GameObject enemy)
        {
            Collider2D[] enemiesInRadius = Physics2D.OverlapCircleAll(enemy.transform.position, radius, LayerMask.GetMask("Enemies"));
            if (enemiesInRadius.Length > 0)
            {
                float nearestDist = -1f;
                GameObject nearest = null;

                foreach (Collider2D currCollider in enemiesInRadius)
                {
                    if (currCollider.gameObject.tag == "Enemy")
                    {
                        Vector3 currDirection = currCollider.GetComponent<Transform>().position - enemy.transform.position;
                        float dist = currDirection.magnitude;
                        if (nearestDist == -1f || dist < nearestDist)
                        {
                            nearestDist = dist;
                            nearest = currCollider.gameObject;
                        }
                    }
                }
                nearest.GetComponent<DamageReceiver>().TakeDamage(new Damage(damage * damageMultiplier, gameObject));
            }
        }
    }
}