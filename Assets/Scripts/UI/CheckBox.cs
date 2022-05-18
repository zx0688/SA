using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckBox : MonoBehaviour
{
    [SerializeField] private Image _check;
    [SerializeField] private Button _button;

    public Action<Boolean> OnTrigger;

    public bool IsToggled;

    void Awake()
    {
        _button = GetComponent<Button>();
    }

    void Start()
    {
        IsToggled = false;
        _button.onClick.AddListener(OnToggle);
        _check.gameObject.SetActive(IsToggled);
    }

    private void OnToggle()
    {
        IsToggled = !IsToggled;
        _check.gameObject.SetActive(IsToggled);

        OnTrigger?.Invoke(IsToggled);
    }
}
