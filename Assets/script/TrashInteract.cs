using UnityEngine;

public class TrashInteract : Interactable
{
    [Header("Trash Sprites")]
    public Sprite dirtySprite;    // Sprite sampah kotor
    public Sprite cleanSprite;    // Sprite setelah dibersihkan

    private SpriteRenderer spriteRenderer;
    private bool isClean = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null && dirtySprite != null)
            spriteRenderer.sprite = dirtySprite;
    }

    public override void Interact()
    {
        if (!isClean)
        {
            // Panggil UIManager dan beritahu "ini sampah yang dipilih"
            UIManager.Instance.ShowInteraction(this);
        }
    }

    public void CleanTrash()
    {
        if (isClean) return;

        isClean = true;

        // Ganti sprite
        if (spriteRenderer != null && cleanSprite != null)
            spriteRenderer.sprite = cleanSprite;

        // Beritahu UI Manager tampilkan notifikasi
        UIManager.Instance.ShowNotification("Sampah dibersihkan!");
    }
}
