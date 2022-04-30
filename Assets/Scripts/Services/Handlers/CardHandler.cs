using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CardHandler
{

    private PlayerVO player;
    private DataService dataManager;

    delegate bool ActionTrigger(TriggerVO trigger, ActionData data, int startTime, int timestamp);
    private ActionTrigger actionTrigger;
    public CardHandler(PlayerVO playerVO)
    {
        this.player = playerVO;
        dataManager = Services.Data;
        actionTrigger = dataManager.ActionTrigger;
    }
    public int AvailableItem(int id, int type)
    {
        return player.cards.Find(c => c.id == id) != null ? 1 : 0;
    }

    private int CreateChoiceData(List<ActionData> choices, int time)
    {
        for (int i = 0; i < choices.Count; i++)
        {
            ActionData cd = choices[i];
            if (cd == null)
                continue;
            if (cd.Text == null && cd.Reward == null && cd.Con == null)
                continue;
            if (Services.Data.CheckConditions(cd.Con, time))
                return i + 1;
        }
        return 0;
    }

    public CardVO Add(CardData data, int count, int time)
    {
        CardVO cardVO = GetVO(data.Id, 0);
        if (cardVO == null)
        {
            cardVO = new CardVO();
            player.cards.Add(cardVO);
            cardVO.id = data.Id;
        }

        cardVO.activated = time;

        /*cardVO.left = CreateChoiceData(new List<ActionData>() {
                data.left1, data.left2, data.left3
            }, time);
        cardVO.right = CreateChoiceData(new List<ActionData>() {
                data.right1, data.right2, data.right3
            }, time);
*/
        return cardVO;
    }
    private void AddToQueue(CardData cardData, List<CardData> queue, List<CardData> candidates)
    {
        if (true)
        {
            if (cardData.Act.Chance > 0)
            {
                int chance = cardData.Act.Chance;// + Services.Player.skillHandler.TotalChance (null, cardData);
                if (UnityEngine.Random.Range(0, 100f) <= chance)
                    candidates.Add(cardData);
            }
            else
                candidates.Add(cardData);
        }
        else if (cardData.Act.Chance > 0)
        {
            int chance = cardData.Act.Chance;// + Services.Player.skillHandler.TotalChance (null, cardData);
            if (UnityEngine.Random.Range(0, 100f) <= chance)
                queue.Add(cardData);
        }
        else
            queue.Add(cardData);
    }

    public void Trigger(List<CardData> queue, TriggerVO trigger, List<RewardData> reward, int time)
    {

        ItemVO action = null;
        if (trigger.id > 0 && trigger.tp == TriggerData.CARD)
        {
            CardData cardData = trigger.swiped.CardData;
            ChoiceData choiceData = null;
            if (trigger.swiped.Left.action == null)
            {
                choiceData = trigger.swiped.Right;
            }
            else
            {
                choiceData = trigger.choice == Swipe.LEFT_CHOICE ? trigger.swiped.Left : trigger.swiped.Right;
            }

            /*if (choiceData.action.reward.Count > 0)
            {

                if (choiceData.action.chance == 0 || choiceData.chance >= UnityEngine.Random.Range(0f, 100f))
                {
                    Services.Data.ApplyReward(reward, choiceData.action.reward, 1f);
                    for (int i = 0; i < choiceData.action.reward.Count; i++)
                    {
                        if (choiceData.action.reward[i].tp == DataService.ACTION_ID)
                        {
                            action = new ItemVO(choiceData.action.reward[i].id, 1);
                            break;
                        }
                    }
                }
            }

            if (cardData.drops != null && cardData.drops.Count > 0)
                Services.Data.ApplyReward(reward, cardData.drops, 1f);

            if (cardData.nosave == false)
            {
                CardVO cardVO = GetVO(cardData.id, 0);
                cardVO.choice = trigger.choice;
                cardVO.executed = time;
                if (cardData.once == true)
                {
                    Utils.RemoveAt(ref dataManager.Game.cards, Array.IndexOf(dataManager.Game.cards, cardData));
                }
            }*/
        }

        List<CardData> candidates = new List<CardData>();
        for (int i = 0; i < dataManager.Game.Cards.Length; i++)
        {
            CardData cardData = dataManager.Game.Cards[i];
            if (queue.Exists(q => q.Id == cardData.Id))
                continue;
            if (!actionTrigger(trigger, cardData.Act, 0, time))
                continue;
            if (action != null && cardData.Act.Con.Exists(c => c.Tp == DataService.ACTION_ID && c.Id != action.id))
                continue;

            CardVO cardVO = GetVO(cardData.Id, 0);
            //if (dataManager.CheckOpenCondition(cardData, cardVO, time))
            //    AddToQueue(cardData, queue, candidates);
        }

        if (candidates.Count > 0)
        {
            int firstPrior = 0;
            for (int i = 0; i < candidates.Count; i++)
            {
                if (candidates[i].Pri > firstPrior)
                    firstPrior = candidates[i].Pri;
            }
            List<CardData> firstPriorList = candidates.Where(c => c.Pri == firstPrior).ToList();
            queue.Add(firstPriorList[UnityEngine.Random.Range(0, firstPriorList.Count)]);
        }

    }

    public CardVO GetVO(int id, int type)
    {
        for (int i = 0; i < player.cards.Count; i++)
        {
            if (player.cards[i].id == id)
                return player.cards[i];
        }
        return null;
    }

    public CardVO Change(CardData data, int time)
    {
        throw new System.NotImplementedException();
    }
}