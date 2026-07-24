using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StickerSpawner : MonoBehaviour
{
    [System.Serializable]
    public class StickerEntry
    {
        public Button button;
        public GameObject stickerPrefab;
    }

    [Header("Setup")]
    public Canvas canvas;
    public Transform stickerParent;

    [Header("Settings")]
    public float pickupGraceTime = 0.15f;

    public List<StickerEntry> stickers = new List<StickerEntry>();

    void Start()
    {
        foreach (var entry in stickers)
        {
            if (entry == null || entry.button == null || entry.stickerPrefab == null)
            {
                Debug.LogWarning("StickerSpawner: skipped an incomplete sticker entry.", this);
                continue;
            }

            GameObject stickerPrefab = entry.stickerPrefab;
            entry.button.onClick.AddListener(() =>
            {
                SpawnSticker(stickerPrefab);
            });
        }
    }

    private void SpawnSticker(GameObject prefab)
    {
        if (prefab == null || stickerParent == null)
        {
            Debug.LogWarning("StickerSpawner: missing sticker prefab or parent.", this);
            return;
        }

        GameObject stickerGO = Instantiate(prefab, stickerParent);
        Stickers sticker = stickerGO.GetComponent<Stickers>();

        if (sticker == null)
        {
            Debug.LogError("Sticker prefab missing Stickers component!");
            return;
        }

        sticker.canvas = canvas;
        sticker.ForcePickUp(pickupGraceTime);
    }
}
