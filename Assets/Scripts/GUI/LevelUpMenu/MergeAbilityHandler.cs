using TeamOne.EvolvedSurvivor;
using UnityEngine;

public class MergeAbilityHandler : MonoBehaviour
{
    // The UI would call this class to get the result of merging primary & secondary ability
    [SerializeField]
    private AbilityMergeSlotUI primaryAbilitySlot;
    [SerializeField]
    private AbilityMergeSlotUI secondaryAbilitySlot;
    [SerializeField]
    private MergeOutputSlotUI outputSlot;
    [SerializeField]
    private LevelUpScreenManager levelUpManager;
    [SerializeField]
    private GameObject confirmButton;

    private AbilityManager abilityManager;
    

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        this.abilityManager = player.GetComponentInChildren<AbilityManager>();    
    }

    public bool AddAbility(Ability ability, CurrentAbilityUI sourceSlot)
    {
        if (this.primaryAbilitySlot.IsEmpty())
        {
            this.primaryAbilitySlot.AddAbilityToButton(ability, sourceSlot);
            return true;
        } 
        else if (this.secondaryAbilitySlot.IsEmpty())
        {
            this.secondaryAbilitySlot.AddAbilityToButton(ability, sourceSlot);
            return true;
        }

        return false;
    }

    // Output ability & prefab (big box) if valid primary & secondary abilities are selected
    public void UpdateOutput()
    {
        if (this.primaryAbilitySlot.IsEmpty() || this.secondaryAbilitySlot.IsEmpty())
        {
            this.outputSlot.ClearSlot();
            confirmButton.SetActive(false);
        } 
        else
        {
            Ability primary = this.primaryAbilitySlot.GetAbility();
            Ability secondary = this.secondaryAbilitySlot.GetAbility();

            if (primary.CanUpgrade(secondary))
            {
                Ability outputAbility = primary.UpgradeAbility(secondary);
                this.outputSlot.AddAbilityToButton(outputAbility, primary);
                confirmButton.SetActive(true);
            }
        }
    }

    public void MergeAbilities()
    {
        Ability primary = this.primaryAbilitySlot.GetAbility();
        Ability secondary = this.secondaryAbilitySlot.GetAbility();
        Ability newAbility = this.outputSlot.GetAbility();

        this.abilityManager.RemoveAbility(primary);
        this.abilityManager.RemoveAbility(secondary);
        this.abilityManager.AddAbility(newAbility);

        primaryAbilitySlot.RemoveAbility();
        secondaryAbilitySlot.RemoveAbility();
        outputSlot.RemoveAbility();
    }

    public void ClearInput()
    {
        primaryAbilitySlot.ClearSelection();
        secondaryAbilitySlot.ClearSelection();
        outputSlot.ClearSlot();

        confirmButton.SetActive(false);
    }
}
