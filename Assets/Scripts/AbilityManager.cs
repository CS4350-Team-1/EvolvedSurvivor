using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    [SerializeField] private List<CombatAbility> abilitiesList;
    private List<bool> isAbilityActive = new List<bool> { true, true, false, false, false, false }; // remove when two abilities are not hard coded in

    public void AddAbility(CombatAbility newAbility)
    {
        for (var i = 0; i < isAbilityActive.Count; i++)
        {
            if (!isAbilityActive[i])
            {
                abilitiesList[i].SetNewStats(newAbility);
                isAbilityActive[i] = true;
                break;
            }
        }
    }

    public void ReplaceAbility(CombatAbility abilityToAdd, int abilityToReplace)
    {
        abilitiesList[abilityToReplace].SetNewStats(abilityToAdd);
    }

    public void RemoveAbility(int abilityToRemove)
    {
        isAbilityActive[abilityToRemove] = false;
    }
}
