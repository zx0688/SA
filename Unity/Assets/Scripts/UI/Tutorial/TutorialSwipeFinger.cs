using DG.Tweening;
using UnityEngine;

public class TutorialSwipeFinger : MonoBehaviour
{

    [SerializeField] private BlackLayer layer;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Animate()
    {
        this.enabled = true;
        this.gameObject.SetActive(true);

        this.GetComponent<CanvasGroup>().DOKill();
        this.GetComponent<CanvasGroup>().alpha = 0;

        layer.Show(0f, () =>
        {
            this.GetComponent<CanvasGroup>().DOFade(1f, 0.2f);

            layer.Hide(2f, () =>
            {
                gameObject.SetActive(false);
            });

        });


    }
}
