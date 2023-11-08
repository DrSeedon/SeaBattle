using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AuthUIView : MonoBehaviour
{
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public TMP_InputField referralCodeField;
    public Button loginButton;
    public Button registerButton;
    public TMP_Text statusText;
    public TMP_Text playerInfoText;

    public void UpdateStatusText(string status)
    {
        statusText.text = status;
    }

    public void UpdatePlayerInfoText(string playerInfo)
    {
        playerInfoText.text = playerInfo;
    }

    // Остальные методы управления UI, если потребуются
}