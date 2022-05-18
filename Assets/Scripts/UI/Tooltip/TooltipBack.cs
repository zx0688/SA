using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipBack : MonoBehaviour
{
    [SerializeField] private GameObject _yellow;
    [SerializeField] private GameObject _red;
    [SerializeField] private GameObject _pink;
    [SerializeField] private GameObject _green;
    [SerializeField] private Text _title;

    void Awake()
    {
        //_title = transform.Find("Title").GetComponent<Text>();
    }

    public void Show(string back, string title)
    {
        _title.text = title;

        gameObject.SetActive(true);

        BlackLayer.Instance.Show();

        _yellow.SetActive(false);
        _red.SetActive(false);
        _pink.SetActive(false);
        _green.SetActive(false);

        switch (back)
        {
            case "yellow":
                _yellow.gameObject.SetActive(true);
                break;
            case "green":
                _green.gameObject.SetActive(true);
                break;
            case "pink":
                _pink.gameObject.SetActive(true);
                break;
            case "red":
                _red.gameObject.SetActive(true);
                break;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        BlackLayer.Instance.Hide();
    }


}
