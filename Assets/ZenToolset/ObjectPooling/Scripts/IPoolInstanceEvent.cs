using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZenToolset
{
    public interface IPoolInstanceEvent
    {
        void OnPoolSpawned();
        void OnPoolDespawned();
    }
}
