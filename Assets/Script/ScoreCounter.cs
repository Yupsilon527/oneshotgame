using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCounter : MonoBehaviour
{
    public static ScoreCounter main;

    public TMPro.TextMeshProUGUI countdownTimer;
    int bullets = 0;
    public int nBullets
    {
        get
        {
            return bullets;
        }
        set
        {
            bullets = value;
            //    bulletsText.text = "Bullets: " + bullets;
        }
    }
    int enemies = 0;
    public int nBadies
    {
        get
        {
            return enemies;
        }
        set
        {
            enemies = value;
            //    baddiesText.text = "Baddies: " + enemies;
        }
    }
    Text bulletsText;
    Text baddiesText;
    private void Awake()
    {
        main = this;
        // bulletsText = transform.Find("Bullets").GetComponent<Text>();
        //baddiesText = transform.Find("Enemies").GetComponent<Text>();
    }

    Coroutine timerCoroutine;
    public void StartCountdown(int seconds)
    {
        if (timerCoroutine!=null)
            StopCoroutine(timerCoroutine);
        StartCoroutine( CountdownTimer(seconds));
    }
    IEnumerator CountdownTimer(int dur)
    {
        for (int i =0;  i < dur; i++) {
            int remaining = dur - i;
            if (dur > 60)
                countdownTimer.text = Mathf.FloorToInt(remaining / 60) + ":" + (remaining % 60);
            else
                countdownTimer.text = remaining + "";
                yield return new WaitForSeconds(1);
    }
        timerCoroutine = null;
    }
}
