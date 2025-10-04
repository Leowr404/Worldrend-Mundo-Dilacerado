using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueOption
{
    public string optionText;       // Texto que aparece no botão
    public bool acceptQuest;        // Ativa a quest ligada ao diálogo
    [TextArea(2, 5)] public List<string> response; // resposta do NPC
}