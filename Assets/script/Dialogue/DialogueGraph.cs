using System;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Media;
using UnityEngine;
public enum DialogueSubject
{
    Player,
    Npc
}
[Serializable]
public class Dialogue
{
    public string message;
    public DialogueSubject subject;
    public Dialogue(string message, DialogueSubject subject)
    {
        this.message = message;
        this.subject = subject;
    }
}
[Serializable]
public class DialogueNode
{
    [SerializeField]
    public String dialogueId;
    [SerializeField]
    public List<Dialogue> dialogue;
    [SerializeField]
    public List<DialogueDirection> dialogueOptions;
    public bool AppendEdge(DialogueDirection nextDialogue)
    {
        int index = dialogueOptions.FindIndex(a => a.targetNode == nextDialogue.targetNode);
        if (index < 0)
        {
            dialogueOptions.Add(nextDialogue);
            return true;
        }
        else
        {
            return false;
        }
    }
}
[Serializable]
public class DialogueDirection
{
    public String optionText;
    public String targetNode;
    public DialogueDirection(String targetNode, String optionText)
    {
        this.optionText = optionText;
        this.targetNode = targetNode;
    }

}
public class DialogueGraph : MonoBehaviour
{
    Dictionary<String, DialogueNode> dialogues;
    List<DialogueNode> dialogueHistory;
    public DialogueItem dialogueFrame;
    // bool canTrigger = false;
    public void Awake()
    {
        dialogues = new Dictionary<string, DialogueNode>();
        dialogueHistory = new List<DialogueNode>();
        if (dialogueFrame == null)
        {
            Debug.LogError("Dialogue is empty, make sure to put it in the script!");
            return;
        }
        foreach (DialogueNode node in dialogueFrame.dialogueSequences)
        {
            dialogues.Add(node.dialogueId, node);
            foreach (DialogueDirection direction in node.dialogueOptions)
            {
                node.AppendEdge(direction);

            }
        }
        StartDialogue(dialogueFrame.startingDialogue);

    }
    public void Trigger()
    {

    }
    public void End()
    {
        if (dialogueHistory.Count > 1)
        {

            dialogueHistory.RemoveRange(1, dialogueHistory.Count - 1);

        }
    }

    public void Play()
    {

    }

    public void StartDialogue(DialogueNode startingDialogue)
    {
        if (!dialogues.ContainsKey(startingDialogue.dialogueId)) // Check if the dialogue already exists
        {
            dialogues.Add(startingDialogue.dialogueId, startingDialogue);
            dialogueHistory.Add(startingDialogue); // Set the current dialogue
            foreach (DialogueDirection direction in startingDialogue.dialogueOptions)
            {
                dialogueFrame.startingDialogue.AppendEdge(direction);
            }
        }
    }
    public bool AddDialogueOption(DialogueNode existingDialogue, DialogueNode dialogueToAdd, String dialogueShow)
    {
        DialogueDirection toAdd = new DialogueDirection(dialogueToAdd.dialogueId, dialogueShow);
        bool success = existingDialogue.AppendEdge(toAdd);
        if (success)
        {
            dialogues.Add(dialogueToAdd.dialogueId, dialogueToAdd);
        }
        return success;
    }
    public bool IsEnding()
    {
        return dialogueHistory[dialogueHistory.Count - 1].dialogueOptions?.Count == 0;
    }
    public List<Dialogue> ReturnDialogue()
    {
        if (dialogueHistory.Count == 0)
        {
            return null;
        }
        return dialogueHistory[dialogueHistory.Count - 1]?.dialogue;
    }
    public List<DialogueDirection> ReturnDialogueOptions()
    {
        if (dialogueHistory.Count == 0)
        {
            return null;
        }
        return dialogueHistory[dialogueHistory.Count - 1].dialogueOptions;
    }
    public bool Next(string name)
    {
        bool foundNext = dialogues.ContainsKey(name);
        if (foundNext)
        {
            dialogueHistory.Add(dialogues[name]);
        }
        return foundNext;
    }

}