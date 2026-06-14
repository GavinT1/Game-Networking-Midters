using UnityEngine;
using TMPro;
using System.Collections; 
using Unity.Netcode; 

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    [Header("Scores")]
    public NetworkVariable<int> p1Score = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> p2Score = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public int scoreToWin = 10;

    [Header("Timer Mechanic")]
    public float timeRemaining = 60f;
    private bool gameIsOver = false;
    private bool gameHasStarted = false; 
    private bool lobbyWaiting = true; 

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
        p1Score.OnValueChanged += (oldVal, newVal) => p1ScoreText.text = "P1: " + newVal;
        p2Score.OnValueChanged += (oldVal, newVal) => p2ScoreText.text = "P2: " + newVal;
        
        timerText.text = "Waiting for Player 2...";
    }

    void Update()
    {
       
        if (IsServer && lobbyWaiting)
        {
            if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
            {
                lobbyWaiting = false; 
                StartCountdownClientRpc(); 
            }
        }

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

    [ClientRpc]
    void StartCountdownClientRpc()
    {
        StartCoroutine(LobbyCountdownRoutine());
    }

    IEnumerator LobbyCountdownRoutine()
    {
        winText.gameObject.SetActive(true);
        
        winText.text = "PLAYER 2 JOINED!\n3";
        yield return new WaitForSeconds(1f);
        
        winText.text = "2";
        yield return new WaitForSeconds(1f);
        
        winText.text = "1";
        yield return new WaitForSeconds(1f);
        
        winText.text = "GO!";
        yield return new WaitForSeconds(0.5f); 
        
        winText.gameObject.SetActive(false); 

        if (IsServer && CoinSpawner.Instance != null)
        {
            CoinSpawner.Instance.SpawnAllCoins();
        }
        // -------------------------------

        gameHasStarted = true; 
    }

    public bool IsMatchActive()
    {
        return gameHasStarted && !gameIsOver;
    }

    public void AddScore(ulong clientId, int points)
    {
        if (!IsServer || gameIsOver) return;

        if (clientId == 0)
        {
            p1Score.Value += points;
            if (p1Score.Value >= scoreToWin) DeclareWinnerClientRpc("Player 1 Wins!");
        }
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

    [ClientRpc]
    void DeclareWinnerClientRpc(string message)
    {
        gameIsOver = true;
        winText.gameObject.SetActive(true);
        winText.text = message;
        Time.timeScale = 0f; 
    }
}