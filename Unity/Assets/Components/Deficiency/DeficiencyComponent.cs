using System;
using System.Collections;
using System.Collections.Generic;
using Controllers;
using Meta;
using UnityEngine;

public class DeficiencyComponent : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject deficiency;

    [SerializeField]
    private int id;

    [SerializeField]
    private bool enemy = false;

    //private int id;
    void Start()
    {

        ItemStateController itemStateController = GetComponent<ItemStateController>();

        if (itemStateController != null)
        {
            id = itemStateController.itemId;
        }

        deficiency.SetActive(false);

        //CardController.OnUnavailableCondition += show;
    }

    private void show(ConditionMeta c)
    {
        //if (c.Tp != DataService.ITEM_ID || c.Id != id)
        //    return;

        deficiency.SetActive(true);

        deficiency.GetComponent<Animator>().SetTrigger("fadein");
    }

    // Update is called once per frame
    void Update()
    {

    }
}