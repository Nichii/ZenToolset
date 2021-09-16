using UnityEngine;

namespace ZenToolset.Example
{
    /// <summary>
    /// This is a standalone example on how to map cross platform controllers into a circle input
    /// </summary>
    public class TestJoystickClamping : MonoBehaviour
    {
        [SerializeField] private GameObject visualPosObj = null;
        [SerializeField] private float maxAxisDistance = 3f;

        private void Update()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // This will make sure the input stay in a circle
            Vector2 input = Vector2.ClampMagnitude(new Vector2(horizontal, vertical), 1f);

            visualPosObj.transform.position = input * maxAxisDistance;
        }
    }
}
