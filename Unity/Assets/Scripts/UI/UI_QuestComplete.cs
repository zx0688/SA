using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

using UnityEngine;

namespace UI
{
    public class UI_QuestComplete : ServiceBehaviour
    {
        // Start is called before the first frame update
        private List<CardMeta> queue;
        private CardMeta current;
        private bool isPlaying;

        protected override void OnServicesInited()
        {
            base.OnServicesInited();
            Services.Player.OnQuestComplete += OnQuestComplete;
        }

        private void OnQuestComplete()
        {
            //queue.Add();

            if (isPlaying == false)
            {
                current = queue[0];
                UpdateUI();
                PlayAnimation();
            }

        }

        private void PlayAnimation()
        {
            isPlaying = true;
            gameObject.SetActive(true);
            gameObject.GetComponent<RectTransform>().DOAnchorPosY(0f, 1f).SetEase(Ease.OutCirc).OnComplete(() =>
            {
                //animation reward;
                if (queue.Count == 0)
                {
                    gameObject.SetActive(false);
                    isPlaying = false;
                }
                else
                {
                    queue.Remove(current);
                    current = queue[0];
                    UpdateUI();
                    PlayAnimation();
                }

            });
        }

        void UpdateUI()
        {

        }

        void Start()
        {
            queue = new List<CardMeta>();
            isPlaying = false;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}