using TeamOne.EvolvedSurvivor;
using UnityEngine.EventSystems;

public class CurrentPassiveAbilityUI : UpgradableButton<PassiveAbility>
{
    public virtual void AddPassiveAbilityToButton(PassiveAbility passiveAbility)
    {
        base.AddUpgradableToButton(passiveAbility);
    }

    public virtual void RemoveAbility()
    {
        if (!this.isEmpty)
        {
            base.RemoveUpgradable();
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        this.textObj.text = "";
        this.detailedTextObj.text = "";
        radarChart.ClearVisual();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        // do nothing
    }
}
