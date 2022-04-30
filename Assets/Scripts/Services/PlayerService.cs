using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Assets.SimpleLocalization;
using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.Networking;



public class PlayerService : IService
{

    private static readonly string URL = "";

    public event Action<List<RewardData>> OnItemReceived;

    public event Action<LocationData> OnChangedLocation;
    public event Action<LocationData> OnOpenedLocation;

    public event Action<CardData, ActionData> OnCardExecuted;
    public event Action OnProfileUpdated;
    public event Action OnQuestComplete;
    public event Action OnAccelerated;
    public event Action OnInited;
    public event Action OnUpdated;
    public event Action OnDestroed;

    public PlayerVO playerVO;

    public int currentChoise;

    //public ItemVO currentAction;



    //private ActionData defaultAction;

    void Awake()
    {

    }

    void Start()
    {

        //if (SecurePlayerPrefs.HasKey ("profile")) {
        //     Recovery ();
        // }

        //Facade.meta.OnUpdate += OnMetaUpdate;
    }

    public void ChangeLocation(LocationData locationData)
    {
        //locationHandler.ChangeLocation(locationData);
        OnChangedLocation?.Invoke(locationData);
        OnProfileUpdated?.Invoke();
    }

    public void OpenLocation(LocationData _location)
    {
        //locationHandler.Add(_location.id, 1, _location, 0);
        OnOpenedLocation?.Invoke(_location);
        OnProfileUpdated?.Invoke();
    }

    public async UniTask Init(IProgress<float> progress = null)
    {
        RequestVO r = new RequestVO("profile");
        //.network.AddRequestToPool(r);

        await UniTask.DelayFrame(2);

        //if the answer is none 
        //if (!SecurePlayerPrefs.HasKey ("profile")) {
        //Services.Data.game.profiles.timestamp = GameTime.GetTime ();
        //string json = JsonUtility.ToJson (Services.Data.game.profiles);
        // Debug.Log(json);
        // PlayerData[] players = JsonUtility.FromJson<PlayerData[]> (json);

        // foreach (PlayerData p in Services.Data.game.profiles) {
        playerVO = await Services.Assets.GetProfile();
        playerVO = playerVO == null ? Services.Data.Game.Profile : playerVO;

        /*if (playerVO.quests == null)
            playerVO.quests = new List<QuestVO>();
        if (playerVO.locations.Count == 0)
        {
            playerVO.locations = new List<int>();
            playerVO.locations.Add(playerVO.locationId);
        }*/

        //CardItem d = new CardItem ();
        //d.id = 2;
        //d.activated = 0;
        // d.choice = 0;
        //p.activeQuests.Add (d);
        /*if (this == Services.Player && p.tags.Contains ("player")) {
            playerData = p;
            //break;
        } else if (this == Services.enemy && p.tags.Contains ("enemy")) {
            playerData = p;
            //break;
        }*/

        //}

        //SecurePlayerPrefs.SetString ("profile", json);
        /*

                for (int i = 0; i < App.Data.game.cards.Length; i++)
                {
                    CardData cardData = App.Data.game.cards[i];
                    if (cardData.once == true)
                    {
                        CardVO cardVO = cardHandler.GetVO(cardData.id, 0);
                        if (cardVO != null && (cardVO.activated > 0 || cardVO.executed > 0))
                            App.Data.game.cards[i] = null;
                    }
                }
                Utils.ClearArray<CardData>(ref App.Data.game.cards);
        */
        await Services.Assets.SetProfile(playerVO);

        //OnProfileUpdated?.Invoke ();
    }


    public bool IsRewardApplicable(List<RewardData> reward)
    {
        foreach (RewardData r in reward)
        {
            //if (r.count < 0 && itemHandler.AvailableItem(r.id) < Math.Abs(r.count))
            return false;
        }
        return true;
    }

