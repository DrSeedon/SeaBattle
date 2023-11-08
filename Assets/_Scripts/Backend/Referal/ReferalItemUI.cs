using TMPro;
using UnityEngine;

public class ReferalItemUI : MonoBehaviour
{
    public TMP_Text referalIdText; // Текст для отображения ID реферала

    public void Initialize(int referalId)
    {
        referalIdText.text = "Referal ID: " + referalId;
    }
}