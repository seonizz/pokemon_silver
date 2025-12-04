using UnityEngine;
using System.Collections;
using TMPro;

public class ProfessorDialogue : MonoBehaviour
{
    [Header("대사 및 UI")]
    public GameObject dialogueBox;
    public TMP_Text dialogueText;
    public GameObject nextArrowIcon;
    public move2 playerMoveScript; 

    private bool playerInRange = false; 
    private bool dialogueFinished = false;


    private void OnTriggerEnter2D(Collider2D other)

    {
        if (other.CompareTag("Player"))

        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    void Update()
    {
   
        if (playerInRange && !dialogueFinished && (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return)))
        {
            StartCoroutine(DialogueSequence());
        }
    }
    void Start()
    {

        if (dialogueBox != null)
        {
            dialogueBox.SetActive(false);
        }
        if (nextArrowIcon != null)
        {
            nextArrowIcon.SetActive(false);
        }
    }

    private IEnumerator DialogueSequence()
    {
        dialogueFinished = true; 
        if (nextArrowIcon != null) nextArrowIcon.SetActive(false); 
        if (dialogueBox != null) dialogueBox.SetActive(false); 

        if (playerMoveScript != null) playerMoveScript.enabled = true;

        string playerName = PlayerPrefs.GetString("playerName", "친구");

        yield return TypeDialogue($"어여 골드군, 기다리고 있었단다!");
        yield return TypeDialogue("오늘 너를 부른 것은 부탁이 있어서란다!");
        yield return TypeDialogue("다름이 아니라 무궁시티에 가서");
        yield return TypeDialogue("포켓몬 아저씨에게 규토리볼을 전해줬으면 한다!");
        yield return TypeDialogue("잠깐!! 앞으로 헤쳐나갈 숲속에는");
        yield return TypeDialogue("강하고 위험한 포켓몬들이 정말 많다!");
        yield return TypeDialogue("물론 파트너가 될 포켓몬을 주겠다!");
        yield return TypeDialogue("최근에 발견한 진귀한 포켓몬이란다");

        dialogueBox.SetActive(true);
        dialogueText.text = "";
        string lastLine = "자 고르거라!";
        foreach (char c in lastLine.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.05f);
        }

        if (playerMoveScript != null) playerMoveScript.enabled = true;

    }



    private IEnumerator TypeDialogue(string line)
    {
        dialogueBox.SetActive(true);
        dialogueText.text = "";
        foreach (char c in line.ToCharArray())
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
}