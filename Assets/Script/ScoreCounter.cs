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
    public TMPro.TextMeshProUGUI executiveCount;

    Coroutine timerCoroutine;
    public void StartCountdown(int seconds)
    {
        StopCountdown();
        StartCoroutine( CountdownTimer(seconds));
    }
    public void StopCountdown()
    {
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);
    }
    IEnumerator CountdownTimer(int dur)
    {
        for (int i =0;  i < dur; i++) {
            int remaining = dur - i;
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
    }
}
