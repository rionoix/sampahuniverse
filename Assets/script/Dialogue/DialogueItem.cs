using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueItem", menuName = "Dialogue/DialogueItem")]
[Serializable]
public class DialogueItem : ScriptableObject
{
    public DialogueNode startingDialogue;
    public List<DialogueNode> dialogueSequences;
}