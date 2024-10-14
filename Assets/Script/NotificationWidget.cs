using System.Collections;
using UnityEngine;

public class NotificationWidget : MonoBehaviour
{
    public static NotificationWidget instance;
    public void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    public float DefaultNotificationDuration = 5;
    public TMPro.TextMeshProUGUI notificationText;

    bool forced = false;
    public void DisplayNotification(string notificationData, bool force = true)
    {
        if (force || !forced)
        {
            gameObject.SetActive(true);
            notificationText.text = notificationData;
            forced = force;
            StartPopinAnimation(DefaultNotificationDuration);
        }
    }
    Coroutine animatioCoroutine;
    void StartPopinAnimation(float duration)
    {
        if (animatioCoroutine != null)
        {
            StopCoroutine(animatioCoroutine);
        }

        bool hasIntro = !gameObject.activeSelf;
        gameObject.SetActive(true);
        StartCoroutine(DisplayNotificationCoroutine(hasIntro, duration));
    }
    IEnumerator DisplayNotificationCoroutine(bool hasIntro, float duration)
    {
        if (hasIntro)
        {
            if (notificationText.color.a > 0)
                yield return PopOutCoroutine();
            yield return PopInCoroutine();
        }
        notificationText.color = new Color(notificationText.color.r, notificationText.color.g, notificationText.color.b,1);
        yield return new WaitForSeconds(duration);
        yield return PopOutCoroutine();
    }

    IEnumerator FadeAlpha(float startAlpha, float endAlpha, float fadeDuration)
    {
        float elapsedTime = 0f;
        Color color = notificationText.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            color.a = alpha;
            notificationText.color = color;
            yield return null;
        }
        forced = false;
        color.a = endAlpha;
        notificationText.color = color;
    }
    IEnumerator PopInCoroutine()
    {
        yield return FadeAlpha(0,1,1);
    }
    IEnumerator PopOutCoroutine()
    {
        yield return FadeAlpha(1, 0, 1);
        animatioCoroutine = null;
        gameObject.SetActive(false);
    }
}

