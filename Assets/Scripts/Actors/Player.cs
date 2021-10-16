using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{   
    [SerializeField] private int coinDeselectDistance;

    public List<Coin> coinsOnScene = new List<Coin>();
    private List<Coin> unselectedCoins = new List<Coin>();
    private bool coinIsSelected;
    private Coin selectedCoin;

    private Vector2 startTouchPos, endTouchPos;

    private void OnEnable()
    {
        SwipeDetector.OnSwipe += SwipeDetector_OnSwipe;
        EventManager.StartListening(Events.ScreenTouched, OnTap);
        EventManager.StartListening(Events.ScreenUnTouched, OnTapEnded);
        EventManager.StartListening(Events.CoinSelected, OnCoinSelected);
        EventManager.StartListening(Events.CoinDeselected, OnCoinDeselected);
        EventManager.StartListening(Events.CoinTravelling, OnCoinStartMove);
    }


    private void OnDisable()
    {
        SwipeDetector.OnSwipe -= SwipeDetector_OnSwipe;
        EventManager.StopListening(Events.ScreenTouched, OnTap);
        EventManager.StopListening(Events.ScreenUnTouched, OnTapEnded);
        EventManager.StopListening(Events.CoinSelected, OnCoinSelected);
        EventManager.StopListening(Events.CoinDeselected, OnCoinDeselected);
        EventManager.StopListening(Events.CoinTravelling, OnCoinStartMove);
    }

    private void OnTap(EventParam param)
    {
        findCoinWithRayCast(param);       
    }

    private void findCoinWithRayCast(EventParam param)
    {
        Ray ray = Camera.main.ScreenPointToRay(param.fingerDownPos);
        int coinLayerMask = LayerMask.GetMask("Coin");
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, float.PositiveInfinity, coinLayerMask))
        {
            var coin = hit.collider.gameObject.GetComponent<Coin>();

            checkIfCoinIsInteractable(param ,coin);
        }
    }

    private void checkIfCoinIsInteractable(EventParam param, Coin coin)
    {
        if (coin.throwableAmount == 0)
        {
            Debug.LogError("O parayý çok fazla kullandýn :(");
        }
        else
        {
            EventManager.TriggerEvent(Events.CoinSelected,
                new EventParam
                {
                    tappedCoin = coin,
                    fingerDownPos = param.fingerDownPos
                });
        }
    }

    private void OnTapEnded(EventParam param)
    {
        coinIsSelected = false;
    }

    private void OnCoinSelected(EventParam param)
    {
        setUnselectedCoins(param);

        selectedCoin.activatePowerBar(true);

        StartCoroutine(checkCoinSendDirection());
    }

    private void setUnselectedCoins(EventParam param)
    {
        coinIsSelected = true;

        selectedCoin = param.tappedCoin;

        startTouchPos = param.fingerDownPos;

        for (int i = 0; i < coinsOnScene.Capacity; i++)
        {
            if (selectedCoin != coinsOnScene[i])
            {
                unselectedCoins.Add(coinsOnScene[i]);
            }
        }

        createLineBetweenUnselectedCoins();

    }

    private void createLineBetweenUnselectedCoins()
    {
        List<Vector3> positions = new List<Vector3>();

        for (int i = 0; i < unselectedCoins.Count; i++)
        {
            positions.Add(unselectedCoins[i].transform.position);
        }

        var eventParam = new EventParam
        {
            linePositions = positions.ToArray(),
            tappedCoin = selectedCoin
        };

        EventManager.TriggerEvent(Events.PositionsSet, eventParam);

        positions.Clear();
    }

    private void OnCoinDeselected(EventParam param)
    {
        closeLineRenderer();

        resetValues();
        
    }

    private void closeLineRenderer()
    {
        var emptyPositions = new Vector3[CoinLineRendererController.instance.GetPositionCount()];

        var eventParam = new EventParam
        {
            linePositions = emptyPositions,
            tappedCoin = selectedCoin
        };

        EventManager.TriggerEvent(Events.PositionsSet, eventParam);
    }  

    private void OnCoinStartMove(EventParam param)
    {
        resetValues();
    }

    private void resetValues()
    {
        coinIsSelected = false;
        selectedCoin = null;
        unselectedCoins.Clear();
    }

    private void SwipeDetector_OnSwipe(SwipeData data)
    {
        endTouchPos = data.fingerDownPos;
    }

    private IEnumerator checkCoinSendDirection()
    {
        while (true)
        {
            var distance = Vector2.Distance(startTouchPos, endTouchPos);
            var direction = startTouchPos - endTouchPos;

            selectedCoin.turnCoin(distance, direction);

            //Debug.LogError(distance);
            if (distance <= coinDeselectDistance && !coinIsSelected)
            {
                selectedCoin.activatePowerBar(false);
                EventManager.TriggerEvent(Events.CoinDeselected, new EventParam { tappedCoin = selectedCoin });                

                yield break;
            }
            else if (distance > coinDeselectDistance && !coinIsSelected)
            {
                selectedCoin.activatePowerBar(false);
                selectedCoin.throwableAmount--;
                EventManager.TriggerEvent(Events.CoinTravelling, new EventParam { tappedCoin = selectedCoin, DirectionVector = startTouchPos - endTouchPos, fingerDownPos = startTouchPos, fingerUpPos = endTouchPos });
                
                yield break;
            }

            yield return null;
        }
    }
}
