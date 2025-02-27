using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class Manager : MonoBehaviour
{
    public List<TMP_Text> menuItems;
    public RectTransform cursor;
    private int currentIndex = 0;
    public int columns = 2;

    private UIManager uiManager; // ✅ UIManager 캐싱

    void Start()
    {
        uiManager = FindFirstObjectByType<UIManager>(); // ✅ UIManager 미리 찾기
        if (uiManager == null)
        {
            Debug.LogError("❌ UIManager를 찾을 수 없습니다! 씬에 UIManager가 있는지 확인하세요.");
        }

        if (menuItems == null || menuItems.Count == 0)
        {
            Debug.LogError("menuItems 리스트가 비어 있습니다! Inspector에서 TMP_Text 항목들을 추가하세요.");
            return;
        }

        currentIndex = 0;
        UpdateSelection();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) MoveSelection(-columns);
        else if (Input.GetKeyDown(KeyCode.DownArrow)) MoveSelection(columns);
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveSelection(-1);
        else if (Input.GetKeyDown(KeyCode.RightArrow)) MoveSelection(1);
        else if (Input.GetKeyDown(KeyCode.D)) ConfirmSelection();
        else if (Input.GetKeyDown(KeyCode.F)) CancelSelection();
    }

    public void ConfirmSelection()
    {
        string selectedOption = menuItems[currentIndex].text;
        Debug.Log("선택 확인: " + selectedOption);

        switch (selectedOption)
        {
            case "대화":
                // ✅ UIManager가 존재하는지 확인 후 실행
                if (uiManager != null)
                {
                    uiManager.ShowDialogueList();
                }
                else
                {
                    Debug.LogError("❌ UIManager를 찾을 수 없습니다! 씬에 UIManager가 있는지 확인하세요.");
                }
                break;

            case "단서":
                Debug.Log("단서 기능 구현 예정...");
                break;

            case "이동":
                Debug.Log("이동 기능 구현 예정...");
                break;

            case "세이브":
                Debug.Log("세이브 기능 구현 예정...");
                break;
        }
    }

    void MoveSelection(int offset)
    {
        int newIndex = currentIndex + offset;
        if (newIndex >= 0 && newIndex < menuItems.Count)
        {
            currentIndex = newIndex;
            UpdateSelection();
        }
    }

    void UpdateSelection()
    {
        RectTransform itemRect = menuItems[currentIndex].GetComponent<RectTransform>();
        Vector2 newPos = itemRect.anchoredPosition;
        newPos.x += -35f;
        newPos.y += 0f;
        cursor.anchoredPosition = newPos;
    }

    void CancelSelection()
    {
        Debug.Log("취소 버튼 입력");
    }
}
