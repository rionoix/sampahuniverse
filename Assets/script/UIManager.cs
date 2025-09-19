using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI References")]
    public GameObject interactionPanel;
    public TMP_Text titleText;
    public Button cleanButton;
    public TMP_Text notificationText;

    private TrashInteract currentTrash;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (interactionPanel != null)
            interactionPanel.SetActive(false);

        if (notificationText != null)
            notificationText.gameObject.SetActive(false);

        if (cleanButton != null)
            cleanButton.onClick.AddListener(OnCleanClicked);
    }

    public void ShowInteraction(TrashInteract trash)
    {
        currentTrash = trash;

        if (interactionPanel != null)
            interactionPanel.SetActive(true);

        if (titleText != null)
            titleText.text = "Sampah Neo";
    }

    private void OnCleanClicked()
    {
        if (currentTrash != null)
        {
            currentTrash.CleanTrash();
            if (interactionPanel != null)
                interactionPanel.SetActive(false);

            currentTrash = null;
        }
    }

    public void ShowNotification(string message)
    {
        if (notificationText == null) return;

        notificationText.gameObject.SetActive(true);
        notificationText.text = message;

        CancelInvoke(nameof(HideNotification));
        Invoke(nameof(HideNotification), 2f);
    }

    private void HideNotification()
    {
        if (notificationText != null)
            notificationText.gameObject.SetActive(false);
    }

    // 🔄 Fungsi Reset
    public void ResetGame()
    {
        TrashInteract[] allTrash = FindObjectsOfType<TrashInteract>();
        foreach (var trash in allTrash)
        {
            trash.ResetTrash();
        }

        ShowNotification("Semua sampah dikembalikan!");
    }

    // ❌ Fungsi Exit Game
    public void ExitGame()
    {
        Debug.Log("Keluar game...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // berhenti play mode
#else
        Application.Quit(); // keluar aplikasi
#endif
    }
}
