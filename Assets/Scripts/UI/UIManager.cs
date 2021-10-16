using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    [SerializeField] private PopUpUI popUpUI;
    [SerializeField] private CoinThrowCountUI coinThrowCountUI;

    private void OnEnable()
    {
        EventManager.StartListening(Events.LevelWon, levelWon);
        EventManager.StartListening(Events.LevelLost, levelLost);
        EventManager.StartListening(Events.CoinTravelling, coinStartMove);
    }

    private void OnDisable()
    {
        EventManager.StopListening(Events.LevelWon, levelWon);
        EventManager.StopListening(Events.LevelLost, levelLost);
        EventManager.StopListening(Events.CoinTravelling, coinStartMove);
    }

    private void levelWon(EventParam param)
    {
        popUpUI.gameObject.SetActive(true);
        finishLevel(param.topText, param.bottomText, param.levelWon);
        Time.timeScale = 0f;
    }

    private void levelLost(EventParam param)
    {
        popUpUI.gameObject.SetActive(true);
        finishLevel(param.topText, param.bottomText, param.levelWon);
        Time.timeScale = 0f;
    }

    private void coinStartMove(EventParam param)
    {
        coinThrowCountUI.changeCount(+1);
    }

    public void finishLevel(string topText, string bottomText, bool levelWon)
    {
        popUpUI.setTexts(topText, bottomText, levelWon);
    }

}
