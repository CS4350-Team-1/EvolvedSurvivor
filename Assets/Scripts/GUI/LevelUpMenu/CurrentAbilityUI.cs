using TeamOne.EvolvedSurvivor;
using UnityEngine;
using UnityEngine.EventSystems;

// This class is added to the Current Abilities in Level Up screen
public class CurrentAbilityUI : AbilityButton
{
    [SerializeField] private MergeAbilityHandler mergeAbilityHandler;

    private bool isEnabled;

    public void DisableButton()
    {
        this.isEnabled = false;
    }

    public void EnableButton()
    {
        this.isEnabled = true;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (this.isEnabled && this.upgradable != null)
        {
            bool wasAdded = this.mergeAbilityHandler.AddAbility(upgradable, this);
            
            if (wasAdded)
            {
                this.mergeAbilityHandler.UpdateOutput();
                this.RemoveAbility();
            }

            clickSfxHandler.PlaySfx();
        }
    }
}
