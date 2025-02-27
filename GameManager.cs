using UnityEngine;
using System.Collections.Generic;
public enum ClueType {
    Character,
    Object
}

[System.Serializable]
public class Clue {
    public string name; // 단서 이름
    public ClueType type;       // 인물단서인지 물건단서인지 구분
    public string clueText;     // 단서 내용
    public string detailinfo; // 인물 단서의 상세 정보
    public string characterName; // 단서의 관련 인물의 이름 *인물단서서
    
    // 인물단서가 아닐 경우 characterName은 비워둡니다.
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // 캐릭터별 호감도 관리 (캐릭터 이름을 key로 사용)
    public Dictionary<string, int> characterAffections = new Dictionary<string, int>();

    // 모든 단서를 저장하는 리스트 (인물단서 / 물건단서 포함)
    public List<Clue> clues = new List<Clue>();

    // 현재 씬 정보 등 다른 전역 데이터도 필요하다면 추가

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeCharacters();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 게임 시작 시 기본 캐릭터와 호감도 초기화
    private void InitializeCharacters()
    {
        SetAffection("Kim", 0);
        SetAffection("Lee", 0);
        SetAffection("Bae", 0);
        SetAffection("Kang", 0);
        SetAffection("Choi", 0);
        SetAffection("Senpai", 2);
    }

    // 특정 캐릭터의 호감도를 등록 (초기값 설정)
    public void SetAffection(string characterName, int initialAffection)
    {
        if (!characterAffections.ContainsKey(characterName))
        {
            characterAffections.Add(characterName, initialAffection);
            Debug.Log($"{characterName}의 호감도를 {initialAffection}으로 초기화했습니다.");
        }
    }

    // 특정 캐릭터의 호감도를 증가시키는 메소드
    public void IncreaseAffection(string characterName, int amount)
    {
        if (characterAffections.ContainsKey(characterName))
        {
            characterAffections[characterName] += amount;
            Debug.Log($"{characterName}의 호감도가 {characterAffections[characterName]}로 증가했습니다.");
        }
        else
        {
            Debug.LogWarning($"{characterName} 캐릭터 데이터가 존재하지 않습니다.");
        }
    }

    // 특정 캐릭터의 호감도를 반환하는 메소드
    public int GetAffection(string characterName)
    {
        if (characterAffections.ContainsKey(characterName))
        {
            return characterAffections[characterName];
        }
        Debug.LogWarning($"{characterName} 캐릭터 데이터가 존재하지 않습니다.");
        return 0;
    }

    // 단서를 추가하는 메소드
    public void AddClue(Clue clue)
    {
        clues.Add(clue);
        Debug.Log($"단서 추가: {clue.clueText} [{clue.type}]");
    }
}
