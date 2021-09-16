using UnityEngine;

namespace ZenToolset.Example
{
    public class ExampleManagedUpdates : MonoBehaviour, IManagedUpdate, IManagedFixedUpdate, IManagedLateUpdate
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
            
            // Register self to the UpdateManager (only register when needed)
            UpdateManager.Instance.RegisterUpdate(this);
            UpdateManager.Instance.RegisterFixedUpdate(this);
            UpdateManager.Instance.RegisterLateUpdate(this);
        }

        private void OnDestroy()
        {
            // Unregister self when destroyed
            if (UpdateManager.Instance)
            {
                UpdateManager.Instance.UnregisterUpdate(this);
                UpdateManager.Instance.UnregisterFixedUpdate(this);
                UpdateManager.Instance.UnregisterLateUpdate(this);
            }
        }

        public void ManagedFixedUpdate()
        {
            // Rotate rigidbody
            Quaternion deltaRotation = Quaternion.Euler(eulerAngleVelocity * Time.deltaTime);
            rigidBody.MoveRotation(rigidBody.rotation * deltaRotation);
        }

        public void ManagedLateUpdate()
        {
            // Sine movement on X axis
            transform.position = new Vector3(Mathf.Sin(Time.time) * sineDistance + startPos.x, transform.position.y + startPos.y, startPos.z);
        }

        public void ManagedUpdate()
        {
            // Sine movement on Y axis
            transform.position = new Vector3(transform.position.x + startPos.x, Mathf.Sin(Time.time) * sineDistance + startPos.y, startPos.z);
        }
    }
}
