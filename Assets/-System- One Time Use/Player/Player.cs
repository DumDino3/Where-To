using UnityEngine;
using System.Diagnostics;

public enum PlayerLockSource
{
    Unknown,
    Dialogue,
    System,
    PauseMenu
}

public class Player : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerInteraction interaction;
    public CharacterController characterController;
    private bool pauseMenuOpen;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        interaction = GetComponent<PlayerInteraction>();
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        LockCursor();
    }

    // ---------------- Pause Menu ----------------

    private void HandlePauseMenuState(bool isOpen)
    {
        pauseMenuOpen = isOpen;
        RefreshPlayerState();
    }

    // ---------------- Core Logic ----------------

    private void RefreshPlayerState()
    {
        // Pause menu is second priority
        if (pauseMenuOpen)
        {
            DisablePlayer(PlayerLockSource.PauseMenu);
            return;
        }

        // No locks remaining
        EnablePlayer(PlayerLockSource.System);
    }

    // ---------------- Player Control ----------------

    public void DisablePlayer(PlayerLockSource source = PlayerLockSource.Unknown)
    {
        UnityEngine.Debug.Log(
            $"<color=red>[Player DISABLED]</color> by <b>{source}</b>\n{GetStackTrace()}"
        );

        UnlockCursor();

        if (movement != null) movement.enabled = false;
        if (interaction != null) interaction.enabled = false;
        if (characterController != null) characterController.enabled = false;
    }

    public void EnablePlayer(PlayerLockSource source = PlayerLockSource.Unknown)
    {
        UnityEngine.Debug.Log(
            $"<color=green>[Player ENABLED]</color> by <b>{source}</b>\n{GetStackTrace()}"
        );

        LockCursor();

        if (movement != null) movement.enabled = true;
        if (interaction != null) interaction.enabled = true;
        if (characterController != null) characterController.enabled = true;
    }

    // ---------------- Partial Control ----------------

    public void FreezeMovementOnly()
    {
        if (movement != null)
            movement.IsFrozen = true;
    }

    public void UnFreezeMovementOnly()
    {
        if (movement != null)
            movement.IsFrozen = false;
    }

    // ---------------- Spawn ----------------

    public void SetPlayerSpawnPoint(Transform spawnPoint)
    {
        if (characterController == null) return;

        characterController.enabled = false;

        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        if (movement != null)
            movement.ResetHead();

        characterController.enabled = true;
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // ---------------- Debug Helper ----------------

    private string GetStackTrace()
    {
        return new StackTrace(2, true).ToString();
    }
}



