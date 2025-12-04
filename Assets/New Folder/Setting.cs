using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class IntroMenuManager : MonoBehaviour
{
    public RectTransform pointer;
    public TMP_Text[] menuOptions; // Start, Back 순서
    public Vector2[] pointerPositions; 
    private int currentIndex = 0;

    void Start()
    {
        UpdatePointer();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentIndex = Mathf.Max(0, currentIndex - 1);
            UpdatePointer();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentIndex = Mathf.Min(menuOptions.Length - 1, currentIndex + 1);
            UpdatePointer();
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Z))
        {
            SelectOption();
        }
    }

    void UpdatePointer()
    {
        pointer.anchoredPosition = pointerPositions[currentIndex];
    }

    void SelectOption()
    {
        switch (currentIndex)
        {
            case 0:
                SceneManager.LoadScene("OakScene");
                break;
            case 1:
                SceneManager.LoadScene("SampleScene");
                break;
        }
    }


    public void SelectByClick(int index)
    {
        currentIndex = index;
        UpdatePointer();
        SelectOption();
    }
}
