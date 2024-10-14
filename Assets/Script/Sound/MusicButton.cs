using UnityEngine;
using UnityEngine.UI;

public class MusicButton : MonoBehaviour
{
    public Toggle toggle;
    public string buttonKey = "music_muted";
    private void Awake()
    {
        SetMuted(PlayerPrefs.GetInt(buttonKey) == 1);
        toggle?.SetIsOnWithoutNotify(!muted);
    }
    bool muted = false;
    public void ToggleMuted()
    {
        SetMuted(!muted);
    }
    public void SetMuted(bool value)
    {
        PlayerPrefs.SetInt(buttonKey, value ? 1 : 0);
        muted = value;
        if (MusicManager.instance!=null && MusicManager.instance.audioSource != null)
          MusicManager.instance.audioSource.enabled = !muted;
    }
}
