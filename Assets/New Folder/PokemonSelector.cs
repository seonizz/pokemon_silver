using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class PokemonSelector : MonoBehaviour
{
    public enum PokemonType { Fire, Water, Grass }

    [Header("타입")]
    public PokemonType pokemonType;

    [Header("UI")]
    public GameObject dialogueBox;
    public TMP_Text dialogueText;
    public GameObject nextArrowIcon;

    public GameObject yesNoPanel;
    public RectTransform yesNoCursor;

    [Header("미리보기")]
    public Sprite previewSprite; 

    [Header("다른 포켓몬 볼 오브젝트들")]
    public GameObject fireBallObject;
    public GameObject waterBallObject;
    public GameObject grassBallObject;

    [Header("플레이어")]
    public move2 playerMoveScript;

    [Header("미리보기 크기 설정")]
    public float previewScale = 3f;
    public float maxPreviewSize = 512f;

    [Header("이 볼의 포켓몬 데이터")]
    public PokemonData pokemonData; 

    [Header("Yes/No 커서 고정 좌표")]
    public Vector2 yesPos = new Vector2(-62.3f, 55f);
    public Vector2 noPos = new Vector2(-62.3f, 17.8f);

    // 내부 상태
    private string pokemonName;
    private bool playerInRange = false;
    private bool isSelecting = false;
    public static bool pokemonHasBeenChosen = false;
    private bool yesNoResult;
    private static bool globalSelecting = false;

    private static Canvas previewCanvas;   
    private Image runtimePreviewImage;   

    void Awake()
    {
        
        pokemonHasBeenChosen = false;

        if (previewSprite != null && previewSprite.texture != null)
            previewSprite.texture.filterMode = FilterMode.Point;
    }

    void Start()
    {
        switch (pokemonType)
        {
            case PokemonType.Fire: pokemonName = "브케인"; break;
            case PokemonType.Water: pokemonName = "리아코"; break;
            case PokemonType.Grass: pokemonName = "치코리타"; break;
        }

        if (yesNoPanel != null) yesNoPanel.SetActive(false);
        if (dialogueBox != null) dialogueBox.SetActive(false);
        if (nextArrowIcon != null) nextArrowIcon.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && !pokemonHasBeenChosen && !isSelecting && !globalSelecting &&
            (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return)))
        {
            isSelecting = true;
            globalSelecting = true;
            StartCoroutine(SelectionSequence());
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInRange = false;
    }

    void OnMouseDown()
    {
        if (playerInRange && !pokemonHasBeenChosen && !isSelecting && !globalSelecting)
        {
            isSelecting = true;
            globalSelecting = true;
            StartCoroutine(SelectionSequence());
        }
    }

    private IEnumerator SelectionSequence()
    {
        Debug.Log($"[Selector:{name}] type={pokemonType}, preview={(previewSprite ? previewSprite.name : "null")}");

        if (playerMoveScript != null) playerMoveScript.enabled = false;
        if (dialogueBox != null) dialogueBox.SetActive(true);

        ShowPreviewCentered(previewSprite);

        if (dialogueText != null)
            dialogueText.text = $"{pokemonName}을/를 고를 거니?";

        try
        {
            yield return StartCoroutine(YesNoChoice());
            bool choseYes = yesNoResult;

            HidePreview();

            if (yesNoPanel != null) yesNoPanel.SetActive(false);

            if (choseYes)
            {
                pokemonHasBeenChosen = true;
                TransitionData.playerPokemon = pokemonData;
                PlayerPrefs.SetString("ChosenPokemonType", pokemonType.ToString());
                string playerName = PlayerPrefs.GetString("PlayerName", "골드");

                yield return TypeDialogue("좋다! 이 포켓몬들과 너는 이제 한마음 한뜻으로");
                yield return TypeDialogue("모험을 계속하게 된다!");
                yield return TypeDialogue($"골드은/는 {pokemonName}을/를 받았다!");
                yield return TypeDialogue("무궁시티까진 외길이니 찾기 쉬울 것이다!");
                yield return TypeDialogue("무슨 일 있으면 연락하여라!");

                gameObject.SetActive(false);

            }
        }
        finally
        {
            isSelecting = false;
            globalSelecting = false;

            if (dialogueBox != null) dialogueBox.SetActive(false);
            if (playerMoveScript != null) playerMoveScript.enabled = true;
        }
    }

    private IEnumerator YesNoChoice()
    {
        if (yesNoPanel != null)
        {
            var panelCanvas = yesNoPanel.GetComponentInParent<Canvas>();
            if (panelCanvas == null)
            {
                if (previewCanvas != null)
                    yesNoPanel.transform.SetParent(previewCanvas.transform, false);
            }
            else
            {
  
                if (previewCanvas != null)
                    panelCanvas.sortingOrder = Mathf.Max(panelCanvas.sortingOrder, previewCanvas.sortingOrder + 1);
                else
                    panelCanvas.sortingOrder = 1000; 
            }

            yesNoPanel.SetActive(true);
        }

        bool choosingYes = true;

        if (yesNoCursor != null)
            yesNoCursor.anchoredPosition = yesPos;

        yield return null;

        while (true)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                choosingYes = !choosingYes;
                if (yesNoCursor != null)
                    yesNoCursor.anchoredPosition = choosingYes ? yesPos : noPos;
            }

            if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return))
            {
                yesNoResult = choosingYes;
                break;
            }
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.X)) 
            {
                yesNoResult = false;
                break;
            }

            yield return null;
        }
    }

    private void ShowPreviewCentered(Sprite s)
    {
        if (s == null) return;

        if (previewCanvas == null)
        {
            var go = new GameObject("PreviewCanvas");
            previewCanvas = go.AddComponent<Canvas>();
            previewCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            previewCanvas.sortingOrder = 100; 
            go.AddComponent<CanvasScaler>();
            go.AddComponent<GraphicRaycaster>();
        }

        if (runtimePreviewImage == null)
        {
            var imgGO = new GameObject("PreviewImage");
            imgGO.transform.SetParent(previewCanvas.transform, false);
            runtimePreviewImage = imgGO.AddComponent<Image>();

            var rt = runtimePreviewImage.rectTransform;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
        }

        if (s.texture != null) s.texture.filterMode = FilterMode.Point;

        runtimePreviewImage.sprite = s;
        runtimePreviewImage.SetNativeSize();

        var rt2 = runtimePreviewImage.rectTransform;


        rt2.sizeDelta *= Mathf.Max(1f, previewScale);

        float sx = Mathf.Min(1f, maxPreviewSize / Mathf.Max(1f, rt2.sizeDelta.x));
        float sy = Mathf.Min(1f, maxPreviewSize / Mathf.Max(1f, rt2.sizeDelta.y));
        float clampScale = Mathf.Min(sx, sy);
        if (clampScale < 1f) rt2.sizeDelta *= clampScale;

        runtimePreviewImage.gameObject.SetActive(true);
        runtimePreviewImage.transform.SetAsLastSibling();
    }

    private void HidePreview()
    {
        if (runtimePreviewImage != null)
            runtimePreviewImage.gameObject.SetActive(false);
    }

    private IEnumerator TypeDialogue(string line)
    {
        if (dialogueText == null) yield break;

        dialogueText.text = line;
        dialogueText.ForceMeshUpdate();
        int total = dialogueText.textInfo.characterCount;

        dialogueText.maxVisibleCharacters = 0;

        for (int i = 0; i <= total; i++)
        {
            dialogueText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(0.04f);
        }

        if (nextArrowIcon != null) nextArrowIcon.SetActive(true);
        yield return StartCoroutine(WaitForPlayerInput());
        if (nextArrowIcon != null) nextArrowIcon.SetActive(false);
    }

    private IEnumerator WaitForPlayerInput()
    {
        yield return new WaitUntil(() => !Input.GetKey(KeyCode.Z) && !Input.GetKey(KeyCode.Return));
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return));
    }

    void OnDisable()
    {
        isSelecting = false;
        globalSelecting = false;
    }

    void OnDestroy()
    {
        isSelecting = false;
        globalSelecting = false;
    }
}
