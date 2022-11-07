using UnityEngine;
using UnityEngine.EventSystems;

namespace TeamOne.EvolvedSurvivor
{
    public abstract class AbilityButton : UpgradableButton<Ability>
    {
        [SerializeField] protected SpriteRenderer recursiveSprite;

        public virtual void AddAbilityToButton(Ability ability)
        {
            base.AddUpgradableToButton(ability);

            if (ability.HasRecursive)
            {
                recursiveSprite.gameObject.SetActive(true);
                recursiveSprite.sprite = ability.GetRecursiveSprite();
            }
        }

        // Detect when Cursor leaves the button
        public override void OnPointerExit(PointerEventData eventData)
        {
            this.textObj.text = "";
            this.detailedTextObj.text = "";

            radarChart.ClearVisual();
        }

        public virtual void RemoveAbility()
        {
            if (!this.isEmpty)
            {
                base.RemoveUpgradable();
                recursiveSprite.gameObject.SetActive(false);
            }
        }
    }
}

