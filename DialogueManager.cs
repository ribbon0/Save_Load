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
    private static string basePath;

    void Awake()
    {
        basePath = Application.persistentDataPath;
    }

    void Start()
    {
        int lastUsedSlot = PlayerPrefs.GetInt("lastUsedSlot", 1);
        LoadGame(lastUsedSlot);
    }

    /// ✅ **특정 슬롯에 게임 저장**
    public static void SaveGame(int slotIndex)
    {
        string filePath = Path.Combine(basePath, $"SaveSlot_{slotIndex}.json");
        DialogueCollection dialogueCollection = new DialogueCollection { dialogues = loadedDialogues };
        string json = JsonUtility.ToJson(dialogueCollection, true);

        File.WriteAllText(filePath, json);
        PlayerPrefs.SetInt("lastUsedSlot", slotIndex);
        PlayerPrefs.Save();

        Debug.Log($"💾 저장 완료! 슬롯 {slotIndex}: {filePath}");
    }

    /// ✅ **특정 슬롯에서 게임 로드**
    public static void LoadGame(int slotIndex)
    {
        string filePath = Path.Combine(basePath, $"SaveSlot_{slotIndex}.json");

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            DialogueCollection dialogueCollection = JsonUtility.FromJson<DialogueCollection>(json);
            loadedDialogues = dialogueCollection.dialogues;

            PlayerPrefs.SetInt("lastUsedSlot", slotIndex);
            PlayerPrefs.Save();

            Debug.Log($"📂 슬롯 {slotIndex}에서 로드 완료!");
        }
        else
        {
            Debug.LogWarning($"❌ 저장된 파일이 없습니다: 슬롯 {slotIndex}");
        }
    }

    /// ✅ **새로운 대사 추가 (저장 슬롯에 반영)**
    public static void AddDialogue(DialogueData newDialogue, int slotIndex)
    {
        if (loadedDialogues.Exists(d => d.dialogueId == newDialogue.dialogueId))
        {
            Debug.LogWarning("⚠️ 이미 존재하는 대화 ID입니다: " + newDialogue.dialogueId);
            return;
        }

        loadedDialogues.Add(newDialogue);
        SaveGame(slotIndex);
        Debug.Log("📝 새로운 대화 추가됨: " + newDialogue.dialogueId);
    }
}
