using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PopUpUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI topTextUGUI, BottomTextUGUI;
    // Start is called before the first frame update

    public void setTexts(string topText, string bottomText, bool levelWon)
    {
        topTextUGUI.text = topText;
        BottomTextUGUI.text = bottomText;

        if (levelWon)
        {
            topTextUGUI.color = Color.green;
        }
        else
        {
            topTextUGUI.color = Color.red;
        }

    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }
}
