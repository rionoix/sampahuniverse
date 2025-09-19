using System.Collections;
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

    // New fields untuk skor
    [Header("Score UI")]
    public TMP_Text currentScoreText;     // contoh: "4/10"
    public TMP_Text totalCleanedText;     // contoh: "Total dibersihkan: 42"

    // Counters
    private int currentCleanCount = 0;    // jumlah yang dibersihkan di ronde sekarang
    private int totalCleanedCount = 0;    // total kumulatif (tidak reset saat ResetGame dipencet)
    private int totalTrashThisRound = 10; // default, akan di-overwrite oleh spawner jika ada

    [Header("Persistence")]
    public bool usePlayerPrefs = true;    // jika true, totalCleanedCount disimpan di PlayerPrefs
    public string prefsKey = "TotalCleanedAllTime";

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

    void Start()
    {
        // load total kumulatif dari PlayerPrefs kalau diaktifkan
        if (usePlayerPrefs)
            totalCleanedCount = PlayerPrefs.GetInt(prefsKey, 0);

        // Ambil informasi jumlah trash dari TrashSpawner bila ada
        var spawner = FindObjectOfType<TrashSpawner>();
        if (spawner != null)
            totalTrashThisRound = spawner.trashCount;

        UpdateScoreUI();

        // Refresh counts *setelah* spawn (biar aman jika spawner spawn di Start)
        StartCoroutine(RefreshCountsNextFrame());
    }

    IEnumerator RefreshCountsNextFrame()
    {
        yield return null; // tunggu 1 frame agar TrashSpawner.Start sudah run
        RefreshCountsFromScene();
        UpdateScoreUI();
    }

    // Hitung ulang berdasarkan objek di scene (berguna kalau spawn dinamis)
    void RefreshCountsFromScene()
    {
        var allTrash = FindObjectsOfType<TrashInteract>();
        totalTrashThisRound = allTrash.Length;
        currentCleanCount = 0;
        foreach (var t in allTrash)
        {
            if (t.IsClean) currentCleanCount++;
        }
    }

    // Dipanggil dari TrashInteract ketika sampah dibersihkan
    public void AddCleaned(int amount = 1)
    {
        currentCleanCount += amount;
        totalCleanedCount += amount;

        // simpan persistent jika diperlukan
        if (usePlayerPrefs)
            PlayerPrefs.SetInt(prefsKey, totalCleanedCount);

        UpdateScoreUI();
        ShowNotification("Sampah dibersihkan!");
    }

    // Update tampilan teks
    private void UpdateScoreUI()
    {
        if (currentScoreText != null)
            currentScoreText.text = currentCleanCount + "/" + totalTrashThisRound;

        if (totalCleanedText != null)
            totalCleanedText.text = "Total dibersihkan: " + totalCleanedCount;
    }

    // Interaction panel (tetap)
    public void ShowInteraction(TrashInteract trash)
    {
        currentTrash = trash;

        if (interactionPanel != null)
            interactionPanel.SetActive(true);

        if (titleText != null)
            titleText.text = "N*o Sampah";
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

    // Notification (tetap)
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

    // 🔄 Fungsi Reset: kembalikan semua trash ke kotor
    public void ResetGame()
    {
        TrashInteract[] allTrash = FindObjectsOfType<TrashInteract>();
        foreach (var trash in allTrash)
        {
            trash.ResetTrash();
        }

        // Karena semua dikembalikan, reset hitungan ronde sekarang
        currentCleanCount = 0;
        UpdateScoreUI();

        ShowNotification("Semua sampah dikembalikan!");
    }

    // ❌ Fungsi Exit Game (tetap)
    public void ExitGame()
    {
        Debug.Log("Keluar game...");

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // berhenti play mode
    #else
        Application.Quit(); // keluar aplikasi
    #endif
    }

    // Optional: fungsi untuk reset total kumulatif (misal untuk debug)
    public void ResetTotalCleanedPersistent()
    {
        totalCleanedCount = 0;
        if (usePlayerPrefs)
            PlayerPrefs.DeleteKey(prefsKey);
        UpdateScoreUI();
    }
}
