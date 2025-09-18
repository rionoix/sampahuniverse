using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionRange = 1f;       // jarak interaksi
    public LayerMask interactableLayer;       // filter layer Interactable

    void Update()
    {
        // Tekan E di PC
        if (Input.GetKeyDown(KeyCode.E)) 
        {
            TryInteract();
        }

        // Kalau di Android, nanti pakai tombol UI → panggil TryInteract()
    }

    private void TryInteract()
    {
        // Cari objek di sekitar player
        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactionRange, interactableLayer);
        if (hit != null)
        {
            Interactable interactable = hit.GetComponent<Interactable>();
            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }

    // Untuk debug, supaya kelihatan lingkaran interaksi
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
