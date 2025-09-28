using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using NUnit.Framework.Constraints;

public class DialogueUIController : MonoBehaviour
{
    // I'm going to fucking kill myself from the amount of shared mutables.
    DialogueGraph playingDialogue;
    VisualElement root;
    Label dialogueContent;
    Label subjectSpeaking;
    Button option;
    List<Button> otherOptions = new List<Button>();
    VisualElement optionContainer;
    string targetLine;
    Button exitButton;
    String otherName;
    float typingSpeed = 0.05f;
    int dialogueIndex = 0;
    Button skipButton;
    public void Activate()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        root.style.display = DisplayStyle.None;

        if (root == null)
        {
            Debug.LogError("UIDocument rootVisualElement is null. Check if UIDocument is assigned and UXML is valid.");
        }
        else
        {
            Debug.Log("UIDocument root loaded successfully.");
        }
        // Tombol-tombol yang dibutuhkan untuk UI
        dialogueContent = root.Q<Label>("dialogue-text");
        subjectSpeaking = root.Q<Label>("subject");
        exitButton = root.Q<Button>("exit-button");
        skipButton = root.Q<Button>("skip-button");
        ScrollView optionTab = root.Q<ScrollView>("option-tab");
        optionContainer = optionTab.contentContainer;
        exitButton.clicked += End;
        skipButton.clicked += HandleDialogueClick;
    }
    void OnEnable()
    {
        Activate();

    }
    void HandleDialogueClick()
    {
        if (otherOptions.Count > 0)
        {
            if (dialogueContent.text != targetLine)
            {
                StopAllCoroutines();
                GetNextLine(true);
            }
            return;
        }

        // Debug.Log("dialogue box meow = " + dialogueContent.text + "dialogue meow meow: " + current[dialogueIndex].message);
        // Debug.Log("length = " + current.Count + "index = " + dialogueIndex + " text = " + current[dialogueIndex].message);
        if (dialogueContent.text == targetLine)
        {
            dialogueIndex++;
            GetNextLine(false);
        }
        else
        {
            StopAllCoroutines();
            // dialogueContent.text = current[dialogueIndex].message;
            GetNextLine(true);
        }
    }
    void Update()
    {
        if (playingDialogue == null)
        {
            return;
        }


    }
    void GetNextLine(bool skipped)
    {
        List<Dialogue> current = playingDialogue.ReturnDialogue();
        if (current != null && dialogueIndex >= current.Count - 1 && otherOptions.Count <= 0)
        {
            ShowOptions(playingDialogue.ReturnDialogueOptions());
        }
        if (skipped)
        {
            dialogueContent.text = targetLine;
            // dialogueIndex++;
        }
        else
        {
            ShowDialogue(current[dialogueIndex]);

        }
        // dialogueContent.text = current[dialogueIndex].message;




    }
    void ShowDialogue(Dialogue dialogue)
    {
        // List<Dialogue> current = playingDialogue.ReturnDialogue();

        StartCoroutine(TypeLine(dialogue));


    }
    void CreateOption(string innerText, EventCallback<ClickEvent> callback)
    {
        Button button = new Button();
        button.text = innerText;
        button.style.color = Color.gray;
        button.style.fontSize = 25;
        button.RegisterCallback<ClickEvent>(callback); // Register the callback here
        otherOptions.Add(button);
        optionContainer.Add(button);
    }
    void ShowOptions(List<DialogueDirection> directions)
    {
        RemoveOptions();
        if (directions == null)
        {
            return;
        }

        if (directions.Count <= 0)
        {
            CreateOption("Goodbye!", ev => End());


        }

        foreach (DialogueDirection dir in directions)
        {
            CreateOption(dir.optionText, ev =>
            {
                NextDialogue(dir.targetNode);
            });

        }
    }
    private IEnumerator TypeLine(Dialogue line)
    {
        subjectSpeaking.text = GetSubject(otherName, line.subject);
        targetLine = line.message;
        Debug.Log("Playing dialogue = " + line.message);
        dialogueContent.text = "";
        foreach (char letter in line.message.ToCharArray())
        {
            dialogueContent.text += letter; // Add one letter at a time
            yield return new WaitForSeconds(typingSpeed); // Wait for the specified typing speed
        }

        // dialogueIndex++;
        // Debug.Log("current index = " + dialogueIndex);
    }
    void NextDialogue(string nextId)
    {
        Debug.Log(nextId);
        bool success = playingDialogue.Next(nextId);
        if (!success)
        {
            EmptyDialogue("There isn't really anything to talk right now", "I have to go!");
            return;
        }
        RemoveOptions();
        dialogueIndex = 0;
        GetNextLine(false);

    }
    public void EmptyDialogue(string message, string closingText)
    {
        Dialogue cantTalk = new Dialogue(message, DialogueSubject.Player);
        root.style.display = DisplayStyle.Flex;
        ShowDialogue(cantTalk);
        RemoveOptions();
        CreateOption(closingText, ev => End());

    }
    string GetSubject(String other, DialogueSubject subject)
    {
        string name = "";
        switch (subject)
        {
            case DialogueSubject.Player:
                name = "You";
                break;
            case DialogueSubject.Npc:
                name = other == null ? "Unknown" : other;
                break;
            default:
                name = "Unknown";
                break;
        }
        return name;
    }

    public void Trigger(DialogueGraph dialogue, String name)
    {

        dialogueIndex = 0;
        Debug.Log("Starting dialogue...");
        this.otherName = name;
        if (dialogue == null)
        {
            EmptyDialogue("I can't talk to them!", "Return");
            return;
        }

        if (dialogue.dialogueFrame == null)
        {
            EmptyDialogue("There is nothing to talk about!", "Return");
            return;
        }
        if (playingDialogue != null)
        {
            Debug.Log("meowing so hard meowwww :3");
            return;

        }
        Debug.Log("Dialogue is now starting");
        root.style.display = DisplayStyle.Flex;
        this.playingDialogue = dialogue;
        GetNextLine(false);

    }
    void RemoveOptions()
    {
        foreach (Button option in otherOptions)
        {
            option.RemoveFromHierarchy();
        }
        otherOptions.Clear();
    }
    public void End()
    {

        if (playingDialogue != null)
        {
            playingDialogue.End();
        }
        Debug.Log("Dialogue ended!");
        RemoveOptions();
        playingDialogue = null;
        otherName = null;
        StopAllCoroutines();
        root.style.display = DisplayStyle.None;
        dialogueIndex = 0;
        dialogueContent.text = "";
        subjectSpeaking.text = "";
    }
}
