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
            entry.button.onClick.AddListener(() =>
            {
                SpawnSticker(entry.stickerPrefab);
            });
        }
    }

    private void SpawnSticker(GameObject prefab)
    {
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
