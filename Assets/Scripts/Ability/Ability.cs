using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;

namespace TeamOne.EvolvedSurvivor
{
    public abstract class Ability : MonoBehaviour
    {
        [Header("Whether this ability will be activated while it is active")]
        [SerializeField]
        private bool activateOnlyOnce = false;
        [Header("The ability is activated once every (coolDown) seconds")]
        [SerializeField]
        protected float coolDown;
        protected int tier;
        protected TraitChart traitChart;

        private bool hasBuilt = false;
        private bool hasActivated;
        private float coolDownTimer;

        protected GameObject playerRef;

        /// <summary>
        /// Uses the trait chart to define the behaviours of the ability. 
        /// E.g., Speed, Damage, CoolDown, etc.
        /// MUST be called before the ability can be activated
        /// </summary>
        public void BuildAbility(TraitChart traitChart)
        {
            Build(traitChart);
            hasBuilt = true;
        }

        /// <summary>
        /// Updates the ability using the consumed ability.
        /// </summary>
        public abstract void UpgradeAbility(Ability consumedAbility);

        private void Update()
        {
            if (!hasBuilt)
            {
                return;
            }

            if (activateOnlyOnce)
            {
                if (!hasActivated)
                {
                    hasActivated = Activate();
                }
                return;
            }

            coolDownTimer -= Time.deltaTime;
            if (coolDownTimer < 0f)
            {
                Activate();
                coolDownTimer = coolDown;
            }
        }

        public void addPlayerRef(GameObject player)
        {
            playerRef = player;
        }

        protected abstract void Build(TraitChart traitChart);

        protected abstract bool Activate();

        
    }
}
