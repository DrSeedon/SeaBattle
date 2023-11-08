using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardUIEntry : MonoBehaviour
{
    public TMP_Text dayText;
    public TMP_Text rewardText;
    public Image checkmarkImage;

    public void SetData(int day, string rewardDescription, bool isReceived)
    {
        dayText.text = $"Day {day}";
        rewardText.text = rewardDescription;
        checkmarkImage.enabled = isReceived;
    }
}
