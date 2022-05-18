using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class UI_InventoryItemUpdate : UI_InventoryItem
{

    [SerializeField]
    public int id = 0;

    protected void Init()
    {

        Services.OnInited -= Init;
        // Services.Player.OnProfileUpdated += OnProfileUpdated;
        // OnProfileUpdated();
    }

    /*protected override void Awake()
    {

        base.Awake();

        if (Services.isInited)
            Init();
        else
            Services.OnInited += Init;
    }*/

    void OnEnable()
    {
        if (Services.isInited)
            OnProfileUpdated();
    }

    private void OnProfileUpdated()
    {
        if (id == 0 || gameObject.activeInHierarchy == false)
            return;

        ItemVO item = null;//Services.Player.itemHandler.GetVO(id, 3);
        SetItem(item);

    }

}