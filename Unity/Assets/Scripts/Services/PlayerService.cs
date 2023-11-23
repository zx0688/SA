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

public partial class PlayerService : IService
{

    private static readonly string URL = "";

    public event Action<List<RewardMeta>> OnItemReceived;
    public event Action<ItemVO, int> OnItemChanged;

    public event Action<CardMeta> OnChangedLocation;

    public event Action<CardMeta, CardVO> OnCardExecuted;
    public event Action<CardMeta, CardVO> OnCardActivated;

    public event Action OnProfileUpdated;
    public event Action OnQuestComplete;
    public event Action OnAccelerated;
    public event Action OnInited;
    public event Action OnUpdated;
    public event Action OnDestroed;

    private PlayerVO playerVO;

    public int currentChoise;

    public PlayerVO GetPlayerVO => playerVO;

    public List<CardMeta> QueueMeta => playerVO.Layers.ConvertAll(i => Services.Data.GetCardMetaByID(i));
    public CardVO GetCardVOByID(int id) => playerVO.Cards.Find(c => c.Id == id);
    public CardVO GetCardVOByIDOrCreate(int id)
    {
        CardVO cardVO = playerVO.Cards.Find(c => c.Id == id);
        if (cardVO == null)
        {
            cardVO = new CardVO(id, 0);
            playerVO.Cards.Add(cardVO);
        }
        return cardVO;
    }

    public bool DoesPlayerKnow(int id, int Tp) => Tp switch
    {
        GameMeta.SKILL => playerVO.Skills.Find(i => i.Id == id) != null,
        GameMeta.ITEM => playerVO.Items.Find(i => i.Id == id) != null,
        _ => throw new NotImplementedException("Undefined type")
    };

    public SkillVO GetSkillVOByID(int id) => playerVO.Skills.Find(i => i.Id == id);
    public ItemVO GetItemVOByID(int id) => playerVO.Items.Find(i => i.Id == id);

    //public List<RewardMeta> GetItemAsRewardList(int id) => new List<RewardMeta>() { GetItemVOByID(id).CreateReward() };

    public int GetCountItemByID(int id)
    {
        ItemVO i = GetItemVOByID(id);
        return i != null ? i.Count : 0;
    }

    //public int SwipeCountLeft(int activate, int time) => (time + activate) - _playerVO.SwipeCount;

    public async UniTask Init(IProgress<float> progress = null)
    {
        SwipeData = new SwipeData();

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

        if (playerVO == null)
        {
            playerVO = new PlayerVO();
            playerVO.Items = Services.Data.GameMeta.Profile.Reward.ConvertAll(r => new ItemVO(r.Id, r.Count)).ToList();
            playerVO.First = true;
        }


        //SecurePlayerPrefs.SetString ("profile", json);


        for (int i = 0; i < Services.Data.GameMeta.Cards.Length; i++)
        {
            /*CardMeta cardMeta = Services.Data.GameMeta.Cards[i];
            if (cardMeta.Once == true)
            {
                CardVO cardVO = GetCardVOByID(cardMeta.Id);
                if (cardVO != null && (cardVO.Executed > 0))
                    Services.Data.GameMeta.Cards[i] = null;
            }*/
        }
        Utils.ClearArray<CardMeta>(ref Services.Data.GameMeta.Cards);

        //await Services.Assets.SetProfile(_playerVO);

        //OnProfileUpdated?.Invoke ();
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
        List<RewardMeta> priceData = Services.Data.GameMeta.Config.PriceReroll;
        List<RewardMeta> items = new List<RewardMeta>();
        //ItemVO i = (ItemVO) itemHandler.Add(Services.Data.ItemInfo(ItemData.ACCELERATE_ID), priceData[0].id, timestamp);
        RewardMeta r = new RewardMeta();
        r.Id = ItemMeta.ACCELERATE_ID;
        //r.Tp = DataService.ITEM_ID;
        r.Count = priceData[0].Id;
        items.Add(r);
        OnItemReceived?.Invoke(items);
        OnProfileUpdated?.Invoke();

    }
    public void Accelerate(int timestamp, int count)
    {

        //int timeAccelerate = Services.Data.GameMeta.Config.Accelerate * count;

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

}
