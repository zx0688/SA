using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class UI_InventoryItemUpdate : UIInventoryItem
{

    [SerializeField]
    public int id = 0;

    protected void Init()
    {

        //ervices.OnInited -= Init;
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

        //ItemData item = null;//Services.Player.itemHandler.GetVO(id, 3);
        //SetItem(item);

    }

}