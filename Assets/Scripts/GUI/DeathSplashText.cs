using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathSplashText : MonoBehaviour
{
    private readonly string TITLE_LEVEL_PASSED = "LEVEL PASSED :)";

    [SerializeField] private Timer timer;
    [SerializeField] private KillCounter killCounter;

    [SerializeField] private Text deathSplashTitle;
    [SerializeField] private Text timeSurvivedText;
    [SerializeField] private Text enemiesKilledText;

    void Awake() {
        this.timeSurvivedText.text += timer.GetFinalTime();
        this.enemiesKilledText.text += killCounter.GetEnemiesKilled();

        if (timer.LevelWasPassed())
        {
            this.deathSplashTitle.text = TITLE_LEVEL_PASSED;
        }
    }
}
