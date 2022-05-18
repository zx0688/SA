using System;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class PlayerVO
{
    public int SwipeCount;

    public TutorVO tutorVO;
    public List<CardVO> cards;
    public List<QuestVO> quests;
    public List<ItemVO> items;
    public List<SkillVO> skills;
    public List<BuildingVO> buildings;
    public string[] tags;
    public int timestamp;
    public bool first;

    public int locationId;
    public List<int> locations;
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
        Id = Activated = Executed = Choice = 0;
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

    [field: NonSerialized]
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
    public SkillVO(int Id, int Count) : base(Id, Count)
    {

    }

    public int Activated;
}

[Serializable]
public class ItemVO
{
    public int Id;
    public int Count;

    public ItemVO(int id, int count)
    {
        this.Id = id;
        this.Count = count;
    }
}