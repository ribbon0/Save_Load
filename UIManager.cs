using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public GameObject menuPanel; // 메뉴 UI
    public GameObject dialogueListPanel; // 대화 목록 UI
    public GameObject messagePanel;       // ✅ 새로 추가된 메시지 패널
    public TMP_Text messageText;          // ✅ 메시지 패널의 텍스트
    public List<TMP_Text> dialogueListItems;
    private int dialogueIndex = 0;
    private List<DialogueData> dialoguesForCharacter;
    
    void Start()
    {
        ShowMenu();
    }

    void Update()
    {
        if (dialogueListPanel.activeSelf)
            HandleDialogueListNavigation();
    }

    public void ShowMenu()
    {
        menuPanel.SetActive(true);
        dialogueListPanel.SetActive(false);
    }

    public void ShowDialogueList()
    {
        menuPanel.SetActive(false);  // ✅ 메인 메뉴 숨김
        dialogueListPanel.SetActive(true);  // ✅ 대화 목록 표시

        Debug.Log("🔄 현재 씬: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        Debug.Log("🔄 JSON에서 로드된 대화 개수: " + DialogueManager.loadedDialogues.Count);

        dialoguesForCharacter = DialogueManager.loadedDialogues;
        UpdateDialogueList();

        // ✅ 첫 번째 선택 가능한 대화로 커서 이동 (없으면 `-1`)
        dialogueIndex = FindFirstSelectableDialogue();

        // ✅ 선택 가능한 대화가 없을 경우 메시지 표시 후 자동 복귀
        if (dialogueIndex == -1)
        {
            ShowMessage("더 이상 대화할 수 있는 것이 없습니다.");
            return;
        }

        UpdateDialogueSelection();
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
            yield return new WaitForSeconds(0.03f); // ✅ 글자 출력 속도 (0.05초 간격)
        }

        yield return new WaitForSeconds(0.6f); // ✅ 모든 글자 출력 후 대기 시간
        messagePanel.SetActive(false); // ✅ 메시지 패널 비활성화
        menuPanel.SetActive(true); // ✅ 메인 메뉴 복귀
    }


    // ✅ 일정 시간이 지나면 메시지 패널을 닫고 메인 메뉴로 복귀하는 함수
    IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        messagePanel.SetActive(false);
        menuPanel.SetActive(true); // ✅ 메인 메뉴 복귀
    }

    // 📌 ✅ 첫 번째 선택 가능한 대화를 찾는 함수 수정 (잠긴 대화 `???`도 제외)
    int FindFirstSelectableDialogue()
    {
        for (int i = 0; i < dialoguesForCharacter.Count; i++)
        {
            if (dialoguesForCharacter[i].isUnlocked && !dialoguesForCharacter[i].isCompleted)
            {
                Debug.Log("🟢 선택 가능한 대화 발견: " + dialoguesForCharacter[i].dialogueTitle);
                return i; // ✅ 첫 번째 선택 가능한 대화 인덱스 반환
            }
        }

        Debug.LogWarning("🚨 선택 가능한 대화가 없음! (-1 반환)");
        return -1; // ✅ 선택 가능한 대화가 없으면 -1 반환
    }




    void UpdateDialogueList()
    {
        for (int i = 0; i < dialogueListItems.Count; i++)
        {
            if (i < dialoguesForCharacter.Count)
            {
                var dialogue = dialoguesForCharacter[i];
                dialogueListItems[i].text = dialogue.isUnlocked ? dialogue.dialogueTitle : "???";
                dialogueListItems[i].color = dialogue.isCompleted ? Color.gray : Color.black;
            }
            else
            {
                dialogueListItems[i].text = "";
            }
            
        }
        
    }


    void HandleDialogueListNavigation()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) ChangeSelection(-1);
        else if (Input.GetKeyDown(KeyCode.DownArrow)) ChangeSelection(1);
        else if (Input.GetKeyDown(KeyCode.D)) SelectDialogue();
        else if (Input.GetKeyDown(KeyCode.F)) CloseDialogueList();
    }

    void ChangeSelection(int direction)
    {
        if (dialogueIndex == -1) return; // ✅ 선택 가능한 대화가 없으면 이동 차단

        int newIndex = dialogueIndex;
        int safetyCounter = 0; // ✅ 무한 루프 방지 카운터

        do
        {
            newIndex = (newIndex + direction + dialogueListItems.Count) % dialogueListItems.Count;
            safetyCounter++;

            if (safetyCounter > dialogueListItems.Count) // ✅ 모든 항목이 잠겨 있으면 루프 중단
            {
                Debug.LogWarning("🚨 모든 대화가 잠겨 있음! 선택지 이동 중단");
                return;
            }
        }
        while (!dialoguesForCharacter[newIndex].isUnlocked || dialoguesForCharacter[newIndex].isCompleted);

        dialogueIndex = newIndex;
        UpdateDialogueSelection();
    }





    void UpdateDialogueSelection()
    {
        for (int i = 0; i < dialogueListItems.Count; i++)
        {
            if (i < dialoguesForCharacter.Count)
            {
                var dialogue = dialoguesForCharacter[i];

                // ✅ 선택할 수 있는 대화가 없으면 커서 숨김
                string prefix = (dialogueIndex == -1) ? "  " : ((i == dialogueIndex) ? "▶ " : "  ");
                dialogueListItems[i].text = prefix + (dialogue.isUnlocked ? dialogue.dialogueTitle : "???");

                // ✅ 기본 색상: 검정 (Black), 선택된 항목은 노랑 (Yellow), `???` 및 완료된 대화는 회색 (Gray)
                if (!dialogue.isUnlocked || dialogue.isCompleted)
                {
                    dialogueListItems[i].color = Color.gray;
                }
                else
                {
                    dialogueListItems[i].color = (i == dialogueIndex) ? Color.yellow : Color.black;
                }

                dialogueListItems[i].ForceMeshUpdate(); // ✅ 강제 업데이트
            }
        }
    }






    void SelectDialogue()
    {
        if (dialogueIndex == -1)
        {
            Debug.LogWarning("🚫 선택 가능한 대화가 없음!");
            return;
        }

        if (dialogueIndex < 0 || dialogueIndex >= dialoguesForCharacter.Count || !dialoguesForCharacter[dialogueIndex].isUnlocked || dialoguesForCharacter[dialogueIndex].isCompleted)
        {
            Debug.Log("🚫 선택할 수 없는 대화입니다.");
            return;
        }

        Debug.Log("🗨 대화 시작: " + dialoguesForCharacter[dialogueIndex].dialogueTitle);
        FindFirstObjectByType<DialogueUIManager>().StartDialogue(dialoguesForCharacter[dialogueIndex]);
        dialogueListPanel.SetActive(false);
    }


    void CloseDialogueList()
    {
        dialogueListPanel.SetActive(false);
        ShowMenu();
    }
    
}
