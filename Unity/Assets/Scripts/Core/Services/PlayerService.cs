using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Core;
using Cysharp.Threading.Tasks;
using GameServer;
using haxe.lang;
using haxe.root;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

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

    public ItemData GetItemData(string Id) => Profile.Items[Id];

    public DeckItem DeckItem => Profile.Deck.Count > 0 ? Profile.Deck.Last() : null;

    public List<ItemData> RewardCollected => Profile.RewardEvents.Values.ToList().Where(r => !Services.Meta.Game.Items[r.Id].Hidden).ToList();

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

        //if (Profile.ActiveQuests.Count == 0)
        //    FollowQuest = null;

        OnProfileUpdated?.Invoke();
    }

    public bool IsTutorAvailable(string key) => !Meta.Config.DisableTutorial
        && (!Profile.Tutorial.TryGetValue(key, out bool swipeTutorial) || !swipeTutorial);

    public void FinishTutor(string key)
    {
        request = new GameRequest(Type: TriggerMeta.TUTORIAL, Value: 0, Id: key);
        HttpBatchServer.Change(request);
        OnProfileUpdated?.Invoke();
    }



    public bool TryGetCardDescription(CardMeta card, out string _text)
    {
        string text = null;
        DeckItem deckItem = SL.TryGetCurrentCard(Profile);
        if (deckItem.S == CardData.NOTHING)
            text = card.IfNothing[(card.IfNothing.Length - 1) - deckItem.DI];
        else if (deckItem.S == CardData.DESCRIPTION)
        {
            if (card.OnlyOnce != null && (!Profile.Cards.TryGetValue(card.Id, out CardData _card) || _card.CT == 0))
                text = card.OnlyOnce[(card.OnlyOnce.Length - 1) - deckItem.DI];
            else if (card.RStory)
                text = card.Descs[Random.Range(0, card.Descs.Length)];
            else
                text = card.Descs[(card.Descs.Length - 1) - deckItem.DI];
        }
        else if (deckItem.S == CardData.REWARD)
        {
            if (Services.Player.RewardCollected.Count > 0)
            {
                if (RewardCollected.Exists(r => r.Count > 0) || !card.IfNothing.HasTexts())
                    text = card.RewardText[0];
                else
                    text = card.IfNothing[0];
            }
            else if (card.Reward == null && card.Cost == null && card.Over != null)
                text = card.RewardText[0];
            else if (card.IfNothing != null && card.IfNothing.Length > 0)
                text = card.IfNothing[0];
            else if (deckItem.DI > 0 && card.RewardText != null)
                text = card.RewardText[deckItem.DI];
            else
                throw new Exception($"card {card.Id} must have no reward message");
        }
        else if (deckItem.Ch.Count > 0 && card.Descs != null)
        {
            if (card.OnlyOnce != null && (!Profile.Cards.TryGetValue(card.Id, out CardData _card) || _card.CT == 0))
                text = card.OnlyOnce[(card.OnlyOnce.Length - 1)];
            else if (card.RStory)
                text = card.Descs[Random.Range(0, card.Descs.Length)];
            else
                text = card.Descs[(card.Descs.Length - 1)];
        }
        _text = text;
        return text != null;
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

    public void SelfHeroChoose(string Id)
    {
        request = new GameRequest(Type: TriggerMeta.CHOOSE_SELF_HERO, Value: 0, Id: Id);
        HttpBatchServer.Change(request);
        OnProfileUpdated?.Invoke();
    }

    public void Swipe(SwipeData swipe)
    {
        request.Hash = swipe.Card.Id;
        request.Type = TriggerMeta.SWIPE;

        if (swipe.Item.Ch.Count > 1)
        {
            request.Value = swipe.CurrentChoice;
            request.Id = swipe.CurrentChoice == CardMeta.LEFT ? swipe.Choices[0].Id : swipe.Choices[1].Id;
        }
        else if (swipe.Item.Ch.Count == 1)
        {
            request.Value = 0;
            request.Id = swipe.Item.Ch[0].Id;
        }
        else
        {
            request.Value = 0;
            request.Id = null;
        }

        HttpBatchServer.Change(request);

        /*if (Profile.ActiveQuests.Count == 0)
            FollowQuest = null;
        else if (Profile.QuestEvent != null)
        {
            //if (FollowQuest == null || !Profile.ActiveQuests.Contains(FollowQuest))
            //    FollowQuest = Profile.ActiveQuests[0];

            if (Profile.ActiveQuests.Contains(Profile.QuestEvent))
                OnQuestStart?.Invoke();
        }*/

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
        swipeData.CurrentChoice = -1;



        //default card
        DeckItem ditem = Profile.Deck[Profile.Deck.Count - 1];
        string nextCardId = ditem.Id;
        swipeData.Item = ditem;
        swipeData.Data = Profile.Cards.GetValueOrDefault(nextCardId);
        swipeData.Card = Meta.Cards.GetValueOrDefault(nextCardId);
        swipeData.Choices = ditem.Ch.Count > 0 ? ditem.Ch.Select(c => Meta.Cards[c.Id]).ToList() : new List<CardMeta>(); //Profile.Left != null ? Meta.Cards[Profile.Left.Id] : null;
        //swipeData.Down = null; //Profile.Right != null ? Meta.Cards[Profile.Right.Id] : swipeData.Up;


        //swipeData.LastCard = swipeData.Up == null && swipeData.Down == null && Profile.Deck.Count <= 1;
        swipeData.Hero = swipeData.Card.Hero != null ? Meta.Heroes[swipeData.Card.Hero] : null;

        swipeData.Conditions = new List<ItemTypeData>();

        if (swipeData.Card.Next.HasTriggers())
            RecursiveFindAllCardTriggers(swipeData.Card.Next, swipeData);

        //if (Profile.Deck.Count > 1 && Meta.Cards.TryGetValue(Profile.Deck[Profile.Deck.Count - 2], out CardMeta nextCardMeta) && nextCardMeta.Con.HasCondition())
        //    nextCardMeta.Con.ToList().ForEach(cc => swipeData.Conditions.Merge(cc.ToList()));

        //swipeData.Conditions = swipeData.Conditions.FindAll(c => c.Type == ConditionMeta.ITEM && Meta.Items.TryGetValue(c.Id, out ItemMeta item) && !item.Hidden);
        //if(swipeData.Card.)
        //swipeData.Card.Reward[0]

        //swipeData.Conditions = swipeData.Conditions.Where(c => !Profile.Items.TryGetValue(c.Id, out ItemData value) || value.Count < c.Count).ToList();

        //we can't take away an item without choice
        /*foreach (TriggerMeta t in swipeData.Card.Over.OrEmptyIfNull())
            if (Services.Meta.Game.Cards.TryGetValue(t.Id, out CardMeta c) && c.Con != null && c.Con.Length > 0)
                swipeData.Conditions.Merge(c.Con.ToList());
*/

        swipeData.FollowPrompt = -1;
        //         if (FollowQuest != null
        //             && swipeData.Up != null
        //             && swipeData.Up.Id != swipeData.Down.Id
        //             && Services.Meta.Game.Cards.TryGetValue(FollowQuest, out CardMeta quest))
        //         {
        //             List<CardMeta> cards = findAllNextPossibleCards(swipeData.Up);
        //             bool left = quest.Next.ToList().Exists(t => swipeData.Up.Id == t.Id || findCardIdDeepRecursive(cards, t.Id, 4));
        // 
        //             if (left == false) cards = findAllNextPossibleCards(swipeData.Down);
        //             bool right = !left && quest.Next.ToList().Exists(t => swipeData.Down.Id == t.Id || findCardIdDeepRecursive(cards, t.Id, 4));
        //             swipeData.FollowPrompt = left ? CardMeta.LEFT : (right ? CardMeta.RIGHT : -1);
        //         }


    }

    private void RecursiveFindAllGroupCardTriggers(TriggerMeta trigger, List<TriggerMeta> ts)
    {
        if (trigger.Type == CardMeta.TYPE_GROUP)
        {
            Meta.Groups.TryGetValue(trigger.Id, out GroupMeta groupMeta);
            if (groupMeta == null)
                Debug.LogError("group trigger error " + trigger.Id);
            foreach (TriggerMeta tg in groupMeta.Cards)
                RecursiveFindAllGroupCardTriggers(tg, ts);
        }
        else
        {
            ts.Add(trigger);
        }
    }

    private void RecursiveFindAllCardTriggers(TriggerMeta[] next, SwipeData data, bool stop = false)
    {
        foreach (TriggerMeta trigger in next)
        {
            List<TriggerMeta> ts = new List<TriggerMeta>();
            RecursiveFindAllGroupCardTriggers(trigger, ts);
            foreach (TriggerMeta t in ts)
            {
                Meta.Cards.TryGetValue(t.Id, out CardMeta c);
                if (c == null)
                    throw new Exception("trigger Id " + t.Id);

                //if (c.Descs == null && c.Over == null && c.Under == null && c.Next.HasTriggers() && !stop)
                //    RecursiveFindAllCardTriggers(c.Next, data, true);
                if (c.Cost != null)
                {
                    foreach (RewardMeta[] cc in c.Cost)
                        data.Conditions.Merge(cc.Select(c => new ItemTypeData(c.Id, c.Count, c.Type)).ToList());
                }
                if (c.Con.HasCondition())
                {
                    foreach (ConditionMeta[] cc in c.Con)
                        data.Conditions.Merge(cc.Select(c => new ItemTypeData(c.Id, c.Count, c.Type)).ToList());
                }

                //if (c.Over.HasTriggers() && c.Over.TryGet(c.Over.Length - 1, out TriggerMeta o))
                //     if (Services.Meta.Game.Cards.TryGetValue(o.Id, out CardMeta co) && co.Con.HasCondition())
                //        co.Con.ToList().ForEach(cc => data.Conditions.Merge(cc.ToList()));
            }
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
