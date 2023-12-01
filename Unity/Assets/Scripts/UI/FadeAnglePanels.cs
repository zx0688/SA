using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace UI
{
    public class FadeAnglePanels : ServiceBehaviour
    {
        [SerializeField] private Image _topLeft;
        [SerializeField] private Image _topRight;
        [SerializeField] private Image _bottomLeft;
        [SerializeField] private Image _bottomRight;

        protected override void OnServicesInited()
        {
            base.OnServicesInited();
            FadeIn();
        }

        private void FadeIn()
        {
            _topLeft.GetComponent<RectTransform>().DOAnchorPos(new Vector2(150f, 766f), 0.5f).SetEase(Ease.OutBack);
            _topRight.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-150f, 766f), 0.5f).SetEase(Ease.OutBack);
            _bottomLeft.GetComponent<RectTransform>().DOAnchorPos(new Vector2(178f, -1001f), 0.5f).SetEase(Ease.OutBack);
            _bottomRight.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-178f, -1001f), 0.5f).SetEase(Ease.OutBack);
        }
    }
}
