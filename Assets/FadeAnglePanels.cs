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

        protected virtual void OnServicesInited()
        {
            base.OnServicesInited();
            FadeIn();
        }

        private void FadeIn()
        {
            _topLeft.transform.DOMove(new Vector3(0f, 0f, 0f), 0.2f, true);
            _topRight.transform.DOMove(new Vector3(0f, 0f, 0f), 0.2f, true);
            _bottomLeft.transform.DOMove(new Vector3(0f, 0f, 0f), 0.2f, true);
            _bottomRight.transform.DOMove(new Vector3(0f, 0f, 0f), 0.2f, true);
        }
    }
}
