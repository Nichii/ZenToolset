using UnityEngine;

namespace ZenToolset.Example
{
    public class ExampleUnmanagedUpdates : MonoBehaviour
    {
        [SerializeField] private float sineDistance = 1f;
        
        private Rigidbody rigidBody = null;
        private Vector3 eulerAngleVelocity = new Vector3(0, 100, 0);
        private Vector3 startPos = Vector3.zero;

        private void Start()
        {
            // Get rigidbody component
            rigidBody = GetComponent<Rigidbody>();

            // Set start pos for sine movement later
            startPos = transform.position;
        }

        private void FixedUpdate()
        {
            // Rotate rigidbody
            Quaternion deltaRotation = Quaternion.Euler(eulerAngleVelocity * Time.deltaTime);
            rigidBody.MoveRotation(rigidBody.rotation * deltaRotation);
        }

        private void LateUpdate()
        {
            // Sine movement on X axis
            transform.position = new Vector3(Mathf.Sin(Time.time) * sineDistance + startPos.x, transform.position.y + startPos.y, startPos.z);
        }

        private void Update()
        {
            // Sine movement on Y axis
            transform.position = new Vector3(transform.position.x + startPos.x, Mathf.Sin(Time.time) * sineDistance + startPos.y, startPos.z);
        }
    }
}
