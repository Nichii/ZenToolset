using System.Collections;
using UnityEngine;

namespace ZenToolset.Example
{
    public class CoroutineState : State
    {
        [SerializeField] private GameObject stateOneObj = null;
        [SerializeField] private float delay = 1f;
        [SerializeField] private State nextState = null;

        private WaitForSeconds wait = null;

        public override void OnStateEnter()
        {
#if UNITY_EDITOR
            Debug.Log("<b><color=#3498db>It's currently COROUTINE STATE!</color></b>", this);
#endif
            stateOneObj.SetActive(true);
            StartCoroutine(DelayedStateChange());
        }

        public override void OnStateExit()
        {
            stateOneObj.SetActive(false);
        }

        private IEnumerator DelayedStateChange()
        {
            yield return wait;

            ChangeState(nextState);
        }

        private void Start()
        {
            wait = new WaitForSeconds(delay);
        }
    }
}
