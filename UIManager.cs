using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public GameObject menuPanel; // 메뉴 UI
    public GameObject dialogueListPanel; // 대화 목록 UI
    public GameObject messagePanel; // ✅ 새로 추가된 메시지 패널
    public TMP_Text messageText; // ✅ 메시지 패널의 텍스트
    public List<TMP_Text> dialogueListItems;

    private int dialogueIndex = 0;
    private List<DialogueData> dialoguesForCharacter;

    void Start()
    {
        ShowMenu();
    }

    public void ShowMenu()
    {
        menuPanel.SetActive(true);
        dialogueListPanel.SetActive(false);
    }

    public void ShowDialogueList()
    {
        menuPanel.SetActive(false); // ✅ 메인 메뉴 숨김
        dialogueListPanel.SetActive(true); // ✅ 대화 목록 표시

        dialoguesForCharacter = DialogueManager.loadedDialogues;
        UpdateDialogueList();

        dialogueIndex = FindFirstSelectableDialogue(); // ✅ 첫 번째 선택 가능한 대화 찾기

        if (dialogueIndex == -1) // ✅ 선택 가능한 대화가 없으면 메시지 출력
        {
            ShowMessage("더 이상 대화할 수 있는 것이 없습니다.");
            return;
        }

        UpdateDialogueSelection(); // ✅ UI 업데이트
    }

    void ShowMessage(string message)
    {
        dialogueListPanel.SetActive(false); // ✅ 메시지 표시 중에는 대화 목록 패널 숨김
        messagePanel.SetActive(true); // ✅ 메시지 패널 활성화
        StartCoroutine(TypeMessage(message)); // ✅ 한 글자씩 출력
    }

    IEnumerator TypeMessage(string message)
    {
        messageText.text = ""; // ✅ 텍스트 초기화

        foreach (char letter in message)
        {
            messageText.text += letter;
            yield return new WaitForSeconds(0.03f); // ✅ 글자 출력 속도 (0.03초 간격)
        }

        yield return new WaitForSeconds(0.6f); // ✅ 모든 글자 출력 후 대기 시간
        messagePanel.SetActive(false); // ✅ 메시지 패널 비활성화
        menuPanel.SetActive(true); // ✅ 메인 메뉴 복귀
    }

    int FindFirstSelectableDialogue()
    {
        for (int i = 0; i < dialoguesForCharacter.Count; i++)
        {
            if (dialoguesForCharacter[i].isUnlocked && !dialoguesForCharacter[i].isCompleted)
            {
                return i; // ✅ 첫 번째 선택 가능한 대화의 인덱스 반환
            }
        }
        return -1; // ✅ 선택 가능한 대화가 없으면 -1 반환
    }

    void UpdateDialogueList()
    {
        for (int i = 0; i < dialogueListItems.Count; i++)
        {
            if (i < dialoguesForCharacter.Count)
            {
                var dialogue = dialoguesForCharacter[i];
                dialogueListItems[i].text = dialogue.isUnlocked ? dialogue.dialogueTitle : "???"; // ✅ 잠긴 대화는 "???"
                dialogueListItems[i].color = dialogue.isCompleted ? Color.gray : Color.black; // ✅ 완료된 대사는 회색
            }
            else
            {
                dialogueListItems[i].text = ""; // ✅ 리스트 길이보다 대사가 적을 경우 빈 문자열 처리
            }
        }
    }

    void SelectDialogue()
    {
        if (dialogueIndex == -1) return; // ✅ 선택 가능한 대화가 없으면 실행 안 함

        Debug.Log("🗨 대화 시작: " + dialoguesForCharacter[dialogueIndex].dialogueTitle);
        FindFirstObjectByType<DialogueUIManager>().StartDialogue(dialoguesForCharacter[dialogueIndex]);
        dialogueListPanel.SetActive(false); // ✅ 대화 시작 후 대화 목록 닫기
    }

    void CloseDialogueList()
    {
        dialogueListPanel.SetActive(false);
        ShowMenu(); // ✅ 대화 목록 닫기 후 메인 메뉴로 복귀
    }

    // ✅ 저장 버튼 UI 이벤트 추가
    public void SaveSlot1() => DialogueManager.SaveGame(1); // ✅ 슬롯 1에 저장
    public void SaveSlot2() => DialogueManager.SaveGame(2); // ✅ 슬롯 2에 저장
    public void SaveSlot3() => DialogueManager.SaveGame(3); // ✅ 슬롯 3에 저장

    // ✅ 불러오기 버튼 UI 이벤트 추가
    public void LoadSlot1() => DialogueManager.LoadGame(1); // ✅ 슬롯 1에서 불러오기
    public void LoadSlot2() => DialogueManager.LoadGame(2); // ✅ 슬롯 2에서 불러오기
    public void LoadSlot3() => DialogueManager.LoadGame(3); // ✅ 슬롯 3에서 불러오기
}
