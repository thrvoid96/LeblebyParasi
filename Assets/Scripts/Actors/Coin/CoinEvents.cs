using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinEvents : MonoBehaviour
{
    private Coin coin;
    private CoinCollisions coinCollisions;

    private void Start()
    {
        coin = GetComponent<Coin>();
        coinCollisions = GetComponent<CoinCollisions>();

        coin.enabled = false;
        coinCollisions.enabled = false;
    }

    private void OnEnable()
    {
        EventManager.StartListening(Events.CoinTravelling, triggerCoinStartMoveEvent);
        EventManager.StartListening(Events.CoinStopped, triggerCoinEndMoveEvent);
    }


    private void OnDisable()
    {
        EventManager.StopListening(Events.CoinTravelling, triggerCoinStartMoveEvent);
        EventManager.StopListening(Events.CoinStopped, triggerCoinEndMoveEvent);
    }

    private void triggerCoinStartMoveEvent(EventParam param)
    {
        if (param.tappedCoin == coin)
        {
            coin.enabled = true;
            coinCollisions.enabled = true;
            coin.startMovement(param);
        }
        else
        {
            coin.enabled = false;
            coinCollisions.enabled = false;
        }
    }

    private void triggerCoinEndMoveEvent(EventParam param)
    {
        if (param.tappedCoin == coin)
        {
            coin.enabled = true;
            coinCollisions.enabled = true;
            coin.endMovement(param);
        }
        else
        {
            coin.enabled = false;
            coinCollisions.enabled = false;
        }
    }

}
