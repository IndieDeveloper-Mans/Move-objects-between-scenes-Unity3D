// This script moves the GameObject you attach in the Inspector to a Scene you specify in the Inspector.
// Attach this script to an empty GameObject.
// Click on the GameObject, go to its Inspector and type the name of the Scene you would like to load in the Scene field.
// Attach the GameObject you would like to move to a new Scene in the "My Game Object" field

// Make sure your Scenes are in your build (File>Build Settings).

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveGameObjectToScene : MonoBehaviour
{
    public static MoveGameObjectToScene sharedInstance = null;

    // Assign your GameObject you want to move Scene in the Inspector
    public GameObject gameManager;
    public GameObject player;
    public float progress;


    void Awake() 
	{
		if (sharedInstance == null) 
		{
			sharedInstance = this;
		} 
		else if (sharedInstance != this) 
		{
			Destroy (gameObject);  
		}
		DontDestroyOnLoad(gameObject);
	}

    private void Start() 
    {
        if (gameManager == null)
        {
            // Find in your scene object which will be moved later
            gameManager = GameObject.FindGameObjectWithTag("Game Manager");
        }
    }

    public void MovePlayerToBetweenScenes(string sceneName)
    {
        
        if (player == null)
        {
            if (transform.childCount > 0)
            {
                player = gameManager.transform.GetChild(0).gameObject;
                foreach (MonoBehaviour scripts in player.GetComponents(typeof(MonoBehaviour)))
                {
                    scripts.enabled = false;
                }
            
                StartCoroutine(LoadYourAsyncScene(sceneName));   
            } 
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player");
            player.transform.SetParent(gameManager.transform , false);

            if (transform.childCount > 0)
            {
                player = gameManager.transform.GetChild(0).gameObject;
                foreach (MonoBehaviour scripts in player.GetComponents(typeof(MonoBehaviour)))
                {
                    scripts.enabled = false;
                }
                    StartCoroutine(LoadYourAsyncScene(sceneName));   
            }  
        }    
    }


    IEnumerator LoadYourAsyncScene(string sceneName)
    {
        
        // Set the current Scene to be able to unload it later
        Scene currentScene = SceneManager.GetActiveScene();

        // The Application loads the Scene in the background at the same time as the current Scene.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        // Wait until the last operation fully loads to return anything
        while (!asyncLoad.isDone)
        {
            // show loading progress
            progress = asyncLoad.progress * 100;
            Debug.Log("Scene is loading " + progress + " %"); 

            yield return null;
        }
        asyncLoad.allowSceneActivation = false;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        
        Debug.Log("Scene " + SceneManager.GetActiveScene().name +  " was loaded ");
        
        // Move the GameObject (you attach this in the Inspector) to the newly loaded Scene
        SceneManager.MoveGameObjectToScene(gameManager, SceneManager.GetSceneByName(sceneName));
        
        // Unload the previous Scene
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(currentScene);

        while (!asyncUnload.isDone)
        {
            Debug.Log("Wait scene is unloading....");
            yield return null;
        }
        
        Debug.Log("Previous scene was unloaded");
    
        foreach (GameObject gameObjects in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (gameObjects.activeInHierarchy == false)
            {
                gameObjects.SetActive(true);
                
                // uncomment th code bellow, if you need to activate children in rootObjects
                // foreach (Transform child in gameObjects.transform)
                // {
                //     child.gameObject.SetActive(true);
                // }
            }

            //activate all scripts on player
            foreach (MonoBehaviour scripts in player.GetComponents(typeof(MonoBehaviour)))
                {
                    scripts.enabled = true;
                }
        }
    }      
}
