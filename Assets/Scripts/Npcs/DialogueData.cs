using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "RPG/Dialogue")]
public class DialogueData : ScriptableObject
{
    public string npcName;

    [Header("Falas Genéricas")]
    public List<DialogueLine> lines;

    [Header("Falas da Quest")]
    public List<DialogueLine> beforeQuest;
    public List<DialogueLine> duringQuest;
    public List<DialogueLine> afterQuest;

    [Header("Quest Opcional")]
    public Quest quest;

    [Header("Opções de Diálogo")]
    public List<DialogueOption> options;
}

[System.Serializable]
public class DialogueLine
{
    [TextArea(2, 5)] public string text;
}

[System.Serializable]
public class DialogueOption
{
    public string optionText;
    public List<DialogueLine> response;
}