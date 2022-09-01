using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] float levelLoadDelay = 2f;

    private void OnCollisionEnter(Collision other) 
    {
        switch(other.gameObject.tag) 
        {
            case "Friendly":
                Debug.Log("This thing is friendly");
                break;
            case "Finish":
                Debug.Log("Congrats, yo, you finished!");
                StartSuccessSequence();
                break;
            default:
                StartCrashSequence();
                break;
        }
    }

    void StartSuccessSequence()
    {
        //TODO add particle effect upom success
        GetComponent<Movement>().enabled = false;
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    void StartCrashSequence()
    {
        //TODO add particle effect upom crash
        GetComponent<Movement>().enabled = false;
        Invoke("ReloadLevel", levelLoadDelay);
    }

    void LoadNextLevel()
    {
        int currentSceneIndex =  SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex +1;
        if(nextSceneIndex==SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex=0;
        }
        SceneManager.LoadScene(nextSceneIndex);
    }
    

    void ReloadLevel()
    {
        int currentSceneIndex =  SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
}
