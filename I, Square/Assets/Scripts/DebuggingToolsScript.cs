using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DebuggingToolsScript : MonoBehaviour
{
    //public Text playerHealthText;
    //public Text CurrentIframesText;
    //public HealthScript playerHealthScript;

    void Update()
    {
        //playerHealthText.text = playerHealthScript.health.ToString();
        //CurrentIframesText.text = playerHealthScript.currentIframes.ToString();

        if ((Input.GetKey(KeyCode.LeftCommand)||Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
