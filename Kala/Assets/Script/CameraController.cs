using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;


    private void LateUpdate()
    {
        if (player != null)
        {
            Vector3 newPosition = new Vector3(player.position.x+1f, player.position.y+0.9f, transform.position.z);
            transform.position = newPosition;
        }
    }
}
