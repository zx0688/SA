using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Assets.SimpleLocalization;
using Core;
using Cysharp.Threading.Tasks;
using Meta;
using UnityEngine;
using UnityEngine.Networking;

public partial class PlayerService
{
    public SwipeData SwipeData { get; private set; }

    private bool CheckTrigger(int Tp, int Id, int choice, ActionMeta action) =>
        Services.Data.CheckTrigger(Tp, Id, choice, action, this);
    private bool CheckConditions(List<ConditionMeta> conditions, CardMeta cardMeta, CardVO cardVO, List<RewardMeta> reward)
        => Services.Data.CheckConditions(conditions, cardMeta, cardVO, this, reward);

    private static List<CardMeta> CANDIDATES = new List<CardMeta>();

    public void CreateSwipeData()
    {
        if (playerVO.Layers.Count == 0)
            throw new Exception("=== GAME: Have no card in queue");

        CardMeta currentCardMeta = Services.Data.GetCardMetaByID(playerVO.Layers.Last());
        CardVO currentCardVO = GetCardVOByIDOrCreate(currentCardMeta.Id);

#if UNITY_EDITOR
        Debug.Log("CREATE CARD:" + playerVO.Layers.ToArray().ToJson());
#endif
        //set new card
        SwipeData.Choice = -1;
        SwipeData.CurrentCardMeta = currentCardMeta;
        SwipeData.CurrentCardVO = currentCardVO;
        SwipeData.PrevLayer = playerVO.Layers.Count > 1 ?
            Services.Data.GetCardMetaByID(playerVO.Layers[playerVO.Layers.Count - 2]) : null;

        CreateChoiceView();



        OnCardActivated?.Invoke(currentCardMeta, currentCardVO);
    }

    public void ApplyChangeLocation(CardMeta location)
    {
        playerVO.Location = location.Id;

        if (false)//!CheckConditions(location.Act.Con, location, null, DataService.EMPTY_REWARD))
        {
            throw new Exception("contition do not right for change location");
        }

        OnChangedLocation?.Invoke(location);
        OnProfileUpdated?.Invoke();
    }

    public void ApplyReroll(int timestamp, int accelerateValue = 0)
    {
        playerVO.SwipeReroll = playerVO.SwipeCount;

        ActivateNextCard(Services.Data.CardDeckMeta
            .Where(cm => CheckTrigger(TriggerMeta.REROLL, 0, 0, cm.Act) && CheckConditions(cm.Act.Con, cm, null, null))
            .PickRandom());

        InitLeftRightChoice();

        if (accelerateValue > 0)
        {
            //RewardMeta r = Services.Data.GameMeta.Config.ItemReroll[0].Clone();
            //r.Count = accelerateValue;

            //wwwApplyReward(new List<RewardMeta>() { r });
        }

    }

    public void ApplySwipe(SwipeData swipeData, Action emptyDeck)
    {
        CardVO cardVO = swipeData.CurrentCardVO;
        CardMeta cardMeta = swipeData.CurrentCardMeta;

        CardMeta nextCardMeta = swipeData.Choice == Swipe.LEFT_CHOICE ? swipeData.Left.NextCard : swipeData.Right.NextCard;

        playerVO.Layers.RemoveAt(playerVO.Layers.Count - 1);
        playerVO.SwipeCount++;

        cardVO.Count++;
        cardVO.Choice = nextCardMeta != null ? nextCardMeta.Id : -1;
        cardVO.Executed = playerVO.SwipeCount;

        if (nextCardMeta != null)
        {
            ActivateNextCard(nextCardMeta);
        }
        else
        {

            //throw new Exception("there should be card");
        }

        Services.Data.CardDeckMeta.Where(cm =>
                cm.Event == true &&
                CheckTrigger(GameMeta.CARD, cardMeta.Id, swipeData.Choice, cm.Act) &&
                CheckConditions(cm.Act.Con, cm, GetCardVOByID(cm.Id), null))
            .OrderBy(cm => cm.Pri)
            .ToList()
            .ForEach(cm => ActivateNextCard(cm));

        InitLeftRightChoice();
        nextCardMeta = playerVO.Layers.Count > 0 ? Services.Data.GetCardMetaByID(playerVO.Layers.Last()) : null;

        /*if (nextCardMeta != null)
        {
            CardVO nextCardVO = GetCardVOByIDOrCreate(nextCardMeta.Id);
            nextCardVO.Activated = playerVO.SwipeCount;
        }*/

        //if (cardMeta.Once == true)
        //    Utils.RemoveAt(ref Services.Data.GetMetaCardDeck, Array.IndexOf(Services.Data.GameMeta.Cards, cardMeta));
        List<RewardMeta> reward = swipeData.Choice == Swipe.LEFT_CHOICE ? swipeData.Left.Reward : swipeData.Right.Reward;
        // List<RewardMeta> reward = nextCardMeta?.Reward ?? new List<RewardMeta>();
        reward = reward.Select(r => r.Clone()).ToList();
        ApplyReward(reward);

        OnCardExecuted?.Invoke(cardMeta, cardVO);


        if (playerVO.Layers.Count == 0)
        {
            playerVO.TimestampStartReroll = GameTime.Current;
            emptyDeck.Invoke();
        }

    }

