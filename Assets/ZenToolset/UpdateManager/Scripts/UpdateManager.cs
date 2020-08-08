using System.Collections.Generic;

namespace ZenToolset
{
    /// <summary>
    /// A singleton that can manage every game object's Update, FixedUpdate and LateUpdate that register itself to it.
    /// Its dependency is ZenToolset.Singleton
    /// </summary>
    public class UpdateManager : Singleton<UpdateManager>
    {
        // Update lists
        private readonly List<IManagedUpdate> updateList = new List<IManagedUpdate>();
        private readonly List<IManagedFixedUpdate> fixedUpdateList = new List<IManagedFixedUpdate>();
        private readonly List<IManagedLateUpdate> lateUpdateList = new List<IManagedLateUpdate>();

        private bool isUpdateListChanged = false;
        private bool isFixedUpdateListChanged = false;
        private bool isLateUpdateListChanged = false;

        // Update arrays
        private IManagedUpdate[] updateArray = null;
        private IManagedFixedUpdate[] fixedUpdateArray = null;
        private IManagedLateUpdate[] lateUpdateArray = null;

        /// <summary>
        /// Registers a script with IManagedUpdate to have its ManagedUpdate() called by UpdateManager
        /// </summary>
        /// <param name="managedUpdate">Script that implements IManagedUpdate</param>
        public void RegisterUpdate(IManagedUpdate managedUpdate)
        {
            if (managedUpdate == null) return;
            updateList.Add(managedUpdate);
            isUpdateListChanged = true;
        }

        public void RegisterFixedUpdate(IManagedFixedUpdate managedFixedUpdate)
        {
            if (managedFixedUpdate == null) return;
            fixedUpdateList.Add(managedFixedUpdate);
            isFixedUpdateListChanged = true;
        }

        public void RegisterLateUpdate(IManagedLateUpdate managedLateUpdate)
        {
            if (managedLateUpdate == null) return;
            lateUpdateList.Add(managedLateUpdate);
            isLateUpdateListChanged = true;
        }

        public void UnregisterUpdate(IManagedUpdate managedUpdate)
        {
            if (updateList.Remove(managedUpdate))
            {
                isUpdateListChanged = true;
            }
        }

        public void UnregisterFixedUpdate(IManagedFixedUpdate managedFixedUpdate)
        {
            if (fixedUpdateList.Remove(managedFixedUpdate))
            {
                isFixedUpdateListChanged = true;
            }
        }

        public void UnregisterLateUpdate(IManagedLateUpdate managedLateUpdate)
        {
            if (lateUpdateList.Remove(managedLateUpdate))
            {
                isLateUpdateListChanged = true;
            }
        }

        private void Update()
        {
            // Detect if the array needs to be updated
            if (isUpdateListChanged || updateArray == null)
            {
                updateArray = updateList.ToArray();
                isUpdateListChanged = false;
            }

            bool triggerCleanup = false;
            
            // Runs the update function
            for (int i = 0; i < updateArray.Length; i++)
            {
                if (updateArray[i] == null)
                {
                    triggerCleanup = true;
                    continue;
                }

                updateArray[i].ManagedUpdate();
            }

            // Clean up any null on the list
            if (triggerCleanup)
            {
                updateList.RemoveAll(null);
                isUpdateListChanged = true;
            }
        }

        private void FixedUpdate()
        {
            // Detect if the array needs to be updated
            if (isFixedUpdateListChanged || fixedUpdateArray == null)
            {
                fixedUpdateArray = fixedUpdateList.ToArray();
                isFixedUpdateListChanged = false;
            }
            
            bool triggerCleanup = false;

            // Runs the fixed update function
            for (int i = 0; i < fixedUpdateArray.Length; i++)
            {
                if (fixedUpdateArray[i] == null)
                {
                    triggerCleanup = true;
                    continue;
                }

                fixedUpdateArray[i].ManagedFixedUpdate();
            }

            // Clean up any null on the list
            if (triggerCleanup)
            {
                fixedUpdateList.RemoveAll(null);
                isFixedUpdateListChanged = true;
            }
        }

        private void LateUpdate()
        {
            // Detect if the array needs to be updated
            if (isLateUpdateListChanged || lateUpdateArray == null)
            {
                lateUpdateArray = lateUpdateList.ToArray();
                isLateUpdateListChanged = false;
            }

            bool triggerCleanup = false;

            // Runs the late update function
            for (int i = 0; i < lateUpdateArray.Length; i++)
            {
                if (lateUpdateArray[i] == null)
                {
                    triggerCleanup = true;
                    continue;
                }

                lateUpdateArray[i].ManagedLateUpdate();
            }

            // Clean up any null on the list
            if (triggerCleanup)
            {
                lateUpdateList.RemoveAll(null);
                isLateUpdateListChanged = true;
            }
        }
    }
}
