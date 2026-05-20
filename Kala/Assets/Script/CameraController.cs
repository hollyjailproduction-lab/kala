using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;

    private float bottomYposition = 0.2214063f;
    private float cameraXposition ;
    private float cameraYposition ;
    // buat camera mengikuti player, tapi dengan batas bawah 0.2214063. Jadi kalau player di bawah 2, camera tetap di 3.15 (2 + 1.15)
    private void LateUpdate()
    {
        // buat fungsi camera agar lebih smooth, dengan menggunakan Lerp. Jadi camera akan mengikuti player dengan kecepatan tertentu, bukan langsung mengikuti player.
        if (player != null)
        {
            cameraXposition = player.position.x + 1f;
            cameraYposition = player.position.y;

            Vector3 targetPosition = new Vector3(cameraXposition, cameraYposition, transform.position.z);
            targetPosition.y  = (targetPosition.y <= bottomYposition) ? bottomYposition : targetPosition.y;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 5f);
        }
    }
}
