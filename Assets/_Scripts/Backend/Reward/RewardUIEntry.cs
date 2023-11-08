using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardUIEntry : MonoBehaviour
{
    public TMP_Text typeText;
    public TMP_Text itemOrCountText;

    private Authentication.Reward reward;

    public void Initialize(Authentication.Reward reward)
    {
        this.reward = reward;

        typeText.text = "Type: " + reward.type;
        if (reward.type == "item")
        {
            itemOrCountText.text = "Item: " + reward.item;
        }
        else
        {
            itemOrCountText.text = "Count: " + reward.count;
        }
    }
}