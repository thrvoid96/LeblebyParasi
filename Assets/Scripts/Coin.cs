using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private Transform goalTransform;
    private bool selected;

    public bool isSelected
    {
        get { return selected; }
        set { selected = value; }
    }

    private Vector3 startPos,endPos;
    private bool touchedLine;
    private Rigidbody rb;

    

    private void OnEnable()
    {
        EventManager.StartListening(Events.CoinTravelling, startChecks);
        EventManager.StartListening(Events.CoinStopped, finalChecks);
    }


    private void OnDisable()
    {
        EventManager.StopListening(Events.CoinTravelling, startChecks);
        EventManager.StartListening(Events.CoinStopped, finalChecks);
    }

    private void Start()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CoinLine"))
        {
            CoinLineRendererController.instance.SetColor(Color.green);
            Debug.LogError("Helal �izgiye de�din :)");
            touchedLine = true;
        }
        else if (other.CompareTag("Goal"))
        {
            Debug.LogError("Vurdu ve GOOOOOOOOOOOOOOOOOOOOOOOOOOOL");
            EventManager.TriggerEvent(Events.LevelWon, new EventParam());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Coin") && !touchedLine)
        {
            Debug.LogError("�izgiden ge�meden �nce ba�ka bir paraya dokundun :)");
            EventManager.TriggerEvent(Events.LevelLost, new EventParam());
        }
    }

    private void startChecks(EventParam param)
    {
        if (isSelected)
        {
            startPos = transform.position;
            StartCoroutine(checkIfCoinStopped());
        }       
    }

    private void finalChecks(EventParam param)
    {
        if (isSelected)
        {
            isSelected = false;

            var eventParam = new EventParam
            {
                linePositions = new Vector3[CoinLineRendererController.instance.GetPositionCount()],
                tappedCoin = gameObject
            };

            EventManager.TriggerEvent(Events.PositionsSet, eventParam);

            var coinLineCenter = CoinLineRendererController.instance.centerPoint;
            if (Vector3.Distance(startPos, goalTransform.position) < Vector3.Distance(coinLineCenter, goalTransform.position))
            {

                if (Vector3.Distance(endPos, goalTransform.position) < Vector3.Distance(coinLineCenter, goalTransform.position))
                {
                    if (!touchedLine)
                    {
                        Debug.LogError("Geri bile gidemedin :(");
                    }
                    else
                    {
                        Debug.LogError("S�rf �izgiye de�mek i�in miydi :(.");
                    }
                }
                else
                {
                    if (!touchedLine)
                    {
                        Debug.LogError("Geriledin :(");
                    }
                    else
                    {
                        Debug.LogError("Tersten gol att�n :(");
                    }
                }
            }

            else
            {
                if (Vector3.Distance(endPos, goalTransform.position) < Vector3.Distance(coinLineCenter, goalTransform.position))
                {
                    if (!touchedLine)
                    {
                        Debug.LogError("�izgiye de�meyi unuttun :(");
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
                        Debug.LogError("�izgiye yeti�emedin bile :(");
                    }
                    else
                    {
                        Debug.LogError("�izgiye de�din ama ge�emedin :(");
                    }
                }
            }

            
        }
    }

    private IEnumerator checkIfCoinStopped()
    {
        yield return new WaitForSeconds(0.05f);

        while (true)
        {
            //Debug.LogError(rb.velocity.magnitude);
            if (rb.velocity.magnitude <= 0f)
            {
                EventManager.TriggerEvent(Events.CoinStopped, new EventParam { tappedCoin = gameObject });
                endPos = transform.position;
                yield break;
            }
            yield return null;
        }
    }
}
