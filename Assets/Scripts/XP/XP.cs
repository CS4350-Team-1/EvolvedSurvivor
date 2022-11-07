using MoreMountains.Tools;
using System.Collections;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// This class manages the XP of an object and XP bar
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/Core/XP")] 
	public class XP : MonoBehaviour
	{
		
        [Header("Status")]

        /// the current XP of the character
        [MMReadOnly]
		[Tooltip("the current XP of the character")]
		public float CurrentXP;
		public float MaxLevel = 80f;
		[SerializeField] private AnimationCurve xpScalingCurve;

        [Header("XP")]

		[MMInformation("Add this component to an object and it'll have XP",MoreMountains.Tools.MMInformationAttribute.InformationType.Info,false)]
		/// the initial amount of XP of the object
		[Tooltip("the initial amount of XP of the object")]
		public float InitialXP = 0f;
		/// the maximum amount of XP of the object
		[Tooltip("the maximum amount of XP of the object")]
		public float XPToNextLevel = 5f;
		/// if this is true, XP values will be reset everytime this character is enabled (usually at the start of a scene)
		[Tooltip("if this is true, XP values will be reset everytime this character is enabled (usually at the start of a scene)")]
		public bool ResetXPOnEnable = true;

		[SerializeField] private GUIUpdaterScriptableObject guiUpdater;
		[SerializeField] private ParticleSystem levelUpEffect;
		[SerializeField] private SfxHandler levelUpSfxHandler;

		private float currentLevel = 0;
        
        /// <summary>
        /// On Start, we initialize our XP
        /// </summary>
        private void Awake()
	    {
			SetInitialXP();
	    }

		private void OnDisable()
		{
			StopAllCoroutines();
		}

		public void SetInitialXP()
        {
			UpdateXP(InitialXP);
        }

		/// <summary>
		/// When the object is enabled (on respawn for example), we restore its initial XP levels
		/// </summary>
	    protected void OnEnable()
		{
			if (ResetXPOnEnable)
			{
				SetInitialXP();
			}
        }

		/// <summary>
		/// Called when the character gets XP from picking up XP Items
		/// </summary>
		/// <param name="XP">The XP the character gets.</param>
		public void UpdateXP(float XP)
		{
			if (currentLevel >= MaxLevel)
			{
				return;
			}
            // Set XP to new XP
            CurrentXP = Mathf.Min(CurrentXP + XP, XPToNextLevel);
            guiUpdater.OnXpChange(CurrentXP, XPToNextLevel);
			
            // If MaxXP is reached, level up 
            if (CurrentXP == XPToNextLevel) {
				currentLevel++;
				XPToNextLevel = GetNextLevelXP();
				CurrentXP = 0f;

				IEnumerator coroutine = LevelUp(1f);
        		StartCoroutine(coroutine);
			}				
		}

		private IEnumerator LevelUp(float waitTime)
		{
			levelUpEffect.Play();
            levelUpSfxHandler.PlaySfx();

            yield return new WaitForSeconds(waitTime);
            guiUpdater.OnLevelUp();
            guiUpdater.OnXpChange(CurrentXP, XPToNextLevel);
        }

		/// <summary>
	    /// Change the character's max XP based on current level
	    /// </summary>
	    public float GetNextLevelXP()
	    {
		    return XPToNextLevel * xpScalingCurve.Evaluate(currentLevel / MaxLevel);
        }
    }
}
