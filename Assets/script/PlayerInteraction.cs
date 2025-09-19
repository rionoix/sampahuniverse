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

    public void TryInteract()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactionRange, interactableLayer);
        if (hit != null)
        {
            Interactable interactable = hit.GetComponent<Interactable>();
            if (interactable != null)
            {
                interactable.Interact();
                Debug.Log("Interaksi berhasil dengan: " + hit.name);
            }
        }
        else
        {
            Debug.Log("Tidak ada objek interaksi di dekat player");
        }
    }

    // Untuk debug, supaya kelihatan lingkaran interaksi
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
