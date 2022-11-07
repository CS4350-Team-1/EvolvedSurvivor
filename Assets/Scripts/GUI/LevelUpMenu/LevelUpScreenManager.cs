using System.Collections.Generic;
using TeamOne.EvolvedSurvivor;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.TopDownEngine;

// This class handles the player actions in the level up screen.
public class LevelUpScreenManager : MonoBehaviour
{
    private const string ERROR_MAX_ABILITIES_REACHED = "Current abilities are full!";

    private const string TITLE_SELECT_FIRST_ABILITY = "SELECT YOUR FIRST ABILITY";
    private const string TITLE_LEVEL_UP = "LEVEL UP";

    [Header("UI elements")]
    [SerializeField] private Text screenTitle;
    [SerializeField] private Text textObj;
    [SerializeField] private GameObject addAbilityMenu;
    [SerializeField] private GameObject mergeAbilityMenu;
    [SerializeField] private LevelUpHeaderButton addAbilityHeaderButton;
    [SerializeField] private LevelUpHeaderButton mergeAbilityHeaderButton;

    [SerializeField] private CurrentAbilityUI[] currentAbilities;
    [SerializeField] private CurrentPassiveAbilityUI[] currentPassiveAbilities;

    [Header("Action handlers")]
    [SerializeField] private AddUpgradableHandler addUpgradableHandler;
    [SerializeField] private MergeAbilityHandler mergeAbilityHandler;

    private AbilityManager abilityManager;
    private PassiveAbilityManager passiveAbilityManager;
    private int nextAbilityIndex = 0;
    private int nextPassiveAbilityIndex = 0;
    private int maxAbilityCount;
    private int maxPassiveAbilityCount;
    private bool wasLoadedBefore = false;
    private CharacterPause pause;

    private void Awake()
    {
        this.maxAbilityCount = this.currentAbilities.Length;
        this.maxPassiveAbilityCount = this.currentPassiveAbilities.Length;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        this.pause = player.GetComponent<ESCharacterPause>();
        this.abilityManager = player.GetComponentInChildren<AbilityManager>();
        this.passiveAbilityManager = player.GetComponentInChildren<PassiveAbilityManager>();
    }

    private void OnEnable()
    {
        if (this.pause != null)
        {
            this.pause.AbilityPermitted = false;
        }

        RefreshCurrentUpgradables();

        // TODO: should stay on add ability if passives not maximised?
        if (this.nextAbilityIndex >= this.maxAbilityCount)
        {
            SwitchToMergeMenu();
        }
        else 
        {
            SwitchToAddAbilityMenu();
        }

        if (!wasLoadedBefore)
        {
            wasLoadedBefore = true;
            screenTitle.text = TITLE_SELECT_FIRST_ABILITY;
        }
        else
        {
            screenTitle.text = TITLE_LEVEL_UP;
        }
    }

    private void OnDisable()
    {
        if (this.pause != null)
        {
            this.pause.AbilityPermitted = true;
        }
    }

    public void SwitchToAddAbilityMenu()
    {
        foreach (CurrentAbilityUI currentAbility in currentAbilities)
        {
            currentAbility.DisableButton();
        }
        mergeAbilityHandler.ClearInput();

        this.addAbilityMenu.SetActive(true);
        this.mergeAbilityMenu.SetActive(false);

        this.addAbilityHeaderButton.SetSelected();
        this.mergeAbilityHeaderButton.ResetAppearance();
    }

    public void SwitchToMergeMenu()
    {
        foreach (CurrentAbilityUI currentAbility in currentAbilities)
        {
            currentAbility.EnableButton();
        }

        this.addUpgradableHandler.ClearCurrentSelectedUpgradable();

        if (nextAbilityIndex < maxAbilityCount)
        {
            this.currentAbilities[nextAbilityIndex].RemoveAbility();
        }

        if (nextPassiveAbilityIndex < maxPassiveAbilityCount)
        {
            this.currentPassiveAbilities[nextPassiveAbilityIndex].RemoveAbility();
        }

        this.addAbilityMenu.SetActive(false);
        this.mergeAbilityMenu.SetActive(true);

        this.addAbilityHeaderButton.ResetAppearance();
        this.mergeAbilityHeaderButton.SetSelected();
    }

    public void AddNewAbility(Ability ability)
    {
        if (this.nextAbilityIndex >= this.maxAbilityCount)
        {
            this.textObj.text = ERROR_MAX_ABILITIES_REACHED;
        } 
        else
        {
            this.currentAbilities[nextAbilityIndex].AddAbilityToButton(ability);
        }  
    }

    public void AddNewPassiveAbility(PassiveAbility passiveAbility)
    {
        this.currentPassiveAbilities[nextPassiveAbilityIndex].AddPassiveAbilityToButton(passiveAbility);
    }

    public void RefreshCurrentUpgradables()
    {
        foreach (CurrentAbilityUI currentAbility in currentAbilities)
        {
            currentAbility.RemoveAbility();
        }

        List<Ability> abilities = abilityManager.Abilities;

        for (int i = 0; i < abilities.Count; i++)
        {
            currentAbilities[i].AddAbilityToButton(abilities[i]);   
        }
        nextAbilityIndex = abilities.Count;

        foreach (CurrentPassiveAbilityUI currentPassiveAbility in currentPassiveAbilities)
        {
            currentPassiveAbility.RemoveUpgradable();
        }

        List<PassiveAbility> passiveAbilities = passiveAbilityManager.GetActiveAbilities();

        for (int i = 0; i < passiveAbilities.Count; i++)
        {
            currentPassiveAbilities[i].AddPassiveAbilityToButton(passiveAbilities[i]);
        }
        nextPassiveAbilityIndex = passiveAbilities.Count;
    }
}
