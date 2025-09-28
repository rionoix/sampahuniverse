using System;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Media;
using UnityEngine;
using UnityEngine.UIElements;
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
/// <summary>
/// Merepresentasikan suatu dokumen dengan sebuah simpul yang memiliki banyak sisi  
/// </summary>
[Serializable]
public class DialogueNode
{
    /// <summary>
    /// Identifier unik dari sebuah dialog.
    /// </summary>
    [SerializeField]
    public string dialogueId;

    /// <summary>
    /// Baris dialog yang disimpan oleh simpul dialog.
    /// </summary>
    [SerializeField]
    public List<Dialogue> dialogue;

    /// <summary>
    /// Opsi-opsi yang terdapat di dialog dengan message menyatakan pesan yang dinyatakan dan subject mengarah ke siapa yang berbicara.
    /// </summary>
    [SerializeField]
    public List<DialogueDirection> dialogueOptions;

    /// <summary>
    /// Mencoba menambahkan arah baru ke node dialog lainnya.
    /// Hanya akan ditambahkan jika node target belum ada dalam daftar opsi.
    /// </summary>
    /// <param name="nextDialogue">Tautan arah menuju node dialog lainnya.</param>
    /// <returns>
    /// True jika arah berhasil ditambahkan; false jika node target sudah ada dalam opsi.
    /// </returns>

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

/// <summary>
/// Mewakili arah pilihan dalam sistem dialog, menghubungkan node saat ini ke node target berikutnya.
/// </summary>
[Serializable]
public class DialogueDirection
{
    /// <summary>
    /// Teks pilihan yang ditampilkan kepada pemain.
    /// </summary>
    public string optionText;

    /// <summary>
    /// ID node target yang akan dituju jika pilihan ini dipilih.
    /// </summary>
    public string targetNode;

    /// <summary>
    /// Membuat instance baru dari arah dialog dengan node tujuan dan teks pilihan.
    /// </summary>
    /// <param name="targetNode">ID node tujuan.</param>
    /// <param name="optionText">Teks pilihan yang ditampilkan.</param>
    public DialogueDirection(string targetNode, string optionText)
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
    public Sprite potrait;
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