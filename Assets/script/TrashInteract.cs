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
            UIManager.Instance.ShowInteraction(this);
        }
    }

    public void CleanTrash()
    {
        if (isClean) return;

        isClean = true;

        if (spriteRenderer != null && cleanSprite != null)
            spriteRenderer.sprite = cleanSprite;

        UIManager.Instance.ShowNotification("Sampah dibersihkan!");
    }

    public void ResetTrash()
    {
        isClean = false;

        if (spriteRenderer != null && dirtySprite != null)
            spriteRenderer.sprite = dirtySprite;
    }
}
