using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class StartSceneUIManager : MonoBehaviour
{
    // TMP Button 컴포넌트들을 담을 리스트 (Inspector에서 할당)
    public List<Button> menuButtons;
    
    // 선택된 버튼 옆에 표시할 커서 이미지의 RectTransform (선택 사항)
    public RectTransform cursor;
    public float cursorOffsetX = -30f;
    public float cursorOffsetY = 0f;

    // 기본 색상과 선택된 항목 색상 (TMP_Text의 색상)
    public Color defaultColor = Color.white;
    public Color selectedColor = Color.yellow;

    private int currentIndex = 0;

    void Start()
    {
        UpdateSelection();
    }

    void Update()
    {
        // 위 방향키로 선택 항목 위로 이동 (wrap-around)
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentIndex--;
            if (currentIndex < 0)
                currentIndex = menuButtons.Count - 1;
            UpdateSelection();
        }
        // 아래 방향키로 선택 항목 아래로 이동 (wrap-around)
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentIndex++;
            if (currentIndex >= menuButtons.Count)
                currentIndex = 0;
            UpdateSelection();
        }
        
        // D 키로 선택(확인) 실행
        if (Input.GetKeyDown(KeyCode.D))
        {
            ConfirmSelection();
        }
        // F 키로 취소 실행
        if (Input.GetKeyDown(KeyCode.F))
        {
            CancelSelection();
        }
    }

    void UpdateSelection()
    {
        // 각 Button의 자식 TMP_Text 색상을 업데이트
        for (int i = 0; i < menuButtons.Count; i++)
        {
            TMP_Text buttonText = menuButtons[i].GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.color = (i == currentIndex) ? selectedColor : defaultColor;
            }
        }
        
        // 커서가 있다면, 현재 선택된 Button의 위치에 오프셋을 적용하여 이동
        if (cursor != null)
        {
            RectTransform selectedRect = menuButtons[currentIndex].GetComponent<RectTransform>();
            Vector2 newPos = selectedRect.anchoredPosition;
            newPos.x += cursorOffsetX;
            newPos.y += cursorOffsetY;
            cursor.anchoredPosition = newPos;
        }
    }

    void ConfirmSelection()
    {
        string selectedOption = menuButtons[currentIndex].GetComponentInChildren<TMP_Text>().text;
        Debug.Log("선택 확인: " + selectedOption);
        
        // 1️⃣ 버튼의 onClick 실행 (Inspector에서 연결 가능)
        
        // 2️⃣ 코드에서 직접 동작 실행
        switch (selectedOption)
        {
            case "시작":
                SceneManager.LoadScene("MainScene"); // MainScene으로 이동
                break;
            case "로드":
                Debug.Log("로드 기능 구현 예정...");
                break;
            case "종료":
                Application.Quit();
                break;
            case "크레딧":
                SceneManager.LoadScene("CreditsScene"); // 크레딧 씬 이동
                break;
        }
    }

    void CancelSelection()
    {
        Debug.Log("취소 버튼 입력");
    }
}
