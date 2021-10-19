using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollisions : MonoBehaviour
{
    private Coin coin;
    private void Awake()
    {
        coin = GetComponent<Coin>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CoinLine"))
        {
            CoinLineRendererController.instance.SetColor(Color.green);
            coin.touchedLine = true;
        }
        else if (other.CompareTag("Goal"))
        {
            if (!coin.touchedLine)
            {
                EventManager.TriggerEvent(Events.LevelLost, new EventParam { topText = "Level Failed!", bottomText = "Hakem ofsayt var :(", levelWon = false });
            }
            else
            {
                EventManager.TriggerEvent(Events.LevelWon, new EventParam { topText = "Level Cleared!", bottomText = "Vurdu ve GOOOOOOOOOOOOOOOOOOOOOOOOOOOL", levelWon = true });
            }
        }

    }

    // Calculate win/lose events
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Coin"))
        {
            if (!coin.touchedLine)
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

            if (!coin.touchedLine)
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
            if (!coin.touchedLine)
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
