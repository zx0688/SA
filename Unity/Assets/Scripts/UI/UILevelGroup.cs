using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILevelGroup : MonoBehaviour
{
    [SerializeField] private RawImage[] stars;

    public void SetLevel(int level)
    {
        if (level == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
        for (int i = 0; i < stars.Length; i++)
            stars[i].gameObject.SetActive(i < level);
    }
}
