using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Text;

public class GoldAsset : MonoBehaviour
{
    [Header("UI 연결")]
    public CanvasGroup whiteFlash;
    public GameObject oakImage;
    public GameObject simhyangImage;
    public GameObject marillImage;
    public GameObject playerSprite;
    public GameObject textBox;
    public TMP_Text dialogueText;
    public GameObject continueIcon;

    [Header("이름 선택 UI")]
    public GameObject nameSelectPanel;
    public GameObject panelArrow;
    public TMP_Text[] nameOptions;
    public TMP_Text inputNameDisplay;

    [Header("대사")]
    [TextArea(2, 5)]
    public string[] dialogues;

    private bool isTyping = false;
    private string chosenName = "";
    private bool isChoosingName = false;
    private int selectedIndex = 0;

    void Start()
    {
        // 대사 초기화
        dialogues = new string[]
        {
            "이야- 오래 기다리게 했다!",
            "포켓몬스터의 세계에 잘 왔단다!",
            "나의 이름은 오박사.",
            "모두로부터 포켓몬 박사로 존경받고 있단다!"
        };

        // 초기 상태 설정
        whiteFlash.alpha = 0f;
        whiteFlash.gameObject.SetActive(false);
        oakImage.SetActive(false);
        simhyangImage.SetActive(false);
        marillImage.SetActive(false);
        playerSprite.SetActive(false);
        textBox.SetActive(false);
        dialogueText.gameObject.SetActive(false);
        continueIcon.SetActive(false);
        nameSelectPanel.SetActive(false);
        panelArrow.SetActive(false);

        StartCoroutine(IntroSequence());
    }

    void Update()
    {
        // 이름 선택 중 방향키 입력 처리
        if (isChoosingName)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectedIndex = Mathf.Max(0, selectedIndex - 1);
                UpdateArrowPosition();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedIndex = Mathf.Min(nameOptions.Length - 1, selectedIndex + 1);
                UpdateArrowPosition();
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                isChoosingName = false;
                panelArrow.SetActive(false);

                string selected = nameOptions[selectedIndex].text;
                if (selected.Contains("스스로"))
                    StartCoroutine(AskForNameInput());
                else
                    StartCoroutine(SetNameAndContinue(selected));
            }
        }

        // 키보드 입력 받기
        if (inputtingName)
        {
            foreach (char c in Input.inputString)
            {
                if (c == '\b' && nameBuilder.Length > 0)
                {
                    nameBuilder.Length--;
                }
                else if ((c == '\n' || c == '\r') && nameBuilder.Length > 0)
                {
                    inputtingName = false;
                    StartCoroutine(SetNameAndContinue(nameBuilder.ToString()));
                }
                else if (char.IsLetterOrDigit(c) && nameBuilder.Length < 8)
                {
                    nameBuilder.Append(c);
                }
            }
            inputNameDisplay.text = nameBuilder.ToString();
        }
    }

    IEnumerator IntroSequence()
    {
        yield return StartCoroutine(FlashScreen());
        yield return StartCoroutine(FlickerOak());
        yield return StartCoroutine(ShowTextBox());

        foreach (string line in dialogues)
        {
            yield return StartCoroutine(TypeDialogue(line));
        }

        yield return StartCoroutine(ShowSimhyangAndAskName());
    }

    IEnumerator FlashScreen()
    {
        whiteFlash.gameObject.SetActive(true);
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 8f;
            whiteFlash.alpha = t;
            yield return null;
        }

        yield return new WaitForSeconds(0.15f);

        while (t > 0f)
        {
            t -= Time.deltaTime * 6f;
            whiteFlash.alpha = t;
            yield return null;
        }

        whiteFlash.alpha = 0f;
        whiteFlash.gameObject.SetActive(false);
    }

    IEnumerator FlickerOak()
    {
        oakImage.SetActive(true);
        Image img = oakImage.GetComponent<Image>();
        Color c = img.color;

        for (int i = 0; i < 3; i++)
        {
            c.a = 0f; img.color = c;
            yield return new WaitForSeconds(0.1f);
            c.a = 1f; img.color = c;
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator ShowTextBox()
    {
        textBox.SetActive(true);
        dialogueText.gameObject.SetActive(true);

        RectTransform rt = textBox.GetComponent<RectTransform>();
        CanvasGroup cg = textBox.GetComponent<CanvasGroup>();
        if (cg == null) cg = textBox.AddComponent<CanvasGroup>();

        rt.localScale = Vector3.zero;
        cg.alpha = 0f;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 3f;
            rt.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            cg.alpha = t;
            yield return null;
        }

        rt.localScale = Vector3.one;
        cg.alpha = 1f;
    }

    IEnumerator TypeDialogue(string line)
    {
        if (isTyping) yield break;
        isTyping = true;

        dialogueText.text = "";
        continueIcon.SetActive(false);
        dialogueText.gameObject.SetActive(true);

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < line.Length; i++)
        {
            sb.Append(line[i]);
            dialogueText.text = sb.ToString();
            yield return new WaitForSeconds(0.05f);
        }

        continueIcon.SetActive(true);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
        continueIcon.SetActive(false);

        isTyping = false;
    }

    IEnumerator ShowSimhyangAndAskName()
    {
        oakImage.SetActive(false);
        marillImage.SetActive(false);
        simhyangImage.SetActive(true);

        yield return StartCoroutine(TypeDialogue("그럼... 슬슬 너의 이름을 가르쳐다오!"));

        // 왼쪽으로 이동
        RectTransform rt = simhyangImage.GetComponent<RectTransform>();
        rt.anchoredPosition += new Vector2(-150f, 0);

        nameSelectPanel.SetActive(true);
        panelArrow.SetActive(true);
        selectedIndex = 0;
        UpdateArrowPosition();
        isChoosingName = true;
    }

    void UpdateArrowPosition()
    {
        RectTransform arrowRT = panelArrow.GetComponent<RectTransform>();
        RectTransform targetRT = nameOptions[selectedIndex].GetComponent<RectTransform>();
        arrowRT.anchoredPosition = new Vector2(arrowRT.anchoredPosition.x, targetRT.anchoredPosition.y);
    }

    // 사용자 직접 입력
    private bool inputtingName = false;
    private StringBuilder nameBuilder = new StringBuilder();

    IEnumerator AskForNameInput()
    {
        nameSelectPanel.SetActive(false);
        inputNameDisplay.text = "";
        inputtingName = true;
        yield return null;
    }

    IEnumerator SetNameAndContinue(string name)
    {
        chosenName = name;
        yield return StartCoroutine(TypeDialogue($"{chosenName}! 준비는 되었는가?"));
        yield return StartCoroutine(TypeDialogue("드디어 이제부터 너의 이야기가 시작된다."));
        yield return StartCoroutine(TypeDialogue("즐거운 일도 괴로운 일도 잔뜩 너를 기다리고 있을 것이다!"));
        yield return StartCoroutine(TypeDialogue("꿈과 모험과 포켓몬스터의 세계에!"));
        yield return StartCoroutine(TypeDialogue("렛츠고!"));

        simhyangImage.SetActive(false);
        playerSprite.SetActive(true);

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Home");
    }
}