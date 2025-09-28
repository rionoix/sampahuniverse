using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueItem", menuName = "Dialogue/DialogueItem")]
[Serializable]
/// <summary>
/// Menyimpan satu set node dialog sebagai aset yang dapat digunakan kembali dalam sistem dialog.
/// </summary>
/// <remarks>
/// Gunakan ScriptableObject ini untuk mendefinisikan urutan dialog dan titik awal percakapan.
/// Cocok untuk sistem dialog berbasis graph atau branching.
/// </remarks>
public class DialogueItem : ScriptableObject
{
    /// <summary>
    /// Node dialog awal yang menjadi titik masuk percakapan.
    /// </summary>
    public DialogueNode startingDialogue;

    /// <summary>
    /// Daftar semua node dialog yang membentuk urutan atau cabang percakapan.
    /// </summary>
    public List<DialogueNode> dialogueSequences;
}
