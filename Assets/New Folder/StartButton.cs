using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public void OnStartButtonClicked()
    {
        Debug.Log("버튼 클릭");
        SceneManager.LoadScene("IntroScene"); 
    }
}