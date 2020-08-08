using UnityEngine;

namespace ZenToolset.Example
{
    public class UpdateState : State
    {
        [SerializeField] private GameObject stateTwoObj = null;
        [SerializeField] private float delay = 1f;
        [SerializeField] private State nextState = null;

        private float timer = 0f;

        public override void OnStateEnter()
        {
#if UNITY_EDITOR
            Debug.Log("<b><color=#8e44ad>It's currently UPDATE STATE!</color></b>", this);
#endif
            stateTwoObj.SetActive(true);
            timer = delay;
        }

        public override void OnStateExecute()
        {
            timer = Mathf.Max(0f, timer - Time.deltaTime);

            if (timer <= 0f)
            {
                ChangeState(nextState);
            }
        }

        public override void OnStateExit()
        {
            stateTwoObj.SetActive(false);
        }

        private void Start()
        {
            stateTwoObj.SetActive(false);
        }
    }
}
