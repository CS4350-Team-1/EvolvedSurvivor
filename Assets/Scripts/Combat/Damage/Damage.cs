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
    }
}