    private void ApplyReward(List<RewardMeta> reward)
    {
        for (int i = 0; i < reward.Count; i++)
        {
            RewardMeta r = reward[i];
            switch (r.Tp)
            {
                case GameMeta.ITEM:
                    ApplyItemReward(r);
                    break;
                case GameMeta.SKILL:
                    ApplySkillReward(r);
                    break;

            }
        }

        OnItemReceived?.Invoke(reward);
    }

    private void ApplyItemReward(RewardMeta reward)
    {
        if (reward.Random.Length > 0)
            reward.Id = reward.Random.PickRandom();

        ItemVO itemVO = GetItemVOByID(reward.Id);
        if (itemVO == null)
        {
            itemVO = new ItemVO(reward.Id, 0);
            playerVO.Items.Add(itemVO);
        }
        int dif = reward.Count;//itemVO.Count + reward.Count < 0 ? itemVO.Count : reward.Count;
        itemVO.Count += dif;
        if (itemVO.Count < 0)
            itemVO.Count = 0;

        OnItemChanged?.Invoke(itemVO, dif);
    }

    private void ApplySkillReward(RewardMeta reward)
    {
        SkillVO skillVO = GetSkillVOByID(reward.Id);
        if (skillVO == null)
        {
            skillVO = new SkillVO(reward.Id, 0);
            playerVO.Skills.Add(skillVO);
        }
        SkillMeta meta = Services.Data.SkillInfo(reward.Id);
        int current = playerVO.Slots[meta.Slot];
        playerVO.Slots[meta.Slot] = meta.Id;
        skillVO.Count = reward.Count;

        if (current > 0)
            GetSkillVOByID(current).Count = 0;

        OnItemChanged?.Invoke(skillVO, reward.Count);
    }

    private CardMeta ActivateNextCard(CardMeta cardMeta)
    {
        playerVO.Layers.Add(cardMeta.Id);
#if UNITY_EDITOR
        Debug.Log("Add CARD:" + playerVO.Layers.Select(q => Services.Data.GetCardMetaByID(q).Name).ToArray().ToJson());
#endif
        return cardMeta;
    }

    private void InitLeftRightChoice()
    {
        if (playerVO.Layers.Count == 0)
        {
            playerVO.Left = playerVO.Right = null;
            return;
        }

        int Id = playerVO.Layers.Last();

        CardMeta[] temp = Services.Data.CardDeckMeta
            .Where(cm =>
                cm.Event == false &&
                CheckTrigger(GameMeta.CARD, Id, Swipe.LEFT_CHOICE, cm.Act) &&
                CheckConditions(cm.Act.Con, cm, GetCardVOByID(cm.Id), DataService.EMPTY_REWARD))
            .OrderBy(cm => cm.Pri)
            .Reverse()
            .ToArray();

        CardMeta cardLeft = temp.Where(cm => cm.Pri == temp[0].Pri).PickRandomOrNull();

        temp = Services.Data.CardDeckMeta
            .Where(cm =>
                cm.Event == false &&
                cm != cardLeft &&
                CheckTrigger(GameMeta.CARD, Id, Swipe.RIGHT_CHOICE, cm.Act) &&
                CheckConditions(cm.Act.Con, cm, GetCardVOByID(cm.Id), DataService.EMPTY_REWARD))
            .OrderBy(cm => cm.Pri)
            .Reverse()
            .ToArray();

        CardMeta cardRight = temp.Where(cm => cm.Pri == temp[0].Pri).PickRandomOrNull();

        playerVO.Left = cardLeft != null ? cardLeft.Id : null;
        playerVO.Right = cardRight != null ? cardRight.Id : null;

        if (playerVO.Left == null && playerVO.Right != null)
            playerVO.Left = playerVO.Right;
        else if (playerVO.Right == null && playerVO.Left != null)
            playerVO.Right = playerVO.Left;
    }

