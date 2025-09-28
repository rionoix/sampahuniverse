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
    // Referensi ke graph dialog yang sedang dimainkan.
    DialogueGraph playingDialogue;

    // Root dari UI Toolkit (VisualElement utama).
    VisualElement root;

    // Label untuk menampilkan isi dialog.
    Label dialogueContent;

    // Label untuk menampilkan nama pembicara.
    Label subjectSpeaking;

    // Tombol pilihan utama (jika ada).
    Button option;
    // 
    Image speakingPotrait;
    // Potret player

    Sprite playerPotrait;
    // Potret yang lain
    Sprite otherPotrait;
    // Daftar tombol pilihan lainnya.
    List<Button> otherOptions = new List<Button>();

    // Kontainer tempat tombol-tombol pilihan ditambahkan.
    VisualElement optionContainer;

    // Baris dialog yang sedang ditargetkan untuk ditampilkan.
    string targetLine;

    // Tombol keluar dari dialog.
    Button exitButton;

    // Nama karakter lain yang sedang berbicara.
    string otherName;

    // Kecepatan mengetik huruf per huruf dalam efek dialog.
    float typingSpeed = 0.05f;

    // Indeks dialog saat ini dalam urutan.
    int dialogueIndex = 0;

    // Tombol untuk skip atau lanjut dialog.
    Button skipButton;
    /// <summary>
    /// Menginisialisasi dan menghubungkan elemen UI dari UIDocument. Pastikan terdapat HANYA SATU GameObject bernama yang berisikan "Dialogue" dan berisi UIDocument beserta DialogueUIController 
    /// </summary>

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
        speakingPotrait = root.Q<Image>("potrait");
        optionContainer = optionTab.contentContainer;
        exitButton.clicked += End;
        skipButton.clicked += HandleDialogueClick;
    }
    void OnEnable()
    {
        Activate();

    }
    /// <summary>
    /// Menangani klik pada area dialog untuk melanjutkan atau menampilkan baris berikutnya.
    /// </summary>

    void HandleDialogueClick()
    {
        // I have no idea how any of this works please pretend it does.
        Debug.Log("This is a test!");
        if (otherOptions.Count > 0)
        {
            if (dialogueContent.text != targetLine)
            {

                StopAllCoroutines();
                GetNextLine(true);
            }
            return;
        }


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

    /// <summary>
    /// Dipanggil setiap frame. Digunakan untuk validasi atau update runtime.
    /// </summary>

    void Update()
    {
        if (playingDialogue == null)
        {
            return;
        }


    }
    /// <summary>
    /// Menampilkan baris dialog berikutnya, baik secara langsung atau dengan efek mengetik.
    /// </summary>
    /// <param name="skipped">Jika true, langsung tampilkan teks tanpa efek mengetik.</param>

    void GetNextLine(bool skipped)
    {
        List<Dialogue> current = playingDialogue.ReturnDialogue();
        if (current != null && dialogueIndex >= current.Count - 1 && otherOptions.Count <= 0)
        {
            ShowOptions(playingDialogue.ReturnDialogueOptions());
        }
        Debug.Log("dialogue box meow = " + dialogueContent.text + "dialogue meow meow: " + current[dialogueIndex].message);
        Debug.Log("length = " + current.Count + "index = " + dialogueIndex + " text = " + current[dialogueIndex].message);
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
    /// <summary>
    /// Menampilkan satu baris dialog dengan efek mengetik.
    /// </summary>
    /// <param name="dialogue">Objek dialog yang akan ditampilkan.</param>

    void ShowDialogue(Dialogue dialogue)
    {
        // List<Dialogue> current = playingDialogue.ReturnDialogue();

        StartCoroutine(TypeLine(dialogue));


    }
    /// <summary>
    /// Membuat tombol pilihan dialog dengan teks dan callback klik.
    /// </summary>
    /// <param name="innerText">Teks yang ditampilkan pada tombol.</param>
    /// <param name="callback">Fungsi yang dipanggil saat tombol diklik.</param>

    void CreateOption(string innerText, EventCallback<ClickEvent> callback)
    {
        Button button = new Button();
        button.text = innerText;
        button.style.color = Color.gray;
        button.style.fontSize = 20;
        button.style.maxHeight = Length.Percent(30);
        button.style.height = Length.Percent(100);
        button.RegisterCallback<ClickEvent>(callback); // Register the callback here
        otherOptions.Add(button);
        optionContainer.Add(button);
    }

    /// <summary>
    /// Menampilkan semua pilihan arah dialog yang tersedia.
    /// </summary>
    /// <param name="directions">Daftar arah dialog dari node saat ini.</param>

    void ShowOptions(List<DialogueDirection> directions)
    {
        RemoveOptions();
        // Jangan tampilkan jika arah dialog ke 0.
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
    /// <summary>
    /// Coroutine untuk menampilkan teks dialog huruf per huruf.
    /// </summary>
    /// <param name="line">Objek dialog yang berisi pesan dan subjek.</param>

    private IEnumerator TypeLine(Dialogue line)
    {
        var (name, currentPotrait) = GetSubject(otherName, line.subject);
        subjectSpeaking.text = name;
        Debug.Log("Speaking potrait = " + speakingPotrait + " With current potrait = " + currentPotrait);
        if (currentPotrait != null && speakingPotrait != null)
        {
            speakingPotrait.style.backgroundImage = new StyleBackground(currentPotrait);
            speakingPotrait.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
            speakingPotrait.style.backgroundPositionX = new BackgroundPosition(BackgroundPositionKeyword.Center);
            speakingPotrait.style.backgroundPositionY = new BackgroundPosition(BackgroundPositionKeyword.Center);
        }
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
    /// <summary>
    /// Melanjutkan ke node dialog berikutnya berdasarkan ID target.
    /// </summary>
    /// <param name="nextId">ID node dialog tujuan.</param>

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
    /// Menampilkan dialog kosong atau fallback jika tidak ada dialog yang tersedia.
    /// </summary>
    /// <param name="message">Pesan yang ditampilkan.</param>
    /// <param name="closingText">Teks tombol untuk menutup dialog.</param>

    public void EmptyDialogue(string message, string closingText)
    {
        Dialogue cantTalk = new Dialogue(message, DialogueSubject.Player);
        root.style.display = DisplayStyle.Flex;
        ShowDialogue(cantTalk);
        RemoveOptions();
        CreateOption(closingText, ev => End());

    }

    /// <summary>
    /// Mengembalikan nama pembicara berdasarkan subjek dialog.
    /// </summary>
    /// <param name="other">Nama karakter lain.</param>
    /// <param name="subject">Subjek dialog (Player atau NPC).</param>
    /// <returns>Nama pembicara yang ditampilkan.</returns>

    (string, Sprite) GetSubject(String other, DialogueSubject subject)
    {
        string name = "";
        Sprite potrait = null;
        switch (subject)
        {
            case DialogueSubject.Player:
                name = "You";
                potrait = playerPotrait;
                break;
            case DialogueSubject.Npc:
                name = other == null ? "Unknown" : other;
                potrait = otherPotrait;
                break;
            default:
                name = "Unknown";
                break;
        }
        return (name, potrait);
    }
    /// <summary>
    /// Memulai sesi dialog baru menggunakan graph dialog dan nama karakter.
    /// Pastikan graph berisi item dialog yang valid, dan tidak ada sesi dialog lain yang sedang berlangsung.
    /// Jika nama karakter tidak tersedia, maka field subject akan menampilkan "Unknown".
    /// </summary>
    /// <param name="dialogue">Graph dialog yang akan dijalankan.</param>
    /// <param name="name">Nama karakter lain yang menjadi lawan bicara.</param>
    public void Trigger(DialogueGraph dialogue, String name, Sprite playerPotrait, Sprite otherPotrait)
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
        this.playerPotrait = playerPotrait;
        this.otherPotrait = otherPotrait;
        GetNextLine(false);

    }

    /// <summary>
    /// Menghapus semua tombol pilihan dari UI.
    /// </summary>

    void RemoveOptions()
    {
        foreach (Button option in otherOptions)
        {
            option.RemoveFromHierarchy();
        }
        otherOptions.Clear();
    }
    /// <summary>
    /// Mengakhiri sesi dialog dan mereset UI.
    /// </summary>

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
        this.playerPotrait = null;
        this.otherPotrait = null;
        root.style.display = DisplayStyle.None;
        dialogueIndex = 0;
        dialogueContent.text = "";
        subjectSpeaking.text = "";
    }
}
