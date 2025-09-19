using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI References")]
    public GameObject interactionPanel;   // Panel utama
    public TMP_Text titleText;            // Judul interaksi
    public Button cleanButton;            // Tombol "Bersihkan"
    public TMP_Text notificationText;     // Notifikasi singkat

    private TrashInteract currentTrash;   // sampah yang sedang aktif

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
}
