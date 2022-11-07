using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using System.Collections;
using UnityEngine;

namespace TeamOne.EvolvedSurvivor
{
    public class Enemy : MonoBehaviour
    {
        [Header("Enemy stats")]
        [SerializeField] protected float initialDamage = 10f;
        [SerializeField] protected float initialHealth = 30f;

        [Header("Enemy components")]
        [SerializeField] protected DamageHandler damageHandler;
        [SerializeField] protected DamageArea damageArea;
        [SerializeField] protected Health health;
        [SerializeField] protected CharacterMovement movement;

        [Header("Spawn manager")]
        [SerializeField] private SpawnManagerScriptableObject spawnManager;

        protected void OnEnable()
        {
            health.Revive();
            movement.ContextSpeedStack.Clear();
        }

        protected void OnDisable()
        {
            spawnManager.OnEnemyDespawn(this.gameObject);
        }

        public virtual void ScaleStats(float enemyLevel)
        {
            Damage damage = new Damage();
            damage.damage = this.initialDamage * 0.8f * (1f + enemyLevel);

            damage = this.damageHandler.ProcessOutgoingDamage(damage);
            this.damageArea.SetDamage(damage);
            this.damageArea.SetActive(true);

            this.health.MaximumHealth = this.initialHealth * (1f + enemyLevel);
            this.health.ResetHealthToMaxHealth();
        }
    }
}

