using System.Collections;
using System;
using System.Collections.Generic;
using Meta;
using UnityEngine;
using System.Linq;
using GameServer;

[Serializable]
public class PlayerVO : GameServerData
{
    public int SwipeCount;
    public TutorVO tutorVO;
    public List<int> Layers;
    public List<CardVO> Cards = new List<CardVO>();
    public List<QuestVO> Quests = new List<QuestVO>();
    public List<ItemVO> Items;

    public List<SkillVO> Skills;
    public int[] Slots;

    public List<BuildingVO> Buildings;
    public string[] Tags;
    public int Timestamp;
    public bool First;

    public int SwipeReroll;


    public int TimestampStartReroll;

    public int Location;

    public int? Left;
    public int? Right;

    public Dictionary<int, Dictionary<int, List<BackBuildVO>>> BackBuild;

    public PlayerVO()
    {
        Layers = new List<int>();
        SwipeCount = 0;
        SwipeReroll = 0;

        string json = SecurePlayerPrefs.GetString("buildBack") as string;
        BackBuild = JsonUtility.FromJson<Dictionary<int, Dictionary<int, List<BackBuildVO>>>>(json);

        Slots = new[] { 0, 0, 0, 0 };
        Location = 27912732;
        //SecurePlayerPrefs.SetString("buildBack", JsonUtility.ToJson(Services.Player.GetPlayerVO.BackBuild));
    }
}

[Serializable]
public class BackBuildVO
{
    public bool Mirrored;
    public int SpriteNumber;
    public bool Off;
}

public class TutorVO
{

    public TutorVO()
    {

    }
    public bool swipeCard = false;
}

public class QuestVO : CardVO
{
    public const int STATE_ACTIVE = 0;
    public const int STATE_COMPLETE = 1;
    public const int STATE_EXPIRED = 2;
    public const int STATE_FAILED = 3;

    public QuestVO(int id, int count) : base(id, count)
    {
        Id = Activated = Executed = Choice = 0;
        //rew1 = rew2 = false;
        //state = 0;
    }
    //public int state;
    //public bool rew1;
    //public bool rew2;
}

public class CardVO : ItemVO
{
    public CardVO(int id, int count) : base(id, count)
    {
        Id = id;
        Count = 0;
        Activated = Executed = Choice = 0;
    }

    public int Activated;
    public int Executed;
    public int Choice;
    public int RewardIndex;
}

[Serializable]
public class BuildingVO
{
    //public int Id;
    public int Id => _meta.Id;
    //[field: NonSerialized]
    public BuildingMeta Meta => _meta;

    private BuildingMeta _meta;

    public BuildingVO(BuildingMeta build, int count)
    {
        this._meta = build;
        this.Count = count;
    }

    public int Count;
    public int Executed;
    public int Stact;
    public int Pos;
}

public class SkillVO : ItemVO
{
    public SkillVO(int Id, int Count) : base(Id, Count) { }
}

[Serializable]
public class ItemVO
{
    public int Id;
    public int Count;

    public RewardMeta CreateReward()
    {
        RewardMeta r = new RewardMeta();
        r.Count = this.Count;
        r.Tp = GameMeta.ITEM;
        r.Count = this.Count;
        return r;
    }

    public ItemVO(int id, int count)
    {
        this.Id = id;
        this.Count = count;
    }
}