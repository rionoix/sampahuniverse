using UnityEngine;
using UnityEngine.UI;
using TMPro; // pakai TextMeshPro

public class TrashInteract : Interactable
{
    [Header("UI References")]
    public GameObject interactionPanel;   // Panel utama
    public TMP_Text titleText;            // Judul interaksi
    public Button cleanButton;            // Tombol "Bersihkan"
    public TMP_Text notificationText;     // Teks notifikasi

    [Header("Trash Sprites")]
    public Sprite dirtySprite;            // Sprite sampah kotor
    public Sprite cleanSprite;            // Sprite setelah dibersihkan
    private SpriteRenderer spriteRenderer;

    private bool isClean = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Pastikan panel & notifikasi mati saat mulai
        if (interactionPanel != null)
            interactionPanel.SetActive(false);

        if (notificationText != null)
            notificationText.gameObject.SetActive(false);

        if (cleanButton != null)
            cleanButton.onClick.AddListener(CleanTrash);
    }

    public override void Interact()
    {
        if (!isClean && interactionPanel != null)
        {
            interactionPanel.SetActive(true);

            if (titleText != null)
                titleText.text = "Sampah Neo";
        }
    }

    private void CleanTrash()
    {
        isClean = true;

        // Ganti sprite
        if (spriteRenderer != null && cleanSprite != null)
            spriteRenderer.sprite = cleanSprite;

        // Tutup panel interaksi
        if (interactionPanel != null)
            interactionPanel.SetActive(false);

        // Tampilkan notifikasi
        if (notificationText != null)
        {
            notificationText.gameObject.SetActive(true);
            notificationText.text = "Sampah dibersihkan";

            // Sembunyikan notifikasi setelah 2 detik
            Invoke(nameof(HideNotification), 2f);
        }
    }

    private void HideNotification()
    {
        if (notificationText != null)
            notificationText.gameObject.SetActive(false);
    }
}
