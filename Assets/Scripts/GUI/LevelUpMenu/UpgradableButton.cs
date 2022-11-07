using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TeamOne.EvolvedSurvivor
{
    public abstract class UpgradableButton<T> : UIButton, IPointerExitHandler where T : Upgradable
    {
        [SerializeField] protected Text textObj;
        [SerializeField] protected Text detailedTextObj;
        [SerializeField] protected SpriteRenderer upgradableSprite;
        [SerializeField] protected T upgradable;
        [SerializeField] protected RadarChartUI radarChart;

        protected bool isEmpty = true;

        public bool IsEmpty()
        {
            return this.isEmpty;
        }

        public virtual void AddUpgradableToButton(T upgradable)
        {
            this.upgradable = upgradable;
            this.upgradableSprite.gameObject.SetActive(true);
            this.upgradableSprite.sprite = upgradable.GetSprite();
            this.isEmpty = false;
        }

        // Detect if the Cursor starts to pass over the button
        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (!IsEmpty())
            {
                this.textObj.text = upgradable.GetName() + upgradable.GetDescription();
                this.detailedTextObj.text = upgradable.GetDetails();

                if (radarChart.isActiveAndEnabled)
                {
                    radarChart.UpdateVisual(upgradable);
                }

                enterSfxHandler.PlaySfx();
            }
        }

        // Detect when Cursor leaves the button
        public abstract void OnPointerExit(PointerEventData eventData);

        public virtual void RemoveUpgradable()
        {
            if (!this.isEmpty)
            {
                this.isEmpty = true;
                this.upgradable = default;
                this.upgradableSprite.gameObject.SetActive(false);
            }
        }
    }
}

