using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroDialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public TimeSelectorController timeSelector;
    public GameObject arrowIcon;

    private List<string> dialogues;
    private int dialogueIndex = 0;
    private bool isTyping = false;
    private bool waitingForInput = false;
    private bool dialogueEnded = false;
    private string currentFormattedLine = "";

    void Start()
    {
        dialogues = new List<string>
        {
            "...............",
            "움- 움냐 음냐.........",
            "뭐야 벌써 시간이? 미안하지만 시간을 좀 봐주겠니?",
            "지금은 몇시인가?",
            "그래서 몇 분이라고?",
            "뭐야, {0}분?!",
            "{1}시 {0}분이라니 이런... 마냥 잠만 잔 것 같군!"
        };

        arrowIcon.SetActive(false);
        timeSelector.Deactivate();
        StartCoroutine(TypeText(dialogues[dialogueIndex]));
    }

    void Update()
    {
        // 타자기 출력 중 → Enter로 즉시 전체 출력
        if (isTyping && Input.GetKeyDown(KeyCode.Return))
        {
            StopAllCoroutines();
            dialogueText.text = currentFormattedLine;
            isTyping = false;
            waitingForInput = true;
            arrowIcon.SetActive(true);
            return;
        }

        // 대사 출력 완료 상태에서 Enter → 다음 처리
        if (waitingForInput && Input.GetKeyDown(KeyCode.Return))
        {
            waitingForInput = false;
            arrowIcon.SetActive(false);

            if (dialogueIndex == 3)
            {
                timeSelector.Activate(TimeSelectorController.TimeMode.Hour);
                timeSelector.onSelectionComplete = () =>
                {
                    dialogueIndex++;
                    StartCoroutine(TypeText(dialogues[dialogueIndex]));
                };
                return;
            }

            // 분 선택 완료 → "뭐야, {0}분?!" 출력
            if (dialogueIndex == 4)
            {
                timeSelector.Activate(TimeSelectorController.TimeMode.Minute);
                timeSelector.onSelectionComplete = () =>
                {
                    dialogueIndex++; // == 5
                    string line = string.Format(dialogues[dialogueIndex], timeSelector.GetMinute());
                    dialogueIndex++; // == 6 로 이동해서 중복 방지
                    waitingForInput = true;
                    StartCoroutine(TypeText(line));
                };
                return;
            }


            if (dialogueIndex == 5)
            {
                string line = string.Format(dialogues[dialogueIndex], timeSelector.GetMinute());
                dialogueIndex++;
                StartCoroutine(TypeText(line));
                return;
            }

            if (dialogueIndex == 6)
            {
                string line = string.Format(dialogues[dialogueIndex], timeSelector.GetMinute(), timeSelector.GetHour());
                dialogueIndex++;
                StartCoroutine(TypeText(line));
                dialogueEnded = true;
                return;
            }

            if (dialogueEnded)
            {
                SceneManager.LoadScene("OakScene2");
                return;
            }

            dialogueIndex++;
            if (dialogueIndex < dialogues.Count)
            {
                StartCoroutine(TypeText(dialogues[dialogueIndex]));
            }
        }
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        currentFormattedLine = text;
        dialogueText.text = "";

        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.05f);
        }

        isTyping = false;
        waitingForInput = true;
        arrowIcon.SetActive(true);
    }
}
