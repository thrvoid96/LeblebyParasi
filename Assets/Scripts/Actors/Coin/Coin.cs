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
    private bool isSelected;
    

    public int throwableAmount
    {
        get { return coinThrowableAmount; }
        set { coinThrowableAmount = value; }
    }

    private Vector3 startPos,endPos;
    private bool touchedLine;
    private Rigidbody rb;
   
    private void OnEnable()
    {
        EventManager.StartListening(Events.CoinTravelling, startMovement);
        EventManager.StartListening(Events.CoinStopped, endMovement);     
    }


    private void OnDisable()
    {
        EventManager.StopListening(Events.CoinTravelling, startMovement);
        EventManager.StopListening(Events.CoinStopped, endMovement);      
    }

    private void Start()
    {
        startPos = transform.position;
        coinThrowAmountUGUI.text = throwableAmount.ToString();
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
    }  

    //Calculate win/lose events
    private void OnTriggerEnter(Collider other)
    {
        if (isSelected)
        {
            if (other.CompareTag("CoinLine"))
            {
                CoinLineRendererController.instance.SetColor(Color.green);
                touchedLine = true;
            }
            else if (other.CompareTag("Goal"))
            {
                if (!touchedLine)
                {
                    EventManager.TriggerEvent(Events.LevelLost, new EventParam { topText = "Level Failed!", bottomText = "Hakem ofsayt var :(", levelWon = false });
                }
                else
                {
                    EventManager.TriggerEvent(Events.LevelWon, new EventParam { topText = "Level Cleared!", bottomText = "Vurdu ve GOOOOOOOOOOOOOOOOOOOOOOOOOOOL", levelWon = true });
                }
            }
        }
    }

    // Calculate win/lose events
    private void OnCollisionEnter(Collision collision)
    {
        if (isSelected)
        {
            if (collision.gameObject.CompareTag("Coin"))
            {
                if (!touchedLine)
                {
                    EventManager.TriggerEvent(Events.LevelLost, new EventParam { topText = "Level Failed!", bottomText = "�izgiden ge�meden �nce ba�ka bir paraya dokundun :(", levelWon = false });

                }
                else
                {
                    Debug.LogError("�izgiden ge�ip ba�ka bir paraya dokundun :)");
                }               
            }
            else if (collision.gameObject.CompareTag("GoalKeeper"))
            {

                if (!touchedLine)
                {
                    EventManager.TriggerEvent(Events.LevelLost, new EventParam { topText = "Level Failed!", bottomText = "Ofsayt yapmana ra�men kaleci tuttu :(", levelWon = false });

                }
                else
                {
                    EventManager.TriggerEvent(Events.LevelLost, new EventParam { topText = "Level Failed!", bottomText = "Hay�r olamazzz, �utun kaleci taraf�ndan tutuldu :(", levelWon = false });
                }                
            }
            else if (collision.gameObject.CompareTag("Obstacle"))
            {
                if (!touchedLine)
                {
                    EventManager.TriggerEvent(Events.LevelLost, new EventParam { topText = "Level Failed!", bottomText = "Bari �izgiyi ge�seydin, duvara kafa att�n :(", levelWon = false });
                }
                else
                {
                    EventManager.TriggerEvent(Events.LevelLost, new EventParam { topText = "Level Failed!", bottomText = "Duvar� ge�emedin :(", levelWon = false });
                }
            }
        }
    }

    private void startMovement(EventParam param)
    {
        if (param.tappedCoin == this) { isSelected = true; } else { isSelected = false; }

        if (isSelected)
        {
            moveObjectTowardsDirection(param);
        }      
                   
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

    private void endMovement(EventParam param)
    {
        if (isSelected)
        {
            checkIfNoMovesLeft();

            closeLineRenderer();

            calculateCoinEvents();

            touchedLine = false;
        }            
        
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
                EventManager.TriggerEvent(Events.LevelLost, new EventParam { topText = "Level Failed!", bottomText = "B�t�n para f�rlatma haklar�n� kulland�n :(", levelWon = false });
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
                    EventManager.TriggerEvent(Events.LevelLost, new EventParam { topText = "Level Failed!", bottomText = "�izgiye de�meden nereye :(", levelWon = false });
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
                    EventManager.TriggerEvent(Events.LevelLost, new EventParam { topText = "Level Failed!", bottomText = "�lerledin ama �izgiye de�medin :(", levelWon = false });
                }
                else
                {
                    Debug.LogError("B�yle devam :)");
                }
            }
        }

        else
        {
            if (endPosToGoalDistance < coinLineToGoalDistance)
            {
                if (!touchedLine)
                {
                    EventManager.TriggerEvent(Events.LevelLost, new EventParam { topText = "Level Failed!", bottomText = "�izgiye de�meyi unuttun :(", levelWon = false });
                }
                else
                {
                    Debug.LogError("Helal �izgiyi g�zel ge�tin :)");
                }
            }
            else
            {
                if (!touchedLine)
                {
                    EventManager.TriggerEvent(Events.LevelLost, new EventParam { topText = "Level Failed!", bottomText = "�izgiye yeti�emedin bile :(", levelWon = false });
                }
                else
                {
                    Debug.LogError("De�i�ik bir ge�i� diyelim :)");
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
                EventManager.TriggerEvent(Events.CoinStopped, new EventParam ());
                endPos = transform.position;
                yield break;
            }
            yield return null;
        }
    }
}
