using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] private Text counter;
    [SerializeField] private float gameLength = 600f;

    private float timePassed = 0f;
    private bool isTimerRunning = true;

    private void Start()
    {
        LevelManager.Instance.Players[0].GetComponent<Health>().OnDeath += StopTimer;
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            timePassed = timePassed + Time.deltaTime;
            counter.text = MMTime.FloatToTimeString(timePassed);
        }
    }

    private void StopTimer()
    {
        isTimerRunning = false;
    }

    public string GetFinalTime()
    {
        return counter.text;
    }

    public bool LevelWasPassed()
    {
        return timePassed > gameLength;
    }
}
