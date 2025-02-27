using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogueUIManager : MonoBehaviour
{
    public GameObject dialoguePanel;
    public GameObject mainPanel;
    public TMP_Text speakerText;
    public TMP_Text dialogueText;

    private DialogueData currentDialogue;
    private int currentDialogueIndex = 0;
    private bool isTyping = false;

    void Start()
    {
        dialoguePanel.SetActive(false);
    }

    // 📌 대화 시작 함수 (예외 처리 추가)
    public void StartDialogue(DialogueData dialogue)
    {
        if (dialogue == null || dialogue.entries.Count == 0)
        {
            Debug.LogError("❌ 잘못된 대화 데이터입니다.");
            return;
        }

        currentDialogue = dialogue;
        currentDialogueIndex = 0;
        dialoguePanel.SetActive(true);
        StartCoroutine(DisplayDialogue());
    }

    // 📌 대사 출력 (타이핑 효과 적용)
    IEnumerator DisplayDialogue()
    {
        isTyping = true;

        if (currentDialogueIndex < 0 || currentDialogueIndex >= currentDialogue.entries.Count)
        {
            Debug.LogError("❌ 대화 인덱스 오류!");
            EndDialogue();
            yield break;
        }

        DialogueEntry entry = currentDialogue.entries[currentDialogueIndex];

        speakerText.text = entry.speaker;
        dialogueText.text = "";

        foreach (char letter in entry.text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.03f);
        }

        isTyping = false;
    }

    // 📌 키 입력 처리 (D키로 다음 대사)
    void Update()
    {
        if (!dialoguePanel.activeSelf) return;
        if (Input.GetKeyDown(KeyCode.D) && !isTyping) NextDialogue();
    }

    // 📌 다음 대사 진행
    public void NextDialogue()
    {
        if (isTyping) return; // 🚨 타이핑 중에는 입력 방지

        if (currentDialogueIndex < currentDialogue.entries.Count - 1)
        {
            currentDialogueIndex++;
            StartCoroutine(DisplayDialogue());
        }
        else
        {
            EndDialogue();
        }
    }

    // 📌 대화 종료 시 처리
    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);

        // ✅ 대화 완료 상태 업데이트
        DialogueManager.CompleteDialogue(currentDialogue.dialogueId);

        // ✅ 메인 메뉴로 돌아가기
        mainPanel.SetActive(true);
    }
}
