using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour
{
    // Just in case we need game manager
    // GameManager gm;
    // private void Start() {
    //     gm = GameManager.GetInstance();
    // }

    public void StartButton(){
        SceneManager.LoadScene("Game");
    }

    public void QuitButton(){
        Application.Quit();
    }

    public void RestartButton(){
        SceneManager.LoadScene("MainMenu");
    }

}
