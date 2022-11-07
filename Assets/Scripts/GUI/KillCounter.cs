using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.UI;

public class KillCounter : MonoBehaviour, MMEventListener<MMLifeCycleEvent>
{
    [SerializeField] private Text text;

    private int killCount = 0;

    private void OnEnable()
    {
        this.MMEventStartListening();
    }

    private void OnDisable()
    {
        this.MMEventStopListening();
    }

    public void OnMMEvent(MMLifeCycleEvent eventType)
    {
        if (eventType.MMLifeCycleEventTypes == MMLifeCycleEventTypes.Death
            && !eventType.AffectedHealth.Model.CompareTag("PlayerModel"))
        {
            killCount++;
            text.text = killCount.ToString();
        }
    }

    public string GetEnemiesKilled()
    {
        return killCount.ToString();
    }
}
