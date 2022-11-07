using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.EventSystems;
using MoreMountains.TopDownEngine;

/// <summary>
/// Handles all GUI effects and changes not handled by GUIManager.
/// </summary>
public class ESGUIManager : MonoBehaviour, MMEventListener<TopDownEngineEvent>
{
    [SerializeField] private GUIUpdaterScriptableObject guiUpdater;
    [SerializeField] private GameManager gameManager;

    /// the XP bars to update
    [Tooltip("the XP bars to update")]
    public MMProgressBar[] XPBars;

    /// the level up screen game object
    [Tooltip("the level up screen game object")]
    public GameObject LevelUpScreen;

    private bool isLevelUpScreenVisible = false;

    private void OnEnable()
    {
        guiUpdater.XpChangeEvent.AddListener(UpdateXPBar);
        guiUpdater.LevelUpEvent.AddListener(ShowLevelUpScreen);
        this.MMEventStartListening();
    }

    private void OnDisable()
    {
        guiUpdater.XpChangeEvent.RemoveListener(UpdateXPBar);
        guiUpdater.LevelUpEvent.RemoveListener(ShowLevelUpScreen);
        this.MMEventStopListening();
    }

    private void Start()
    {
        ShowLevelUpScreen();
    }

    /// <summary>
    /// Shows the level up screen.
    /// </summary>
    public void ShowLevelUpScreen()
    {
        if (LevelUpScreen != null)
        {
            isLevelUpScreenVisible = true;
            LevelUpScreen.SetActive(true);
            EventSystem.current.sendNavigationEvents = true;
             
            gameManager.Pause(PauseMethods.NoPauseMenu);
        }
    }

    /// <summary>
    /// Updates the XP bar.
    /// </summary>
    /// <param name="currentXP">Current XP.</param>
    /// <param name="maxXP">Max XP.</param>
    public void UpdateXPBar(float currentXP, float maxXP)
    {
        if (XPBars == null) { return; }
        if (XPBars.Length <= 0) { return; }

        foreach (MMProgressBar XPBar in XPBars)
        {
            if (XPBar == null) { continue; }
            XPBar.UpdateBar(currentXP, 0f, maxXP);
        }
    }

    public void OnMMEvent(TopDownEngineEvent eventType)
    {
        if (eventType.EventType == TopDownEngineEventTypes.UnPause && isLevelUpScreenVisible)
        {
            isLevelUpScreenVisible = false;
            LevelUpScreen.SetActive(false);
        }
    }
}
