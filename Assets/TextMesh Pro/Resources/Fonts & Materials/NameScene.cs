using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class NameSceneTextClick : MonoBehaviour, IPointerClickHandler
{
    public TMP_InputField inputField;
    public TMP_Text decideText;
    public TMP_Text quitText;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerEnter == decideText.gameObject)
        {
            OnDecide();
        }
        else if (eventData.pointerEnter == quitText.gameObject)
        {
            OnQuit();
        }
    }

    public void OnDecide()
    {
        string name = inputField.text.Trim();
        if (!string.IsNullOrEmpty(name))
        {
            PlayerPrefs.SetString("playerName", name);
            PlayerPrefs.Save();
            SceneManager.UnloadSceneAsync("NameScene");
        }
    }

    public void OnQuit()
    {
        PlayerPrefs.DeleteKey("playerName");
        SceneManager.UnloadSceneAsync("NameScene");
    }
}
