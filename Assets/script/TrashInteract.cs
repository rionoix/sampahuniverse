using UnityEngine;

public class TrashInteract : Interactable
{
    [Header("Trash Sprites")]
    public Sprite dirtySprite;    // Sprite sampah kotor
    public Sprite cleanSprite;    // Sprite setelah dibersihkan

    private SpriteRenderer spriteRenderer;
    public bool IsClean { get; private set; } = false; // public read-only

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        IsClean = false;

        if (spriteRenderer != null && dirtySprite != null)
            spriteRenderer.sprite = dirtySprite;
    }

    public override void Interact()
    {
        if (!IsClean)
        {
            UIManager.Instance.ShowInteraction(this);
        }
    }

    public void CleanTrash()
    {
        if (IsClean) return; // jangan hitung dua kali

        IsClean = true;

        if (spriteRenderer != null && cleanSprite != null)
            spriteRenderer.sprite = cleanSprite;

        // Beritahu UIManager bahwa satu sampah sudah dibersihkan
        if (UIManager.Instance != null)
            UIManager.Instance.AddCleaned(1);
    }

    public void ResetTrash()
    {
        IsClean = false;

        if (spriteRenderer != null && dirtySprite != null)
            spriteRenderer.sprite = dirtySprite;
    }
}