    public CardVO OpenCard(SwipeData param, int time)
    {
        CardData cardData = param.CardData;
        if (true)
        {
            return null;//questHandler.Add(Services.Data.QuestInfo(cardData.id), 1, time);
        }
        /*else if (cardData.nosave == true)
        {
            CardVO cardVO = new CardVO();
            cardVO.activated = time;
            cardVO.id = cardData.Id;
            return cardVO;
        }*/
        else
        {
            CardVO cardVO = null;//cardHandler.Add(cardData, 1, time);

            //ActionData[] right = { cardData.right1, cardData.right2, cardData.right3 };
            //param.right.action = right[cardVO.right - 1];

            if (cardVO.left == 0)
            {
                param.Left.action = null;
            }
            else
            {
                //ActionData[] left = { cardData.left1, cardData.left2, cardData.left3 };
                //param.left.action = left[cardVO.left - 1];
            }



            return cardVO;
        }
    }

    public int AvailableItem(int id)
    {
        return 0;//itemHandler.AvailableItem(id);
        //throw new NotImplementedException ();
    }

    /*private void ExecuteTrigger(List<CardData> queue, TriggerVO trigger, List<RewardData> reward, int time)
    {

        List<RewardData> _reward = new List<RewardData>();
        itemHandler.Trigger(queue, trigger, _reward, time);
        buildingHandler.Trigger(queue, trigger, _reward, time);
        questHandler.Trigger(queue, trigger, _reward, time);
        cardHandler.Trigger(queue, trigger, _reward, time);
        skillHandler.Trigger(queue, trigger, _reward, time);

        for (int i = 0; i < _reward.Count; i++)
        {
            RewardData r = _reward[i];
            switch (r.tp)
            {
                case DataService.SKILL_ID:
                    skillHandler.Add(Services.Data.SkillInfo(r.id), r.count, time);
                    break;
                case DataService.CARD_ID:
                    if (Services.Data.CheckConditions(r.condi, time))
                        queue.Add(Services.Data.CardInfo(r.id));
                    break;
                case DataService.ITEM_ID:
                    itemHandler.Add(Services.Data.ItemInfo(r.id), r.count, time);
                    break;
                case DataService.BUILDING_ID:
                    buildingHandler.Add(Services.Data.BuildingInfo(r.id), r.count, time);
                    break;
            }
        }

        Services.Data.ApplyReward(reward, _reward, 1f);

    }*/
    public void Trigger(List<CardData> queue, TriggerVO trigger, List<RewardData> reward, int time)
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
        }*/

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
        }*/
    }
    public void Buy(int timestamp)
    {
        List<RewardData> priceData = Services.Data.Game.Config.Price;
        List<RewardData> items = new List<RewardData>();
        //ItemVO i = (ItemVO) itemHandler.Add(Services.Data.ItemInfo(ItemData.ACCELERATE_ID), priceData[0].id, timestamp);
        RewardData r = new RewardData();
        r.Id = ItemData.ACCELERATE_ID;
        r.Tp = DataService.ITEM_ID;
        r.Count = priceData[0].Id;
        items.Add(r);
        OnItemReceived?.Invoke(items);
        OnProfileUpdated?.Invoke();

    }
    public void Accelerate(int timestamp, int count)
    {

        int timeAccelerate = Services.Data.Game.Config.Accelerate * count;

        /*if (Deck.instance.waitingTimeLeft - timeAccelerate < 0)
            timeAccelerate = Deck.instance.waitingTimeLeft;

        Deck.instance.waitingTimeLeft -= timeAccelerate;

        foreach (CardVO cardVO in playerVO.cards)
            if (cardVO.executed > 0)
                cardVO.executed = cardVO.executed - timeAccelerate;
        foreach (BuildingVO buildingVO in playerVO.buildings)
            if (buildingVO.executed > 0)
                buildingVO.executed = buildingVO.executed - timeAccelerate;

        List<RewardData> items = new List<RewardData>();
        ItemData accelerateData = Services.Data.ItemInfo(ItemData.ACCELERATE_ID);
        //ItemVO i = (ItemVO)itemHandler.Add(accelerateData, -count, timestamp);
        RewardData r = new RewardData();
        //r.id = i.id;
        r.Tp = DataService.ITEM_ID;
        //r.count = i.count;
        items.Add(r);
*/
        // OnItemReceived?.Invoke(items);
        OnProfileUpdated?.Invoke();
        OnAccelerated?.Invoke();
    }

    public PlayerVO GetVO(int id, int type)
    {
        return playerVO;
    }

}
