using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ServiceBehaviour : MonoBehaviour
{
    protected virtual void Awake()
    {

        if (Services.isInited)
            OnServicesInited();
        else
            Services.OnInited += OnServicesInited;
    }

    protected virtual void OnServicesInited()
    {
        Services.OnInited -= OnServicesInited;
    }

}
