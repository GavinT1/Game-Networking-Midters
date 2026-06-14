using UnityEngine;
using Unity.Netcode;

public class NetworkMenuUI : MonoBehaviour
{
    private string joinCode = "123456";

    void OnGUI()
    {
        // 1. Starts the layout area
        GUILayout.BeginArea(new Rect(20, 20, 250, 400));

        if (NetworkManager.Singleton == null)
        {
            GUILayout.Label("Waiting for NetworkManager...");
            GUILayout.EndArea(); // Safely exit if it's missing
            return;
        }

        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            GUILayout.Label("=== MULTIPLAYER MODE ===");
            
            if (GUILayout.Button("Host Locally (Player 1)"))
            {
                NetworkManager.Singleton.StartHost();
                GameManager.Instance.StartGameTimer();
            }

            if (GUILayout.Button("Join Locally (Player 2)"))
            {
                NetworkManager.Singleton.StartClient();
                GameManager.Instance.StartGameTimer();
            }

            GUILayout.Space(20);
            GUILayout.Label("Enter 6-Digit Room Code:");
            joinCode = GUILayout.TextField(joinCode, 6);

            if (GUILayout.Button("Create Room via Code"))
            {
                NetworkManager.Singleton.StartHost(); 
                GameManager.Instance.StartGameTimer();
            }

            if (GUILayout.Button("Join Room via Code"))
            {
                NetworkManager.Singleton.StartClient();
                GameManager.Instance.StartGameTimer();
            }
        }
        else
        {
            GUILayout.Label("Connected status: Active");
            if (GUILayout.Button("Disconnect"))
            {
                NetworkManager.Singleton.Shutdown();
            }
        }

        // 2. MATCHING END TAG: Closes the layout area perfectly
        GUILayout.EndArea(); 
    }
}