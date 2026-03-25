using System;
using TMPro;
using UnityEngine;

public class LiveQuestInstance : MonoBehaviour
{
    public LiveQuestPool liveQuestPool;
    public int duration;
    public int pickupID;
    public int dropOffID;
    public float currentTime;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI pickupText;
    [SerializeField] private TextMeshProUGUI dropOffText;
    [SerializeField] private TextMeshProUGUI countdownText;

    public static event Action<int, int> onQuestAccepted;
    public static event Action onQuestExpired;

    private void Update()
    {
        StartRequestCycle();
    }

    public void Initialize(int durationID, int pickupID, int dropOffID)
    {
        currentTime = 0;
        duration = (durationID > 0) ? durationID : 300;
        this.pickupID = pickupID;
        this.dropOffID = dropOffID;

        UpdateUI();
    }

    private void StartRequestCycle()
    {
        currentTime += Time.deltaTime;

        float remaining = duration - currentTime;
        if (remaining <= 0)
        {
            onQuestExpired?.Invoke();
            liveQuestPool.ReturnToPool(this);
            return;
        }

        StartCountdown(remaining);
    }

    #region Hieu Update to use the actual name
    //-------------------------------------------------------------------------
    // private void UpdateUI()
    // {
    //     if (pickupText != null)
    //         pickupText.text = $"Pickup: {pickupID}";

    //     if (dropOffText != null)
    //         dropOffText.text = $"Drop Off: {dropOffID}";
    // }
    //-------------------------------------------------------------------------

    private const string LOCATION_DB_PATH = "SO/Asset/LocationDatabaseSO";
    private LocationDatabaseSO locationDatabase;

    private void UpdateUI()
    {

        if (pickupText != null)
            pickupText.text = $"Pickup: {ExtractTrueName(pickupID)}";

        if (dropOffText != null)
            dropOffText.text = $"Drop Off: {ExtractTrueName(dropOffID)}";
    }

    private void EnsureLocationDatabase()
    {
        if (locationDatabase == null)
            locationDatabase = Resources.Load<LocationDatabaseSO>(LOCATION_DB_PATH);
    }
    private string ExtractTrueName(int id)
    {
        //Load database and set a trueName variable
        EnsureLocationDatabase();
        string trueName = null;

        //Search by id from location database and extract entry's name into trueName
        if (locationDatabase != null)
        {
            var entry = locationDatabase.SearchByID(id);
            if (entry.HasValue)
                trueName = entry.Value.name;
        }

        return trueName;
    }
    #endregion

    private void StartCountdown(float remaining)
    {
        if (countdownText == null) return;

        int minutes = Mathf.FloorToInt(remaining / 60);
        int seconds = Mathf.FloorToInt(remaining % 60);
        countdownText.text = $"{minutes:00}:{seconds:00}";
    }

    public void AcceptQuest()
    {
        
        onQuestAccepted?.Invoke(pickupID, dropOffID);
        liveQuestPool.FlushPool();
        liveQuestPool.ReturnToPool(this);
    }
}