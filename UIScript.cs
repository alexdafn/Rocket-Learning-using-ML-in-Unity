using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIScript : MonoBehaviour
{

    public void PlayLevel()
    {
        Time.timeScale=2;
        //pausePanel.SetActive(true);
    }

    public void PauseLevel()
    {
        Time.timeScale=0;
        //pausePanel.SetActive(false);
    }

    public void LoadLevel1()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadLevel2()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadLevel3()
    {
        SceneManager.LoadScene(2);
    }

    public void LoadLevel4()
    {
        SceneManager.LoadScene(3);
    }
    public void LoadLevel5()
    {
        SceneManager.LoadScene(4);
    }
    public void LoadLevel5Imitation()
    {
        SceneManager.LoadScene(5);
    }
    public void LoadLevel6()
    {
        SceneManager.LoadScene(6);
    }
    public void LoadLevel7()
    {
        SceneManager.LoadScene(7);
    }
    public void LoadLevel8()
    {
        SceneManager.LoadScene(8);
    }


}
