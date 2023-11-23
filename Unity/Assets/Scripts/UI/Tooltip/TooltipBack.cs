using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipBack : MonoBehaviour
{
    [SerializeField] private GameObject yellow;
    [SerializeField] private GameObject red;
    [SerializeField] private GameObject pink;
    [SerializeField] private GameObject green;
    [SerializeField] private GameObject blue;
    [SerializeField] private Text title;
    [SerializeField] private BlackLayer blackBack;

    public void Show(string back, string title)
    {
        this.title.text = title;

        gameObject.SetActive(true);

        blackBack.Show();

        yellow.SetActive(false);
        red.SetActive(false);
        pink.SetActive(false);
        green.SetActive(false);
        blue.SetActive(false);

        switch (back)
        {
            case "yellow":
                yellow.gameObject.SetActive(true);
                break;
            case "green":
                green.gameObject.SetActive(true);
                break;
            case "pink":
                pink.gameObject.SetActive(true);
                break;
            case "red":
                red.gameObject.SetActive(true);
                break;
            case "blue":
                blue.gameObject.SetActive(true);
                break;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        blackBack.Hide();
    }


}
