using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class DialogueEntry
{
    public string speaker;
    public string text;
}

[System.Serializable]
public class DialogueData
{
    public string scene;
    public string dialogueId;
    public string dialogueTitle;
    public bool isUnlocked;
    public bool isCompleted;
    public List<DialogueEntry> entries;
}

[System.Serializable]
public class DialogueCollection
{
    public List<DialogueData> dialogues;
}

public class DialogueManager : MonoBehaviour
{
    public static List<DialogueData> loadedDialogues = new List<DialogueData>();

    void Start()
    {
        LoadDialogueForCharacter();
    }

    public static void LoadDialogueForCharacter()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("dialogue_data");  // ✅ 확장자 제거!
        
        if (jsonFile == null)
        {
            Debug.LogError("❌ JSON 파일을 찾을 수 없습니다: Resources/dialogue_data.json");
            return;
        }

        DialogueCollection dialogueCollection = JsonUtility.FromJson<DialogueCollection>(jsonFile.text);
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        loadedDialogues = dialogueCollection.dialogues.FindAll(d => d.scene == currentScene);
        Debug.Log("✅ 현재 씬의 대화 데이터 로드 완료: " + currentScene);
    }

    public static void CompleteDialogue(string dialogueId)
    {
        foreach (var dialogue in loadedDialogues)
        {
            if (dialogue.dialogueId == dialogueId)
            {
                dialogue.isCompleted = true;
                SaveDialogueState(dialogueId, true); // ✅ PlayerPrefs 저장
                Debug.Log("🟢 대화 완료 상태 업데이트됨: " + dialogueId);
                break;
            }
        }
    }

    // 📌 PlayerPrefs를 사용하여 대화 상태 저장
    private static void SaveDialogueState(string dialogueId, bool isCompleted)
    {
        PlayerPrefs.SetInt("DialogueCompleted_" + dialogueId, isCompleted ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log("💾 PlayerPrefs에 저장 완료: " + dialogueId);
    }

    // 📌 PlayerPrefs에서 대화 완료 상태 불러오기
    public static void LoadDialogueState()
    {
        foreach (var dialogue in loadedDialogues)
        {
            string key = "DialogueCompleted_" + dialogue.dialogueId;
            if (PlayerPrefs.HasKey(key))
            {
                dialogue.isCompleted = PlayerPrefs.GetInt(key) == 1;
            }
        }
        Debug.Log("🔄 대화 완료 상태 불러오기 완료!");
    }
}