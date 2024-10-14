
using UnityEngine.UI;
using UnityEngine;

public class MuteButton : MonoBehaviour
{
    public Toggle toggle;
    public string buttonKey = "muted";
    private void Awake()
    {
        SetVolume(PlayerPrefs.GetInt(buttonKey) == 1);
        toggle?.SetIsOnWithoutNotify(!muted);
    }
    bool muted = false;
    public void ToggleMuted()
    {
        SetVolume(!muted);
    }
    public void SetVolume(bool value)
    {
        PlayerPrefs.SetInt(buttonKey, value ? 1 : 0);
        muted = value;
        AudioListener.volume = muted ? 0 : 1;
    }
}
