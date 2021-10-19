using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Coin : MonoBehaviour
{
    [SerializeField] private Transform goalTransform;
    [SerializeField] private Player player;
    [SerializeField] private GameObject powerArrowGameObject;
    [SerializeField] private Image interiorFillImage;
    [SerializeField] private int coinThrowableAmount;
    [SerializeField] private Material usedCoinMat;
    [SerializeField] private float speed;
    [SerializeField] private int maxForce;
    [SerializeField] private TextMeshProUGUI coinThrowAmountUGUI;

    private MeshRenderer meshRenderer;
    private bool lineIsTouched;

    public int throwableAmount
    {
        get { return coinThrowableAmount; }
        set { coinThrowableAmount = value; }
    }

    public bool touchedLine
    {
        get { return lineIsTouched; }
        set { lineIsTouched = value; }
    }

    private Vector3 startPos, endPos;
    private Rigidbody rb;



    private void Awake()
    {
        startPos = transform.position;
        coinThrowAmountUGUI.text = throwableAmount.ToString();
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    //Calculate win/lose events


    public void startMovement(EventParam param)
    {
        moveObjectTowardsDirection(param);
    }

    private void moveObjectTowardsDirection(EventParam param)
    {
        coinThrowAmountUGUI.text = throwableAmount.ToString();

        startPos = transform.position;

        var clampX = Mathf.Clamp(speed * param.DirectionVector.x, -maxForce, maxForce);
        var clampY = Mathf.Clamp(speed * param.DirectionVector.y, -maxForce, maxForce);

        rb.AddForce(new Vector3(clampX, 0, clampY));

        StartCoroutine(checkIfCoinStopped());
    }

    public void endMovement(EventParam param)
    {

        checkIfNoMovesLeft();

        closeLineRenderer();

        calculateCoinEvents();

        touchedLine = false;

    }

    private void checkIfNoMovesLeft()
    {
        if (coinThrowableAmount == 0)
        {
            meshRenderer.material = usedCoinMat;
            int unusableCoinCount = 0;

            for (int i = 0; i < player.coinsOnScene.Count; i++)
            {
                if (player.coinsOnScene[i].coinThrowableAmount == 0)
                {
                    unusableCoinCount++;
                }
            }

            if (unusableCoinCount == player.coinsOnScene.Count)
            {
                EventManager.TriggerEvent(Events.LevelLost, new EventParam { topText = "Level Failed!", bottomText = "Bütün para fýrlatma haklarýný kullandýn :(", levelWon = false });
            }

        }
    }

    //Where did the coin go compared to the Line between other 2 coins
    private void calculateCoinEvents()
    {
        var coinLineCenter = CoinLineRendererController.instance.centerPoint;
        var startPosToGoalDistance = Vector3.Distance(startPos, goalTransform.position);
        var coinLineToGoalDistance = Vector3.Distance(coinLineCenter, goalTransform.position);
        var endPosToGoalDistance = Vector3.Distance(endPos, goalTransform.position);

        if (startPosToGoalDistance < coinLineToGoalDistance)
        {

            if (endPosToGoalDistance < coinLineToGoalDistance)
            {
                if (!touchedLine)
                {
                    EventManager.TriggerEvent(Events.LevelLost, new EventParam { topText = "Level Failed!", bottomText = "Çizgiye deðmeden nereye :(", levelWon = false });
                }
                else
                {
                    Debug.LogError("Bu da olur :)");
                }
            }
            else
            {
                if (!touchedLine)
                {
                    EventManager.TriggerEvent(Events.LevelLost, new EventParam { topText = "Level Failed!", bottomText = "Ýlerledin ama çizgiye deðmedin :(", levelWon = false });
                }
                else
                {
                    Debug.LogError("Böyle devam :)");
                }
            }
        }

        else
        {
            if (endPosToGoalDistance < coinLineToGoalDistance)
            {
                if (!touchedLine)
                {
                    EventManager.TriggerEvent(Events.LevelLost, new EventParam { topText = "Level Failed!", bottomText = "Çizgiye deðmeyi unuttun :(", levelWon = false });
                }
                else
                {
                    Debug.LogError("Helal çizgiyi güzel geçtin :)");
                }
            }
            else
            {
                if (!touchedLine)
                {
                    EventManager.TriggerEvent(Events.LevelLost, new EventParam { topText = "Level Failed!", bottomText = "Çizgiye yetiþemedin bile :(", levelWon = false });
                }
                else
                {
                    Debug.LogError("Deðiþik bir geçiþ diyelim :)");
                }
            }
        }
    }

    private void closeLineRenderer()
    {
        var eventParam = new EventParam
        {
            linePositions = new Vector3[CoinLineRendererController.instance.GetPositionCount()],
        };

        EventManager.TriggerEvent(Events.PositionsSet, eventParam);

        CoinLineRendererController.instance.SetColor(Color.red);
    }

    public void turnCoin(float touchDistance, Vector3 direction)
    {
        interiorFillImage.fillAmount = Mathf.Clamp(touchDistance / 200f, 0, 1);
        transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y));
    }

    public void activatePowerBar(bool value)
    {
        powerArrowGameObject.SetActive(value);
    }

    private IEnumerator checkIfCoinStopped()
    {
        yield return new WaitForSeconds(0.05f);

        while (true)
        {
            //Debug.LogError(rb.velocity.magnitude);
            if (rb.velocity.magnitude <= 0.1f)
            {
                EventManager.TriggerEvent(Events.CoinStopped, new EventParam { tappedCoin = this }) ;
                endPos = transform.position;
                yield break;
            }
            yield return null;
        }
    }
}
