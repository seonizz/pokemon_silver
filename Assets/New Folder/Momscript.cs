using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Momscript : MonoBehaviour
{
    [Header("이동 세팅")]
    public float moveSpeed = 3f;
    private Vector2 startPosition;


    [Header("애니메이션 스프라이트")]
    public Sprite[] downSprites;
    public Sprite[] upSprites;
    public Sprite[] leftSprites;
    public Sprite[] rightSprites;

    [Header("대사 및 UI")]
    public move playerMoveScript;
    public GameObject dialogueBox;
    public TMP_Text dialogueText;
    public GameObject daySelectionPanel;
    public TMP_Text dayText;
    public GameObject nextArrowIcon;
    public GameObject daySelectionArrows;



    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private bool isInteracting = false;
    private float animTimer = 0f;
    private int animIndex = 0;
    private Sprite[] currentAnim;
    private string selectedDay = "일요일";
    private readonly string[] daysOfWeek = { "일요일", "월요일", "화요일", "수요일", "목요일", "금요일", "토요일" };

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

   
        rb.bodyType = RigidbodyType2D.Dynamic; 

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
 
    }
    void Start()
    {

        dialogueBox.SetActive(false);
        daySelectionPanel.SetActive(false);
        if (daySelectionArrows != null) daySelectionArrows.SetActive(false);

        startPosition = new Vector2(2.8f, 0.7f);
        transform.position = startPosition;
        rb.position = transform.position;

        currentAnim = downSprites;
        if (downSprites != null && downSprites.Length > 0)
        {
            sr.sprite = downSprites[0];
        }

        StartCoroutine(FullSequence());
    }

    void Update()
    {
        if (isInteracting)
        {
            AnimateStep();
        }
    }

    private IEnumerator FullSequence()
    {
        if (playerMoveScript != null) playerMoveScript.enabled = false; //
        yield return StartCoroutine(MoveTo(new Vector2(5f, 2.9f)));
        yield return StartCoroutine(DialogueSequence());
        yield return StartCoroutine(MoveTo(startPosition, true));
        Debug.Log("엄마행동끗");
        if (playerMoveScript != null) playerMoveScript.enabled = true;
    }

    private IEnumerator MoveTo(Vector2 destination, bool moveBack = false)
    {
        if (isInteracting) yield break;
        isInteracting = true;


        Vector2 xTarget = new Vector2(destination.x, rb.position.y);
        Vector2 xDirection = destination.x > rb.position.x ? Vector2.right : Vector2.left;
        currentAnim = GetSpriteArray(xDirection);
        while (Mathf.Abs(rb.position.x - destination.x) > 0.01f)
        {
            Vector2 newPos = Vector2.MoveTowards(rb.position, xTarget, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
            yield return new WaitForFixedUpdate();
        }

        Vector2 yTarget = destination;
        Vector2 yDirection = destination.y > rb.position.y ? Vector2.up : Vector2.down;
        currentAnim = GetSpriteArray(yDirection);
        while (Mathf.Abs(rb.position.y - destination.y) > 0.01f)
        {
            Vector2 newPos = Vector2.MoveTowards(rb.position, yTarget, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(destination);
        isInteracting = false;

        if (!moveBack)
        {
            currentAnim = upSprites;
            if (currentAnim != null && currentAnim.Length > 0) sr.sprite = currentAnim[0];
        }
    }

    private IEnumerator DialogueSequence()
    {
        isInteracting = true;
        string playerName = PlayerPrefs.GetString("playerName");


        yield return TypeDialogue($"아 골드!");
        yield return TypeDialogue("옆집의 공박사님이 찾아왔었단다");
        yield return TypeDialogue("뭔지 너에게 부탁할 것이 있다고 하셔서");
        yield return TypeDialogue("그래! 잊어먹을 뻔 했네 수리를 보냈던");
        yield return TypeDialogue("포켓기어가 돌아왔단다");
        yield return TypeDialogue("여기!");
        yield return TypeDialogue($"골드은/는 포켓기어를 얻었다!");
        yield return TypeDialogue("포켓몬기어\n줄여서 포켓기어");
        yield return TypeDialogue("훌륭한 트레이너라면 가지고 있지 않아선 안 될 걸");
        yield return TypeDialogue("엥? 요일을 맞추지 않았네");
        yield return TypeDialogue("사용하기 전에 오늘은 무슨 요일인가 정해놓지 않으면");
        yield return TypeDialogue("안 되겠지?");
        yield return TypeDialogue("오늘은 무슨 요일?");

        yield return StartCoroutine(SelectDaySequence());

        yield return TypeDialogue($"{selectedDay}, 설마 틀리진 않았겠지?");
        yield return TypeDialogue("그래 그래 전화의 사용방법도 기억하겠지?");

        dialogueBox.SetActive(false);
        isInteracting = false;
    }


    private IEnumerator TypeDialogue(string line)
    {
        dialogueBox.SetActive(true);
        dialogueText.text = "";
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.05f);
        }
        nextArrowIcon.SetActive(true);

        yield return StartCoroutine(WaitForPlayerInput());

        nextArrowIcon.SetActive(false);
    }

    private IEnumerator WaitForPlayerInput()
    {

        yield return new WaitUntil(() => !Input.GetKey(KeyCode.Z) && !Input.GetKey(KeyCode.Return));

        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return));
    }

    private IEnumerator SelectDaySequence()
    {
        daySelectionPanel.SetActive(true);

        if (daySelectionArrows != null) daySelectionArrows.SetActive(true);
        yield return new WaitUntil(() => !Input.GetKey(KeyCode.Z) && !Input.GetKey(KeyCode.Return));

        int currentIndex = 0;
 
        for (int i = 0; i < daysOfWeek.Length; i++) { if (daysOfWeek[i] == selectedDay) { currentIndex = i; break; } }
        dayText.text = daysOfWeek[currentIndex];

        bool isDaySelected = false;
        while (!isDaySelected)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentIndex = (currentIndex + daysOfWeek.Length - 1) % daysOfWeek.Length;
                dayText.text = daysOfWeek[currentIndex];
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentIndex = (currentIndex + 1) % daysOfWeek.Length;
                dayText.text = daysOfWeek[currentIndex];
            }
            else if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return))
            {
                selectedDay = daysOfWeek[currentIndex];
                isDaySelected = true;
            }
            yield return null;
        }

        daySelectionPanel.SetActive(false);

        if (daySelectionArrows != null) daySelectionArrows.SetActive(false);
    }


    void AnimateStep()
    {
        if (currentAnim == null || currentAnim.Length <= 1) return;
        animTimer += Time.deltaTime;
        if (animTimer >= 0.15f)
        {
            animIndex = (animIndex + 1) % currentAnim.Length;
            sr.sprite = currentAnim[animIndex];
            animTimer = 0f;
        }
    }

    Sprite[] GetSpriteArray(Vector2 dir)
    {
        if (dir.x > 0) return rightSprites;
        if (dir.x < 0) return leftSprites;
        if (dir.y > 0) return upSprites;
        if (dir.y < 0) return downSprites;
        return currentAnim;
    }
}