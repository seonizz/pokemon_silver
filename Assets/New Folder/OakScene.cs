using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class OakScene2 : MonoBehaviour
{
    [Header("캐릭터/에셋")]
    public GameObject OakAsset;
    public GameObject marillAsset;
    public GameObject goldAsset;    
    public GameObject goldAssetmini; 

    [Header("대사창 및 대사")]
    public GameObject Textbox;
    public TMP_Text DialogText;
    public GameObject ArrowIcon;

    [Header("이름 패널 관련")]
    public GameObject Pannel;         
    public RectTransform PannelArrow;  
    public TMP_Text[] nameOptions;   

    [Header("커서 위치 지정")]
    public Vector2[] arrowPositions = new Vector2[4]; 

    private string chosenName = "";
    private bool isNameInputDone = false;

    void Start()
    {
        // 중복 호출 제거!
        PokemonSelector.pokemonHasBeenChosen = false;
        AllInactive();
        StartCoroutine(IntroSequence());
    }

    void AllInactive()
    {
        OakAsset.SetActive(false);
        marillAsset.SetActive(false);
        goldAsset.SetActive(false);
        goldAssetmini.SetActive(false);

        Textbox.SetActive(false);
        DialogText.gameObject.SetActive(false);
        ArrowIcon.SetActive(false);

        Pannel.SetActive(false);
        PannelArrow.gameObject.SetActive(false);
    }

    IEnumerator IntroSequence()
    {
        OakAsset.SetActive(true);
        yield return TypeDialogue("이야 오래 기다리게 했구나");
        yield return TypeDialogue("포켓몬스터의 세계에 잘 왔단다!");
        yield return TypeDialogue("나의 이름은 오박사");
        yield return TypeDialogue("모두로부터 포켓몬 박사라고 존경받고 있단다");
        OakAsset.SetActive(false);

        marillAsset.SetActive(true);
        yield return TypeDialogue("포켓몬스터…");
        yield return TypeDialogue("줄여서 포켓몬");
        yield return TypeDialogue("이 세계에는 포켓몬스터로 불려지는");
        yield return TypeDialogue("생명체들이 도처에 살고 있다!");
        yield return TypeDialogue("사람들은 포켓몬들과 정답게 지내거나");
        yield return TypeDialogue("함께 싸우거나…");
        yield return TypeDialogue("서로 도와가며 살아가고 있단다");
        marillAsset.SetActive(false);

        OakAsset.SetActive(true);
        yield return TypeDialogue("하지만 우리들은 아직");
        yield return TypeDialogue("포켓몬 전부를 알고있진 못한다.");
        yield return TypeDialogue("포켓몬의 비밀은 아직도 잔뜩 있다!");
        yield return TypeDialogue("나는 그것을 밝혀내기 위하여");
        yield return TypeDialogue("매일 포켓몬 연구를");
        yield return TypeDialogue("계속하고 있다는 말이다!");
        OakAsset.SetActive(false);

        goldAsset.SetActive(true);
        yield return TypeDialogue("그럼…");
        yield return TypeDialogue("슬슬 너의 이름을 가르쳐다오!");

        yield return ShowNamePanel();

        goldAsset.SetActive(false);
        goldAssetmini.SetActive(true);

        yield return TypeDialogue($"{chosenName}!");
        yield return TypeDialogue("준비는 되었는가?");
        yield return TypeDialogue("드디어 이제부터");
        yield return TypeDialogue("너의 이야기가 시작되어진다!");
        yield return TypeDialogue("즐거운 것도 괴로운 것도");
        yield return TypeDialogue("잔뜩 너를 기다리고 있을 것이다!");
        yield return TypeDialogue("꿈과 모험과!");
        yield return TypeDialogue("포켓몬스터의 세계에!");
        yield return TypeDialogue("렛츠고!!");

        SceneManager.LoadScene("roomscene");
    }

    IEnumerator TypeDialogue(string line)
    {
        yield return new WaitWhile(() => Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.Return));

        Textbox.SetActive(true);
        DialogText.gameObject.SetActive(true);
        DialogText.text = "";

        foreach (char c in line)
        {
            DialogText.text += c;
            yield return new WaitForSeconds(0.07f);
        }

        ArrowIcon.SetActive(true);
        Coroutine blink = StartCoroutine(BlinkArrowIcon());
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return));
        StopCoroutine(blink);
        ArrowIcon.SetActive(false);
    }

    IEnumerator BlinkArrowIcon()
    {
        while (true)
        {
            ArrowIcon.SetActive(true);
            yield return new WaitForSeconds(0.4f);
            ArrowIcon.SetActive(false);
            yield return new WaitForSeconds(0.3f);
        }
    }

    IEnumerator ShowNamePanel()
    {
        Pannel.SetActive(true);
        PannelArrow.gameObject.SetActive(true);

        int index = 0;
        UpdatePannelArrow(index);

        yield return new WaitWhile(() => Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.Return));

        isNameInputDone = false;

        while (!isNameInputDone)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                index = (index + nameOptions.Length - 1) % nameOptions.Length;
                UpdatePannelArrow(index);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                index = (index + 1) % nameOptions.Length;
                UpdatePannelArrow(index);
            }

            if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return))
            {
                string option = nameOptions[index].text.Trim();

                if (option == "스스로 결정" || option == "스스로 결정")
                {
                    SceneManager.LoadScene("NameScene", LoadSceneMode.Additive);

                    yield return StartCoroutine(WaitForNameInput());

                    chosenName = PlayerPrefs.GetString("playerName", "주인공");
                }
                else
                {
                    chosenName = option;
                }
                isNameInputDone = true;
            }
            yield return null;
        }

        Pannel.SetActive(false);
        PannelArrow.gameObject.SetActive(false);
    }

    IEnumerator WaitForNameInput()
    {

        while (true)
        {
            if (!SceneManager.GetSceneByName("NameScene").isLoaded)
                break;

            yield return null;
        }
    }

    void UpdatePannelArrow(int index)
    {
        if (index >= 0 && index < arrowPositions.Length)
            PannelArrow.anchoredPosition = arrowPositions[index];
    }
}
