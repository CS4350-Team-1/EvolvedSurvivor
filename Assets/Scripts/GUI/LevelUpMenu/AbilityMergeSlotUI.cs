using UnityEngine;
using UnityEngine.EventSystems;
using TeamOne.EvolvedSurvivor;
using MoreMountains.TopDownEngine;

// This class is added to the primary & secondary ability slots on Merge Abilities screen
public class AbilityMergeSlotUI : AbilityButton
{
    private const string TIP_MERGE_INSTRUCTION = "Click on Current Abilities to merge them.";

    [SerializeField] protected MergeAbilityHandler mergeAbilityHandler;
    [SerializeField] protected LevelUpScreenManager levelUpManager;
    [SerializeField] protected Animator animator;
    [SerializeField] protected Animator recursiveAnimator;

    private CurrentAbilityUI sourceSlot;

    public override void OnPointerClick(PointerEventData pointerEventData)
    {
        ClearSelection();
    }

    public void ClearSelection()
    {
        if (!this.isEmpty)
        {
            this.sourceSlot.AddAbilityToButton(this.upgradable);
            this.RemoveAbility();
            this.mergeAbilityHandler.UpdateOutput();
            clickSfxHandler.PlaySfx();
        }
    }

    public override void AddAbilityToButton(Ability ability)
    {
        base.AddAbilityToButton(ability);
        this.animator.SetFloat("upgradableIndex", ability.GetAnimatorIndex());

        if (ability.HasRecursive)
        {
            Debug.Log(ability.GetRecursiveAnimatorIndex());
            this.recursiveAnimator.SetFloat("upgradableIndex", ability.GetRecursiveAnimatorIndex());
        }
    }

    public void AddAbilityToButton(Ability ability, CurrentAbilityUI sourceSlot)
    {
        this.AddAbilityToButton(ability);
        this.sourceSlot = sourceSlot;
    }

    public override void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (!this.isEmpty) {
            base.OnPointerEnter(pointerEventData);
            this.animator.SetBool("isHovering", true);

            if (this.upgradable.HasRecursive)
            {
                this.recursiveAnimator.SetBool("isHovering", true);
            }
        }
        else 
        {
            this.textObj.text = TIP_MERGE_INSTRUCTION;
            enterSfxHandler.PlaySfx();
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        if (!this.isEmpty)
        {
            this.animator.SetBool("isHovering", false);

            if (this.upgradable.HasRecursive)
            {
                this.recursiveAnimator.SetBool("isHovering", false);
            }
        }
    }

    public Ability GetAbility()
    {
        return this.upgradable;
    }
}