    private void CreateChoiceView()
    {
        ChoiceData Left = SwipeData.Left;
        ChoiceData Right = SwipeData.Right;

        CardMeta PrevLayer = SwipeData.PrevLayer;

        Left.NextCard = playerVO.Left.HasValue ? Services.Data.GetCardMetaByID(playerVO.Left.Value) : null;
        Left.Icon = Left.NextCard != null ? Left.NextCard.Image : (PrevLayer != null ? PrevLayer.Image : "endturn");
        Left.Text = Left.NextCard != null ?
            (Left.NextCard.Act.Text.HasText() ? Left.NextCard.Act.Text : Left.NextCard.Name) :
            (PrevLayer != null ? PrevLayer.Name : "Конец дня");

        Left.Reward = Left.NextCard != null && Left.NextCard.Reward != null ? Left.NextCard.Reward.Select(r => r.Clone()).ToList() : new List<RewardMeta>();
        if (SwipeData.CurrentCardMeta.Left != null && SwipeData.CurrentCardMeta.Left.Reward != null)
            Left.Reward.Merge(SwipeData.CurrentCardMeta.Left.Reward);

        Right.NextCard = playerVO.Right.HasValue ? Services.Data.GetCardMetaByID(playerVO.Right.Value) : null;
        Right.Icon = Right.NextCard != null ? Right.NextCard.Image : (PrevLayer != null ? PrevLayer.Image : "endturn");
        Right.Text = Right.NextCard != null ?
            (Right.NextCard.Act.Text.HasText() ? Right.NextCard.Act.Text : Right.NextCard.Name) :
            (PrevLayer != null ? PrevLayer.Name : "Конец дня");

        Right.Reward = Right.NextCard != null && Right.NextCard.Reward != null ? Right.NextCard.Reward.Select(r => r.Clone()).ToList() : new List<RewardMeta>();
        if (SwipeData.CurrentCardMeta.Right != null && SwipeData.CurrentCardMeta.Right.Reward != null)
            Right.Reward.Merge(SwipeData.CurrentCardMeta.Right.Reward);

        List<ConditionMeta> conditions = new List<ConditionMeta>();
        foreach (CardMeta cm in Services.Data.CardDeckMeta.Where(c =>
            (CheckTrigger(GameMeta.CARD, SwipeData.CurrentCardMeta.Id, Swipe.LEFT_CHOICE, c.Act) ||
            CheckTrigger(GameMeta.CARD, SwipeData.CurrentCardMeta.Id, Swipe.RIGHT_CHOICE, c.Act)) &&
            !c.Act.Con.Exists(cc => !DoesPlayerKnow(cc.Id, cc.Tp))))
        {
            ConditionMeta skill = cm.Act.Con.Find(c => c.Tp == GameMeta.SKILL);
            if (skill != null)
            {
                if (conditions.Exists(c => c.Tp == GameMeta.SKILL && c.Id == skill.Id))
                    continue;

                if (GetSkillVOByID(skill.Id).Count == 0)
                {
                    conditions.Add(skill);
                    continue;
                }
            }
            foreach (ConditionMeta con in cm.Act.Con)
            {
                if (conditions.Exists(c => con.Tp == c.Tp && c.Id == cm.Id))
                    continue;

                if (CheckConditions(new List<ConditionMeta>() { con }, null, null, null))
                    continue;

                conditions.Add(con);
            }
        }

        SwipeData.Conditions = conditions;
    }

}
