using HYPLAY.Core.Runtime;
using UnityEngine;

public class LoginPage : MonoBehaviour
{
    public GameObject signInSplash;
    public TMPro.TextMeshProUGUI text;

    private void Awake()
    {
        HyplayBridge.LoggedIn += OnLoggedIn;
        if (HyplayBridge.IsLoggedIn)
        {
            OnLoggedIn();
        }
    }

    private async void OnLoggedIn()
    {
        signInSplash.gameObject.SetActive(false);
        var res = await HyplayBridge.GetUserAsync();
        if (res.Success && text!=null)
            text.text = $"Welcome {res.Data.Username}";
    }
}
