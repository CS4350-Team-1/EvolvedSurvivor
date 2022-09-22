using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamOne.EvolvedSurvivor
{
    [System.Serializable]
    public class Damage
    {
        public float damage;
        public GameObject instigator;
        public Vector3 direction;

        public Damage(float damage, GameObject instigator, Vector3 direction)
        {
            this.damage = damage;
            this.instigator = instigator;
            this.direction = direction;
        }
    }
}
