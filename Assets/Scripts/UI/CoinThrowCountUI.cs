using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinThrowCountUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countTextUGUI;
    private int count;
    public void changeCount(int value)
    {
        count += value;
        countTextUGUI.text = count.ToString();
    }
}
