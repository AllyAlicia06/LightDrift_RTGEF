using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    private enum GameState { Playing, Paused, Dead, Won }
    
    [Header("Input")]
    [SerializeField] private InputActionReference pauseAction;
    
    [Header("UI References")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject crosshair;
    
    [Header("Gameplay References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerBallThrower playerBallThrower;
    [SerializeField] private Behaviour cameraInputController;
    
    private GameState currentState;
    
    private void OnEnable()
    {
        if (pauseAction != null)
            pauseAction.action.Enable();
    }
    
    private void OnDisable()
    {
        if (pauseAction != null)
            pauseAction.action.Disable();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChangeState(GameState.Playing);
    }

    // Update is called once per frame
    void Update()
    {
        if (pauseAction == null || !pauseAction.action.WasPressedThisFrame())
            return;
        
        if (currentState == GameState.Playing)
            PauseGame();
        else if (currentState == GameState.Paused)
            ResumeGame();
    }
    
    public void PauseGame()
    {
        if (currentState != GameState.Playing)
            return;
        
        ChangeState(GameState.Paused);
    }
    
    public void ResumeGame()
    {
        if (currentState != GameState.Paused)
            return;
        
        ChangeState(GameState.Playing);
    }
    
    public void ShowDeath()
    {
        if (currentState == GameState.Dead || currentState == GameState.Won)
            return;
        
        ChangeState(GameState.Dead);
    }
    
    public void ShowVictory()
    {
        if (currentState == GameState.Dead || currentState == GameState.Won)
            return;
        
        ChangeState(GameState.Won);
    }
    
    private void ChangeState(GameState newState)
    {
        currentState = newState;
        bool isPlaying = currentState == GameState.Playing;
        
        if (pausePanel != null)
            pausePanel.SetActive(currentState == GameState.Paused);
        
        if (deathPanel != null)
            deathPanel.SetActive(currentState == GameState.Dead);
        
        if (victoryPanel != null)
            victoryPanel.SetActive(currentState == GameState.Won);
        
        if (crosshair != null)
            crosshair.SetActive(isPlaying);
        
        if (playerMovement != null)
            playerMovement.enabled = isPlaying;
        
        if (playerBallThrower != null)
            playerBallThrower.enabled = isPlaying;
        
        if (cameraInputController != null)
            cameraInputController.enabled = isPlaying;
        
        Time.timeScale = isPlaying ? 1f : 0f;
        
        Cursor.lockState = isPlaying ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isPlaying;
    }
    
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void QuitGame()
    {
        Time.timeScale = 1f;
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    
    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
