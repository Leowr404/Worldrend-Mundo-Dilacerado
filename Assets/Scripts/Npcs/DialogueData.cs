using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "RPG/Dialogue")]
public class DialogueData : ScriptableObject
{
    public string npcName;

    [Header("Fal falas normais (se não tiver quest)")]
    [TextArea(2, 5)] public List<string> lines;

    [Header("Quest ligada ao diálogo (opcional)")]
    public Quest quest;

    [TextArea(2, 5)] public List<string> beforeQuest;
    [TextArea(2, 5)] public List<string> duringQuest;
    [TextArea(2, 5)] public List<string> afterQuest;

    [Header("Opções de diálogo (botões)")]
    public List<DialogueOption> options;
}