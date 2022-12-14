using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace TeamOne.EvolvedSurvivor
{
    public abstract class Ability : MonoBehaviour, Upgradable
    {
        private readonly int maxTier = 10;
        protected readonly float buffFactor = 0.2f;
        protected readonly float debuffFactor = 0.2f;
     
        public string AbilityName => abilityName;

        [SerializeField] private string abilityName;
        [SerializeField] private string abilityDescription;

        [Header("Whether this ability will be always activated while it is active")]
        [SerializeField]
        private bool activateOnlyOnce = false;

        [Header("The ability is activated once every (coolDown) seconds")]
        [SerializeField]
        protected AbilityStat<float> coolDown;

        protected int tier;
        public TraitChart traitChart;

        protected Element element;
        protected List<StatusEffect> effects = new();
        [SerializeField]
        private bool hasBuilt = false;
        protected bool hasActivated;
        protected float coolDownTimer;
        [SerializeField]
        private Sprite abilitySprite;
        
        [SerializeField]
        private UpgradableAnimatorIndex animatorIndex;
        private bool isActive;

        [Header("Recursive ability pool")]
        [SerializeField]
        protected AbilityObjectPooler recursiveAbilityObjectPool;
        protected bool hasRecursive = false;
        private Sprite recursiveSprite;
        private UpgradableAnimatorIndex recursiveAnimatorIndex;

        public bool HasRecursive => hasRecursive;

        [Header("Projectile pool")]
        [SerializeField]
        protected MMObjectPooler projectileObjectPool;

        [Header("Element Magnitudes: Plasma, Cryo, Force, Infect, Pyro")]
        [SerializeField]
        protected List<float> elementMagnitudes =  new List<float>();
        [SerializeField]
        protected float statusEffectThresholdRatio = 0.1f;

        [Header("Sfx")]
        [SerializeField]
        protected SfxHandler sfxHandler;

        protected DamageHandler damageHandler;
        private AbilityGenerator abilityGenerator;

        protected float coolDownMultiplier = 1f;

        /// <summary>
        /// Uses the trait chart to define the behaviours of the ability. 
        /// E.g., Speed, Damage, CoolDown, etc.
        /// MUST be called before the ability can be activated
        /// </summary>
        public void BuildAbility(int tier, TraitChart traitChart)
        {
            this.tier = tier;
            this.traitChart = traitChart;
            BuildElement();
            Build();
            hasBuilt = true;
            isActive = true;
        }

        public void SetOwner(DamageHandler damageHandler, AbilityGenerator abilityGenerator)
        {
            this.damageHandler = damageHandler;
            this.abilityGenerator = abilityGenerator;
        }

        private void CopyAbility(Ability other)
        {
            tier = other.tier;
            traitChart = new TraitChart(other.traitChart);
            element = new Element(other.element);
        }

        /// <summary>
        /// Creates a new ability based on this ability and the consumed ability
        /// </summary>
        public Ability UpgradeAbility(Ability consumedAbility)
        {
            if (CanUpgrade(consumedAbility))
            {
                Ability newAbility = Instantiate(abilityGenerator.GetPrefab(abilityName));
                newAbility.CopyAbility(this);

                bool isMaxTier = this.tier + consumedAbility.tier == 10;
                newAbility.tier = this.tier + (isMaxTier ? 1 : consumedAbility.tier);

                // Element Upgrade
                int additionalLevel = newAbility.tier % 2 + newAbility.tier / 2 - element.GetTotalLevel();
                if (additionalLevel > 0)
                {
                    newAbility.element.CombineWith(consumedAbility.element);
                }

                // Trait Chart Merging
                float pointsToRedistribute = consumedAbility.DebuffTraitsForMerging(newAbility);
                TraitChart chartToCombine = consumedAbility.CreateTraitChartForMerging(pointsToRedistribute, consumedAbility.GetType() == newAbility.GetType());

                newAbility.traitChart.CombineWith(chartToCombine);

                // Build Ability Again
                newAbility.Build();
                newAbility.hasBuilt = true;
                newAbility.isActive = true;

                if (isMaxTier)
                {
                    Ability recursiveAbility = Instantiate(consumedAbility);
                    recursiveAbility.CloneAbility(consumedAbility);

                    newAbility.AddRecursiveAbility(recursiveAbility);
                    newAbility.tier = 10;
                }

                return newAbility;
            } 
            else
            {
                throw new System.Exception("Trying to upgrade past max tier");
            }
        }

        /// <summary>
        /// Checks that the ability can be upgraded if combined tier does not exceed max.
        /// </summary>
        public bool CanUpgrade(Ability consumedAbility)
        {
            return (tier + consumedAbility.tier <= maxTier);
        }

        protected virtual void Update()
        {
            if (!hasBuilt)
            {
                return;
            }

            if (activateOnlyOnce && isActive)
            {
                HandleRecursive();
                return;
            }

            if (isActive && damageHandler != null)
            {
                coolDownTimer -= Time.deltaTime;
                if (coolDownTimer <= 0f)
                {
                    Activate();
                    coolDownTimer = coolDown.value * this.coolDownMultiplier;
                }
            }
        }

        private void OnDestroy()
        {
            projectileObjectPool.DestroyObjectPool();
        }

        private void BuildElement()
        {
            element = new Element();

            int elementLevel = tier % 2 + tier / 2;

            for (int i = 0; i < elementLevel; i++)
            {
                ElementType chosenType = (ElementType)Random.Range(0, 5);
                element.elements[chosenType] += 1;
            }
        }
        protected StatusEffect GenerateEffect(ElementType type, float utilityRatio, float maxMagnitude)
        {
            int level = element.elements[type];
            float levelRatio = (float)level / Element.maxLevel;
            float ratio = levelRatio * utilityRatio; 
            if (ratio < 0.1f)
            {
                ratio = 0f;
            }
            float magnitude = ratio * maxMagnitude;
            StatusEffect effect;
            switch (type)
            {
                case ElementType.Plasma:
                    effect = gameObject.AddComponent<PlasmaStatusEffect>();
                    effect.Build(level, magnitude);
                    break;
                case ElementType.Cryo:
                    effect = gameObject.AddComponent<CryoStatusEffect>();
                    effect.Build(level, magnitude);
                    break;
                case ElementType.Force:
                    effect = gameObject.AddComponent<ForceStatusEffect>();
                    effect.Build(level, magnitude);
                    break;
                case ElementType.Infect:
                    effect = gameObject.AddComponent<InfectStatusEffect>();
                    effect.Build(level, magnitude);
                    break;
                case ElementType.Pyro:
                    effect = gameObject.AddComponent<PyroStatusEffect>();
                    effect.Build(level, magnitude);
                    break;
                default:
                    throw new System.Exception("Element Type invalid");
            }
            return effect;
        }

        public Sprite GetSprite()
        {
            return this.abilitySprite;
        }

        public Sprite GetRecursiveSprite()
        {
            return this.recursiveSprite;
        }

        public int GetAnimatorIndex()
        {
            return (int)this.animatorIndex;
        }

        public int GetRecursiveAnimatorIndex()
        {
            return (int)this.recursiveAnimatorIndex;
        }

        // This method is used for recursive abilities.
        public void SetActive(bool isActive)
        {
            this.hasActivated = !isActive;
            this.gameObject.SetActive(isActive);
            this.isActive = isActive;
        }

        public void CloneAbility(Ability other)
        {
            CopyAbility(other);
            SetOwner(other.damageHandler, other.abilityGenerator);
            Build();
            this.hasBuilt = true;
        }

        public void ClearAnyRecursive()
        {
            if (hasRecursive)
            {
                recursiveAbilityObjectPool.DestroyObjectPool();
                Destroy(recursiveAbilityObjectPool.GameObjectToPool);
                hasRecursive = false;
            }
        }

        protected abstract void Build();

        protected abstract void Activate();

        protected abstract void HandleRecursive();

        /// <summary>
        /// Debuff the trait chart of the other ability and returns the points debuffed for redistribution
        /// </summary>
        protected abstract float DebuffTraitsForMerging(Ability other);

        /// <summary>
        /// Returns a new TraitChart for merging based on this ability, reassigning all points and the debuffed points to all abilities such that
        /// the buffed trait is boosted more
        /// </summary>
        protected abstract TraitChart CreateTraitChartForMerging(float pointsToAssign, bool isSameType);

        public void Stop()
        {
            isActive = false;
        }

        public string GetName()
        {
            return $"Level {tier} {abilityName}\n";
        }

        public string GetDescription()
        {
            return abilityDescription;
        }

        public abstract string GetDetails();

        public abstract string GetComparedDetails(Ability other);

        protected string GetComparedFloatString(float original, float upgraded)
        {
            if (upgraded.CompareTo(original) > 0)
            {
                return "<color=#00ff00ff>" + upgraded.ToString("0.0") + "</color>";
            }

            if (upgraded.CompareTo(original) < 0)
            {
                return "<color=#ff3224>" + upgraded.ToString("0.0") + "</color>";
            }

            return upgraded.ToString("0.0");
        }

        protected string GetComparedIntString(int original, int upgraded)
        {
            if (upgraded.CompareTo(original) > 0)
            {
                return "<color=#00ff00ff>" + upgraded + "</color>";
            }

            if (upgraded.CompareTo(original) < 0)
            {
                return "<color=#ff3224>" + upgraded + "</color>";
            }

            return upgraded.ToString();
        }

        protected string GetStatusEffects()
        {
            string result = "Status Effects: ";

            for (int i = 0; i < effects.Count; i++)
            {
                string effectName = effects[i].GetName();

                if (i > 0)
                {
                    result += ", ";
                }

                result += effectName;
            }

            return result;
        }

        protected string GetComparedStatusEffects(Ability other)
        {
            string result = "Status Effects: ";

            for (int i = 0; i < effects.Count; i++)
            {
                if (i > 0)
                {
                    result += ", ";
                }

                StatusEffect upgraded = effects[i];
                StatusEffect original = other.effects.Find(x => x.EqualTypeTo(upgraded));
                if (original == null)
                {
                    result += "<color=#00ff00ff>" + upgraded.GetName() + "</color>";
                }
                else
                {
                    if (upgraded.Level > original.Level)
                    {
                        result += "<color=#00ff00ff>" + upgraded.GetName() + "</color>";
                    }
                    else
                    {
                        result += upgraded.GetName();
                    }
                }
            }

            return result;
        }

        public void SetCoolDownMultiplier(float multiplier)
        {
            this.coolDownMultiplier = multiplier;
        }

        public bool IsAbility()
        {
            return true;
        }

        public bool IsPassiveAbility()
        {
            return false;
        }

        protected void AddRecursiveAbility(Ability recursiveAbility)
        {
            this.hasRecursive = true;
            this.recursiveSprite = recursiveAbility.abilitySprite;
            this.recursiveAnimatorIndex = recursiveAbility.animatorIndex;

            recursiveAbility.activateOnlyOnce = true;
            recursiveAbility.SetActive(false);

            recursiveAbilityObjectPool.GameObjectToPool = recursiveAbility.gameObject;
            recursiveAbilityObjectPool.FillObjectPool();
        }

        protected virtual void Deactivate()
        {
            SetActive(false);
        }

        public TraitChart GetTraitChart()
        {
            return traitChart;
        }
    }
}
