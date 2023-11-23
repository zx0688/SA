using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class UI_FadeInPanels : ServiceBehaviour
    {
        // Start is called before the first frame update

        protected override void OnServicesInited()
        {
            base.OnServicesInited();

            Transform topPanel = transform.Find("UI_TopPanel");
            Transform bottomPanel = transform.Find("UI_BottomPanel");

            topPanel.gameObject.GetComponent<RectTransform>().DOAnchorPosY(0f, 0.2f).SetEase(Ease.OutCirc).SetAutoKill();
            bottomPanel.gameObject.GetComponent<RectTransform>().DOAnchorPosY(0f, 0.2f).SetEase(Ease.OutCirc).SetAutoKill();
        }

        void Start()
        {

        }

    }
}