using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizeText : ServiceBehaviour
{
    [SerializeField] private string key;

    protected override void OnServicesInited()
    {
        base.OnServicesInited();

        Text c = null;
        if (!TryGetComponent<Text>(out c))
            throw new Exception("failed localization process: no TextComponent found");

        c.Localize(key, LocalizePartEnum.GUI);
    }
}
