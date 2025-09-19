using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    public GameObject trashPrefab;   // drag prefab sampah ke sini
    public int trashCount = 10;      // jumlah spawn
    public float spawnRange = 5f;    // jangkauan acak (X dan Y)

    void Start()
    {
        SpawnTrash();
    }

    void SpawnTrash()
    {
        for (int i = 0; i < trashCount; i++)
        {
            // ambil posisi acak di sekitar spawner
            Vector2 randomPos = new Vector2(
                transform.position.x + Random.Range(-spawnRange, spawnRange),
                transform.position.y + Random.Range(-spawnRange, spawnRange)
            );

            // buat objek sampah baru
            Instantiate(trashPrefab, randomPos, Quaternion.identity);
        }
    }
}
