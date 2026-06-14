using UnityEngine;
using TMPro;
using Unity.Netcode; // Essential

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    // NetworkVariables automatically synchronize integers across all connected clients
    [Header("Scores")]
    public NetworkVariable<int> p1Score = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> p2Score = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public int scoreToWin = 10;

    [Header("Timer Mechanic")]
    public float timeRemaining = 60f;
    private bool gameIsOver = false;
    private bool gameHasStarted = false; 

    [Header("UI Elements")]
    public TextMeshProUGUI p1ScoreText;
    public TextMeshProUGUI p2ScoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI winText;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Listen for changes to the scores to update text on both screens automatically
        p1Score.OnValueChanged += (oldVal, newVal) => p1ScoreText.text = "P1: " + newVal;
        p2Score.OnValueChanged += (oldVal, newVal) => p2ScoreText.text = "P2: " + newVal;
    }

    void Update()
    {
        if (!gameHasStarted || gameIsOver) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            timerText.text = "Time: " + Mathf.CeilToInt(timeRemaining).ToString();
        }
        else
        {
            if (IsServer) EndGameByTimeServerRpc();
        }
    }

    public void StartGameTimer()
    {
        gameHasStarted = true;
    }

    // FIXED: Changed parameter from 'string playerTag' to 'ulong clientId'
    public void AddScore(ulong clientId, int points)
    {
        // Only the server is authorized to change a NetworkVariable
        if (!IsServer || gameIsOver) return;

        // In Unity Netcode, Client ID 0 is ALWAYS the Host (Player 1)
        if (clientId == 0)
        {
            p1Score.Value += points;
            if (p1Score.Value >= scoreToWin) DeclareWinnerClientRpc("Player 1 Wins!");
        }
        // Any other Client ID (usually 1) belongs to Player 2
        else
        {
            p2Score.Value += points;
            if (p2Score.Value >= scoreToWin) DeclareWinnerClientRpc("Player 2 Wins!");
        }
    }

    [ServerRpc]
    void EndGameByTimeServerRpc()
    {
        if (p1Score.Value > p2Score.Value) DeclareWinnerClientRpc("Player 1 Wins!");
        else if (p2Score.Value > p1Score.Value) DeclareWinnerClientRpc("Player 2 Wins!");
        else DeclareWinnerClientRpc("It's a Tie!");
    }

    // A ClientRpc forces code to execute on EVERY connected player's screen simultaneously
    [ClientRpc]
    void DeclareWinnerClientRpc(string message)
    {
        gameIsOver = true;
        winText.gameObject.SetActive(true);
        winText.text = message;
        Time.timeScale = 0f; 
    }
}