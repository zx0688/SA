using System.Collections;
using System.Collections.Generic;
using Meta;
using UnityEngine;


public class QuestHandler
{

    private PlayerVO player;
    private DataService dataManager;

    //delegate bool ActionTrigger(TriggerVO trigger, ActionData data, int startTime, int timestamp);
    //private ActionTrigger actionTrigger;

    public QuestHandler(PlayerVO playerVO)
    {
        this.player = playerVO;
        dataManager = Services.Data;
        //  actionTrigger = dataManager.ActionTrigger;
    }
    public int AvailableItem(int id, int type)
    {
        return player.quests.Find(c => c.id == id) != null ? 1 : 0;
    }

    public QuestVO Add(CardData data, int count, int time)
    {

        QuestVO questVO = player.quests.Find(q => q.id == data.Id);
        if (questVO == null)
        {
            questVO = new QuestVO();
            player.quests.Add(questVO);
        }
        if (true)
        {
            questVO.id = data.Id;
            questVO.activated = time;
            questVO.rew1 = questVO.rew2 = false;
            questVO.state = 0;
            questVO.executed = 0;
        }
        return questVO;
    }

    /*
        public void Trigger(List<CardData> queue, TriggerVO trigger, List<RewardData> reward, int time)
        {
            CardData[] quests = dataManager.Game.Quests;
            for (int i = 0; i < quests.Length; i++)
            {
                CardData questData = quests[i];
                QuestVO questVO = GetVO(questData.Id, 0);
                /* 
                                if (questVO != null && questVO.state == QuestVO.STATE_ACTIVE) {
                                    if (questData.act.time > 0 && GameTime.TimeLeft (time, questVO.activated, questData.act.time) <= 0) {
                                        questVO.state = QuestVO.STATE_EXPIRED;
                                        questVO.executed = time;
                                        CardData _cardData = CreateCard (questData, questVO, 4, 0);
                                        _cardData.prior = questData.prior + 1;
                                        // _cardData.left.reward = _cardData.right.reward = new List<RewardData> (questData.up.reward);
                                        queue.Add (_cardData);
                                    } else if (dataManager.ActionTrigger (triggers, questData.up, 0, time)) {
                                        questVO.state = QuestVO.STATE_FAILED;
                                        questVO.executed = time;
                                        CardData _cardData = CreateCard (questData, questVO, 5, 0);
                                        _cardData.prior = questData.prior + 1;
                                        // _cardData.left.reward = _cardData.right.reward = new List<RewardData> (questData.up.reward);
                                        queue.Add (_cardData);

                                    } else {

                                        bool noSecondStage = false; //(questData.right.trigg == null && questData.right.condi == null);
                                        if (questVO.rew1 == false) // && dataManager.ActionTrigger (triggers, questData.left, 0, time)) {
                                            questVO.rew1 = true;
                                        if (!noSecondStage) {
                                            CardData _cardData = CreateCard (questData, questVO, 1, 0);
                                            //_cardData.left.reward = _cardData.right.reward = new List<RewardData> (questData.left.reward);
                                            _cardData.prior = questData.prior + 5;
                                            queue.Add (_cardData);
                                        }
                                    }
                                    if (questVO.rew2 == false && dataManager.ActionTrigger (triggers, questData.right, 0, time)) {
                                        CardData _cardData = CreateCard (questData, questVO, 2, 0);
                                        _cardData.prior = questData.prior + 4;
                                        // _cardData.left.reward = _cardData.right.reward = new List<RewardData> (questData.right.reward);
                                        questVO.rew2 = true;
                                        queue.Add (_cardData);
                                    }
                                    if (questVO.rew1 == true && (questVO.rew2 == true || noSecondStage)) {
                                        questVO.state = QuestVO.STATE_COMPLETE;
                                        questVO.executed = time;
                                        CardData _cardData = CreateCard (questData, questVO, 3, 0);
                                        _cardData.prior = questData.prior + 3;
                                        // _cardData.left.reward = _cardData.right.reward = new List<RewardData> (questData.act.reward);
                                        queue.Add (_cardData);
                                    }
                                }
                            }
                            if (actionTrigger (triggers, questData.act, 0, time))
                                if (dataManager.CheckOpenCondition (questData, questVO, time)) {
                                    //triggers.Add(new TriggerVO(TriggerData.QUEST, questData.id, 0, null, questData, null));
                                    queue.Add (CreateCard (questData, null, 0, 0));
                                }

            }
        }
    */
    public CardData CreateCard(CardData data, QuestVO vo, int state, int time)
    {
        CardData cardData = new CardData();
        cardData.Id = data.Id;
        cardData.Pri = data.Pri;
        //cardData.Typ = 1;
        return cardData;
    }

    public QuestVO GetVO(int id, int type)
    {
        return player.quests.Find(_r => _r.id == id);
    }

    public QuestVO Change(CardData data, int time)
    {
        throw new System.NotImplementedException();
    }
}