using UnityEngine;
using Unity.Netcode; // Make sure this is added at the top

public class PlayerController : NetworkBehaviour
{
    public float speed = 5f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            if (IsServer)
            {
                gameObject.tag = "Player1";
                Debug.Log("Successfully tagged this local player as: Player1");
            }
            else
            {
                gameObject.tag = "Player2";
                Debug.Log("Successfully tagged this local player as: Player2");
            }
        }
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(moveX, 0f, moveZ).normalized * speed;
        rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);
    }
}