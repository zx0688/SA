using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Core;
using Cysharp.Threading.Tasks;
using GameServer;
using haxe.lang;
using UnityEngine;
using UnityEngine.Networking;

public partial class PlayerService : IService
{
    //public event Action<List<RewardMeta>> OnGetReward;
    //public event Action<int, int> OnItemChanged;

    public event Action<CardMeta> OnChangedLocation;
    public event Action<List<RewardMeta>> OnGetReward;
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

    //public List<CardMeta> QueueMeta => playerVO.Layers.ConvertAll(i => Services.Data.GetCardMetaByID(i));
    // public bool DoesPlayerKnow(int id, int Tp) => Tp switch
    // {
    //     GameMeta.SKILL => playerVO.Skills.Find(i => i.Id == id) != null,
    //     ConditionMeta.ITEM => playerVO.Items.Find(i => i.Id == id) != null,
    //     _ => throw new NotImplementedException("Undefined type")
    // };

    //public SkillVO GetSkillVOByID(int id) => playerVO.Skills.Find(i => i.Id == id);
    //public ItemVO GetItemVOByID(int id) => playerVO.Items.Find(i => i.Id == id);

    //public List<RewardMeta> GetItemAsRewardList(int id) => new List<RewardMeta>() { GetItemVOByID(id).CreateReward() };

    // public int GetCountItemByID(int id)
    // {
    //     ItemVO i = GetItemVOByID(id);
    //     return i != null ? i.Count : 0;
    // }

    //public int SwipeCountLeft(int activate, int time) => (time + activate) - _playerVO.SwipeCount;

    public async UniTask Init(IProgress<float> progress = null)
    {
        request = new GameRequest(Type: 0, Value: 0, Id: "");

        Profile = await HttpBatchServer.GetProfile(progress: progress);

        Profile.Cards.Values.ToList().ForEach(c =>
        {
            CardMeta cardMeta = Meta.Cards[c.Id];
            if (cardMeta.CT > 0 && c.CT >= cardMeta.CT)
            {
                Meta.Cards.Remove(c.Id);
            }
        });

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
        RewardMeta r = new RewardMeta();
        //r.Id = ItemMeta.ACCELERATE_ID;
        //r.Tp = DataService.ITEM_ID;
        //r.Count = priceData[0].Id;
        items.Add(r);

        OnProfileUpdated?.Invoke();

    }


    public void StartGame()
    {
        if (Profile.RewardEvent.Count > 0)
            OnGetReward?.Invoke(Profile.RewardEvent);

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

        CardData data = null;
        if (swipe.Card.CT > 0 && Profile.Cards.TryGetValue(swipe.Card.Id, out data) && data.CT >= swipe.Card.CT)
        {
            Meta.Cards.Remove(swipe.Card.Id);
        }

        if (Profile.RewardEvent.Count > 0)
            OnGetReward?.Invoke(Profile.RewardEvent);

        if (Profile.QuestEvent != null)
        {
            if (Profile.ActiveQuests.Count == 0)
                FollowQuest = null;
            else if (FollowQuest == null || !Profile.ActiveQuests.Contains(FollowQuest))
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
        if (swipeData.Card.Next != null && swipeData.Card.Next.Length > 0)
            foreach (TriggerMeta t in swipeData.Card.Next)
                if (Services.Meta.Game.Cards.TryGetValue(t.Id, out CardMeta c) && c.Con != null && c.Con.Length > 0)
                    swipeData.Conditions.Merge(c.Con.ToList());
    }
}
