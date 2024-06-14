using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Setting()
    {
        StartCoroutine(DelaySceneLoad("Settings_1"));
    }

    public void Back()
    {
        StartCoroutine(DelaySceneLoad("MainMenu"));
    }

    public void BetweenSettings()
    {
    
        StartCoroutine(DelaySceneLoad("Settings_2"));
    }

     IEnumerator DelaySceneLoad(string name)
 {
     yield return new WaitForSeconds(0.3f);
     SceneManager.LoadScene(name);
 }
}
