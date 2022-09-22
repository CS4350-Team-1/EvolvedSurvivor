using MoreMountains.Tools;
using UnityEngine;

/// <summary>
/// An AIAction used to cause the enemy to explode.
/// </summary>
[AddComponentMenu("Scripts/EnemyAI/Actions/AIActionExplode")]
public class AIActionExplode : AIAction
{
    [SerializeField] private Animator animator;
    [SerializeField] private float explosionDuration = 1f;

    public bool OnlyRunOnce = true;

    protected bool _alreadyRan = false;

    private const string explodeParameterName = "Explode";

    public override void PerformAction()
    {
        if (OnlyRunOnce && !_alreadyRan)
        {
            MMAnimatorExtensions.UpdateAnimatorBoolIfExists(animator, explodeParameterName, true);
            _alreadyRan = true;
            _brain.BrainActive = false;
            Invoke("DestroyObject", explosionDuration);
        }
    }

    /// <summary>
    /// On enter state we reset our flag
    /// </summary>
    public override void OnEnterState()
    {
        base.OnEnterState();
        _alreadyRan = false;
    }

    private void DestroyObject()
    {
        gameObject.SetActive(false);
    }
}
