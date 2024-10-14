using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCounter : MonoBehaviour
{
    public static ScoreCounter main;

    private void Awake()
    {
        main = this;
    }
    public TMPro.TextMeshProUGUI countdownTimer;
    public TMPro.TextMeshProUGUI scoreKeeper;
    public TMPro.TextMeshProUGUI executiveCount;
    public TMPro.TextMeshProUGUI fartAssistant;

    Coroutine timerCoroutine;
    public void StartCountdown(float seconds)
    {
        StopCountdown();
        timerCoroutine = StartCoroutine( CountdownTimer(seconds));
    }
    public void StopCountdown()
    {
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);
    }
    IEnumerator CountdownTimer(float dur)
    {
        for (int i =0;  i < dur; i++) {
            int remaining = Mathf.FloorToInt(dur - i);
            if (dur > 60)
                countdownTimer.text = Mathf.FloorToInt(remaining / 60) + ":" + (remaining % 60);
            else
                countdownTimer.text = remaining + "";

            executiveCount.text = Level.main.GetNumExecutives() + "";
            yield return new WaitForSeconds(1);
        }
        timerCoroutine = null;
    }
    public void SetTimerVisible(bool visible)
    {
        countdownTimer.gameObject.SetActive(visible);
        executiveCount.gameObject.SetActive(visible);
        fartAssistant.gameObject.SetActive(!visible);
    }
    public void SetScore(float v)
    {
        scoreKeeper.text = "Score: " + Mathf.Floor(v);
    }
}
