using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private List<GameObject> coinsOnScene = new List<GameObject>();
    [SerializeField] private int coinDeselectDistance;

    private List<GameObject> unselectedCoins = new List<GameObject>();
    private bool coinIsSelected;
    private GameObject selectedCoin;

    private Vector2 startTouchPos,endTouchPos;

    private void OnEnable()
    {
        SwipeDetector.OnSwipe += SwipeDetector_OnSwipe;
        EventManager.StartListening(Events.ScreenTouched, OnTap);
        EventManager.StartListening(Events.ScreenUnTouched, OnTapEnded);
        EventManager.StartListening(Events.CoinSelected, OnCoinSelected);
        EventManager.StartListening(Events.CoinDeselected, OnCoinDeselected);
        EventManager.StartListening(Events.CoinTravelling, OnCoinTravelling);

    }


    private void OnDisable()
    {
        SwipeDetector.OnSwipe -= SwipeDetector_OnSwipe;
        EventManager.StopListening(Events.ScreenTouched, OnTap);
        EventManager.StopListening(Events.ScreenUnTouched, OnTapEnded);
        EventManager.StopListening(Events.CoinSelected, OnCoinSelected);
        EventManager.StopListening(Events.CoinDeselected, OnCoinDeselected);
        EventManager.StopListening(Events.CoinTravelling, OnCoinTravelling);

    }

    private void OnTap(EventParam param)
    {
        //I dont know if declaring variables here is bad

        Ray ray = Camera.main.ScreenPointToRay(param.fingerDownPos);
        int coinLayerMask = LayerMask.GetMask("Coin");
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, float.PositiveInfinity, coinLayerMask))
        {
            EventManager.TriggerEvent(Events.CoinSelected,
                new EventParam
                {
                    tappedCoin = hit.collider.gameObject,
                    fingerDownPos = param.fingerDownPos
                });
            //Enable UI arrow
        }
    }

    private void OnTapEnded(EventParam param)
    {
        coinIsSelected = false;
    }
 
    private void OnCoinSelected(EventParam param)
    {
        coinIsSelected = true;

        selectedCoin = param.tappedCoin;

        startTouchPos = param.fingerDownPos;

        selectedCoin.GetComponent<Coin>().isSelected = true;

        for (int i = 0; i < coinsOnScene.Capacity; i++)
        {
            if (selectedCoin != coinsOnScene[i])
            {
                unselectedCoins.Add(coinsOnScene[i]);
            }
        }

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

        StartCoroutine(checkCoinSendDirection());
    }

    private void OnCoinDeselected(EventParam param)
    {
        var emptyPositions = new Vector3[CoinLineRendererController.instance.GetPositionCount()];

        var eventParam = new EventParam
        {
            linePositions = emptyPositions,
            tappedCoin = selectedCoin
        };

        EventManager.TriggerEvent(Events.PositionsSet, eventParam);

        selectedCoin.GetComponent<Coin>().isSelected = false;
        coinIsSelected = false;
        selectedCoin = null;
        unselectedCoins.Clear();
    }

    private void OnCoinTravelling(EventParam param)
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
            if (Vector2.Distance(startTouchPos, endTouchPos) <= coinDeselectDistance && !coinIsSelected)
            {
                EventManager.TriggerEvent(Events.CoinDeselected, new EventParam { tappedCoin = selectedCoin, fingerDownPos = endTouchPos});
                yield break;
            }
            else if(Vector2.Distance(startTouchPos, endTouchPos) > coinDeselectDistance && !coinIsSelected)
            {
                EventManager.TriggerEvent(Events.CoinTravelling, new EventParam { tappedCoin = selectedCoin, fingerDownPos = endTouchPos });
                yield break;
            }

            yield return null;
        }
    }
}
