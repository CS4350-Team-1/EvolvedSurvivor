using TeamOne.EvolvedSurvivor;
using UnityEngine.EventSystems;

public class MergeOutputSlotUI : AbilityMergeSlotUI
{
    private Ability primaryAbility;

    public void AddAbilityToButton(Ability outputAbility, Ability primaryAbility)
    {
        AddAbilityToButton(outputAbility);
        this.primaryAbility = primaryAbility;
    }

    public void ClearSlot()
    {
        if (this.upgradable != null)
        {
            upgradable.ClearAnyRecursive();
            Destroy(upgradable.gameObject);
        }

        this.RemoveAbility();
    }

    public override void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (!isEmpty)
        {
            this.textObj.text = upgradable.GetName() + upgradable.GetDescription();
            this.detailedTextObj.text = upgradable.GetComparedDetails(primaryAbility);

            if (radarChart.isActiveAndEnabled)
            {
                radarChart.UpdateVisual(primaryAbility, upgradable);
            }

            this.animator.SetBool("isHovering", true);

            if (this.upgradable.HasRecursive)
            {
                this.recursiveAnimator.SetBool("isHovering", true);
            }

            enterSfxHandler.PlaySfx();
        }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        // do nothing
    }
}
