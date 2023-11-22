using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IATSceneManager : MonoBehaviour
{
    public void LoadScene(string sceneName) {
        SceneManager.LoadScene(sceneName); // loads the specified scene.
    }

    public void ExitGame() {
        Application.Quit(); // Quits the game.
    }
}
