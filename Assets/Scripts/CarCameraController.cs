using UnityEngine;

public class CarCameraController : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Rigidbody rb;
    [SerializeField] float speed;
    [SerializeField] Vector3 offset;

    // Camera Stutters at speed 
    Vector3 targetforward => (target.forward + rb.velocity).normalized;
    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(
            transform.position, 
            target.position + target.TransformVector(offset) + targetforward * (-5f),
            speed * Time.deltaTime);

        transform.LookAt(target);
    }

    
}