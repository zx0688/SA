using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Core;
using Cysharp.Threading.Tasks;
using GameServer;
using haxe.lang;
using haxe.root;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerService : IService
{
    //public event Action<List<RewardMeta>> OnGetReward;
    //public event Action<int, int> OnItemChanged;

    public event Action<CardMeta> OnChangedLocation;

    public event Action OnFollowQuestChanged;

    public event Action<CardMeta, CardData> OnCardExecuted;

    public event Action OnProfileUpdated;
    public event Action OnQuestStart;
    public event Action OnAccelerated;
    public event Action OnInited;
    public event Action OnUpdated;
    public event Action OnDestroed;

    public ProfileData Profile;
    private GameMeta Meta => Services.Meta.Game;
    private GameRequest request = null;

    public string FollowQuest
    {
        get => PlayerPrefs.HasKey("AQ") ? PlayerPrefs.GetString("AQ") : null;
        set
        {
            PlayerPrefs.SetString("AQ", value);
            OnFollowQuestChanged?.Invoke();
        }
    }


    public async UniTask Init(IProgress<float> progress = null)
    {
        request = new GameRequest(Type: 0, Value: 0, Id: "");

        Profile = await HttpBatchServer.GetProfile(progress: progress);

        PlayerPrefs.DeleteAll();

        if (Profile.ActiveQuests.Count == 0)
            FollowQuest = null;

        OnProfileUpdated?.Invoke();
    }


    public bool IsRewardApplicable(List<RewardMeta> reward)
    {
        foreach (RewardMeta r in reward)
        {
            //if (r.count < 0 && itemHandler.AvailableItem(r.id) < Math.Abs(r.count))
            return false;
        }
        return true;
    }

    /*public void Trigger(List<CardData> queue, TriggerVO trigger, List<RewardData> reward, int time)
    {

        //ExecuteTrigger(queue, trigger, reward, time);

        //List<RewardData> actions = reward.Where (r => r.tp == DataManager.ACTION_ID).ToList ();
        List<RewardData> inventory = reward.Where(r => r.Tp == DataService.ITEM_ID).ToList();
        List<RewardData> buildings = reward.Where(r => r.Tp == DataService.BUILDING_ID).ToList();
        // if (inventory.Count > 0)
        //    ExecuteTrigger(queue, new TriggerVO(TriggerData.ITEM, 0, TriggerData.CHOICE_GET, null, null, null, inventory), reward, time);
        //if (buildings.Count > 0)
        //   ExecuteTrigger(queue, new TriggerVO(TriggerData.BUILD, 0, TriggerData.CHOICE_GET, null, null, null, buildings), reward, time);
        //if (actions.Count > 0)
        ///    ExecuteTrigger (queue, new TriggerVO (TriggerData.ACTION, 0, TriggerData.CHOICE_GET, null, null, null, actions), reward, time);
        /*List<ItemVO> items = new List<ItemVO> ();
        foreach (RewardData r in reward) {
            if (r.tp == DataManager.ITEM_ID || r.tp == DataManager.BUILDING_ID)
                items.Add (new ItemVO (r.id, r.count));
        }

        if (reward.Count > 0)
            OnItemReceived?.Invoke(reward);

        if (trigger.tp == TriggerData.CARD && trigger.id > 0)
        {
            ActionData choiceData = trigger.choice == Swipe.LEFT_CHOICE ? trigger.swiped.Left.action : trigger.swiped.Right.action;
            OnCardExecuted?.Invoke(trigger.swiped.CardData, choiceData);
        }

        OnProfileUpdated?.Invoke();

        Services.Assets.SetProfile(JsonUtility.ToJson(playerVO)).Forget();
        /*if (this == Services.Player) {
            Services.network.AddRequestToPool (new RequestVO ("choise"));
        }
    }*/
    public void Buy(int timestamp)
    {
        //List<RewardMeta> priceData = Services.Data.GameMeta.Config.PriceReroll;
        List<RewardMeta> items = new List<RewardMeta>();
        //ItemVO i = (ItemVO) itemHandler.Add(Services.Data.ItemInfo(ItemData.ACCELERATE_ID), priceData[0].id, timestamp);

        //r.Id = ItemMeta.ACCELERATE_ID;
        //r.Tp = DataService.ITEM_ID;
        //r.Count = priceData[0].Id;


        OnProfileUpdated?.Invoke();

    }


    public void StartGame()
    {
        HttpBatchServer.Change(new GameRequest(TriggerMeta.START_GAME));
        OnProfileUpdated?.Invoke();
    }

    public void Swipe(SwipeData swipe)
    {
        request.Hash = swipe.Card.Id;
        request.Type = TriggerMeta.SWIPE;
        request.Value = swipe.Choice;
        request.Id = swipe.Left != null ? (swipe.Choice == CardMeta.LEFT ? swipe.Left.Id : swipe.Right.Id) : null;

        HttpBatchServer.Change(request);

        if (Profile.ActiveQuests.Count == 0)
            FollowQuest = null;
        else if (Profile.QuestEvent != null)
        {
            if (FollowQuest == null || !Profile.ActiveQuests.Contains(FollowQuest))
                FollowQuest = Profile.ActiveQuests[0];

            if (Profile.ActiveQuests.Contains(Profile.QuestEvent))
                OnQuestStart?.Invoke();
        }

        OnCardExecuted?.Invoke(swipe.Card, swipe.Data);
        OnProfileUpdated?.Invoke();
    }

    public void ChangeLocation(CardMeta location)
    {
        request.Id = Profile.CurrentLocation;
        request.Type = TriggerMeta.CHANGE_LOCATION;
        request.Hash = location.Id;

        HttpBatchServer.Change(request);

        OnChangedLocation?.Invoke(location);
        OnProfileUpdated?.Invoke();
    }


    public void Accelerate()
    {
        if (Profile.Deck.Count > 0)
        {
            Debug.LogError("can't accelerate if cards are available");
            return;
        }
        int duration = Meta.Config.DurationReroll;
        int timeLeft = GameTime.Left(GameTime.Get(), Profile.Cooldown, duration);
        if (timeLeft > 0)
        {
            int price = SL.GetPriceReroll(GameTime.Left(GameTime.Get(), Profile.Cooldown, duration), Meta);
            if (!Profile.Items.TryGetValue(Meta.Config.PriceReroll[0].Id, out ItemData i) || i.Count < price)
            {
                Debug.LogError("can't accelerate if not enough price");
                return;
            }
        }

        request.Type = TriggerMeta.REROLL;
        HttpBatchServer.Change(request);

        OnProfileUpdated?.Invoke();
        OnAccelerated?.Invoke();
    }


    public void CreateSwipeData(SwipeData swipeData)
    {
        swipeData.Choice = -1;

        //default card
        string nextCardId = Profile.Deck[Profile.Deck.Count - 1];
        swipeData.Data = Profile.Cards.GetValueOrDefault(nextCardId);
        swipeData.Card = Meta.Cards.GetValueOrDefault(nextCardId);
        swipeData.Left = Profile.Left != null ? Meta.Cards[Profile.Left] : null;
        swipeData.Right = Profile.Right != null ? Meta.Cards[Profile.Right] : swipeData.Left;
        swipeData.LastCard = swipeData.Left == null && swipeData.Right == null && Profile.Deck.Count <= 1;
        swipeData.Hero = swipeData.Card.Hero != null ? Meta.Heroes[swipeData.Card.Hero] : null;

        swipeData.Conditions = new List<ConditionMeta>();
        foreach (TriggerMeta t in swipeData.Card.Next.OrEmptyIfNull())
            if (Services.Meta.Game.Cards.TryGetValue(t.Id, out CardMeta c) && c.Con != null && c.Con.Length > 0)
                swipeData.Conditions.Merge(c.Con.ToList());
        swipeData.Conditions = swipeData.Conditions.Where(c => !Profile.Items.TryGetValue(c.Id, out ItemData value) || value.Count < c.Count).ToList();

        //we can't take away an item without choice
        /*foreach (TriggerMeta t in swipeData.Card.Over.OrEmptyIfNull())
            if (Services.Meta.Game.Cards.TryGetValue(t.Id, out CardMeta c) && c.Con != null && c.Con.Length > 0)
                swipeData.Conditions.Merge(c.Con.ToList());
*/

        swipeData.FollowPrompt = -1;
        if (FollowQuest != null
            && swipeData.Left != null
            && swipeData.Left.Id != swipeData.Right.Id
            && Services.Meta.Game.Cards.TryGetValue(FollowQuest, out CardMeta quest))
        {
            List<CardMeta> cards = findAllNextPossibleCards(swipeData.Left);
            bool left = quest.Next.ToList().Exists(t => swipeData.Left.Id == t.Id || findCardIdDeepRecursive(cards, t.Id, 4));

            if (left == false) cards = findAllNextPossibleCards(swipeData.Right);
            bool right = !left && quest.Next.ToList().Exists(t => swipeData.Right.Id == t.Id || findCardIdDeepRecursive(cards, t.Id, 4));
            swipeData.FollowPrompt = left ? CardMeta.LEFT : (right ? CardMeta.RIGHT : -1);
        }
    }

    private List<CardMeta> findAllNextPossibleCards(CardMeta start)
    {
        List<TriggerMeta> possibleNextTrigger = new List<TriggerMeta>();
        if (start.Next != null)
            possibleNextTrigger = possibleNextTrigger.Concat(start.Next.ToList()).ToList();

        if (start.Over != null)
            possibleNextTrigger = possibleNextTrigger.Concat(start.Over.ToList()).ToList();

        List<CardMeta> cards = possibleNextTrigger.Select(t =>
            {
                if (Services.Meta.Game.Cards.TryGetValue(t.Id, out CardMeta cardMeta))
                    return cardMeta;
                else
                    return null;

            }).Where(c => c != null && (c.Type == CardMeta.TYPE_CARD || c.Type == CardMeta.TYPE_SKILL)).ToList();
        return cards;
    }

    private bool findCardIdDeepRecursive(List<CardMeta> cards, string lookingForId, int deep)
    {
        foreach (CardMeta c in cards)
        {
            if (c.Id == lookingForId)
                return true;
            if (deep > 0)
            {
                List<CardMeta> nextCards = findAllNextPossibleCards(c);
                if (nextCards.Count > 0 && findCardIdDeepRecursive(nextCards, lookingForId, deep - 1))
                    return true;
            }

        }
        return false;
    }
}
