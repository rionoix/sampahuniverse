using UnityEngine;
using System.Collections.Generic;

public class TrashSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject trashPrefab;   // prefab sampah
    public int trashCount = 10;      // jumlah spawn
    public float spawnRange = 5f;    // jangkauan acak (X dan Y)

    [Header("Spacing Settings")]
    public float minDistance = 1.5f; // jarak minimum antar sampah

    private List<Vector2> usedPositions = new List<Vector2>();

    void Start()
    {
        SpawnTrash();
    }

    void SpawnTrash()
    {
        int maxAttempts = 30; // batas percobaan cari posisi

        for (int i = 0; i < trashCount; i++)
        {
            Vector2 spawnPos = Vector2.zero;
            bool validPos = false;

            // coba beberapa kali sampai dapat posisi yang valid
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                Vector2 randomPos = new Vector2(
                    transform.position.x + Random.Range(-spawnRange, spawnRange),
                    transform.position.y + Random.Range(-spawnRange, spawnRange)
                );

                if (IsFarEnough(randomPos))
                {
                    spawnPos = randomPos;
                    validPos = true;
                    break;
                }
            }

            if (validPos)
            {
                Instantiate(trashPrefab, spawnPos, Quaternion.identity);
                usedPositions.Add(spawnPos);
            }
            else
            {
                Debug.LogWarning("Gagal menemukan posisi spawn yang cukup jauh, skip 1 sampah.");
            }
        }
    }

    bool IsFarEnough(Vector2 pos)
    {
        foreach (Vector2 usedPos in usedPositions)
        {
            if (Vector2.Distance(pos, usedPos) < minDistance)
                return false; // terlalu dekat
        }
        return true;
    }
}
