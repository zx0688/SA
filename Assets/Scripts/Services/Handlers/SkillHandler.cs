using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cards;
using Data;
using UnityEngine;

public class SkillHandler
{

    private PlayerVO player;
    private DataService dataManager;

    //delegate bool ActionTrigger(TriggerVO trigger, ActionData data, int startTime, int timestamp);
    //private ActionTrigger actionTrigger;
    public SkillHandler(PlayerVO playerVO)
    {
        this.player = playerVO;
        dataManager = Services.Data;
        //  actionTrigger = Services.Data.ActionTrigger;
    }

    /*public void ApplyCardSkill(SwipeData cardParams, TriggerVO trigger, int time)
    {
        int left_chance = 0;
        int right_chance = 0;

        List<RewardData> drop = new List<RewardData>();
        List<SkillVO> skills = new List<SkillVO>();
        bool has = false;
        for (int i = 0; i < player.skills.Count; i++)
        {
            SkillVO skillVO = player.skills[i];
            if (skillVO.count == 0)
                continue;
            SkillData skillData = Services.Data.SkillInfo(skillVO.id);

            if (actionTrigger(trigger, skillData.act, 0, time))
            {
                /*if (skillData.act.chance > 0) {
                       float random = UnityEngine.Random.Range (0, 100f);
                       if (random > skillData.act.chance)
                           continue;
                   }
                Services.Data.ApplyReward(drop, GetDrop(skillVO, skillData), 1f);
                has = true;
            }

            List<RewardData> chance = GetChance(skillVO, skillData);
            if (cardParams.Left.action != null)
            {
                RewardData l = chance.Find(r => cardParams.Left.action.Reward.Exists(_r => _r.Tp == r.Tp && _r.Id == r.Id));
                if (l != null)
                {
                    has = true;
                    left_chance += l.Count;
                }
            }
            if (cardParams.Right.action != null)
            {
                RewardData ri = chance.Find(r => cardParams.Right.action.Reward.Exists(_r => _r.Tp == r.Tp && _r.Id == r.Id));
                if (ri != null)
                {
                    has = true;
                    right_chance += ri.Count;
                }
            }

            if (has == true)
            {
                skills.Add(skillVO);
            }

        }

        cardParams.Drop = drop;
        //cardParams.skills = skills;

        if (left_chance > 0)
            cardParams.Left.Chance = cardParams.Left.action.Chance + left_chance;
        if (right_chance > 0)
            cardParams.Right.Chance = cardParams.Right.action.Chance + right_chance;

    }*/

    public SkillVO Add(SkillMeta data, int count, int time)
    {

        if (data.One == true)
        {
            SkillVO c = GetVO(0, data.Type);
            if (c != null)
                player.skills.Remove(c);
        }

        SkillVO current = player.skills.Find(s => s.Id == data.Id);
        if (current == null)
        {
            current = new SkillVO(data.Id, 1);
            //current.id = data.Id;
            current.Count = 0;
            player.skills.Add(current);
        }
        //current.time = data.Act.Time;
        //current.id = data.;
        current.Activated = time;
        current.Count = count;
        return current;
    }

    /*public void Trigger(List<CardData> queue, TriggerVO trigger, List<RewardData> reward, int time)
    {

        if (trigger.tp == TriggerData.CARD)
        {

            Services.Data.ApplyReward(reward, trigger.swiped.Drop, 1f);

            List<SkillVO> _remove = new List<SkillVO>();
            /*foreach (SkillVO s in trigger.swiped.skills)
            {
                if (s.time > 0)
                {
                    s.time--;
                    if (s.time == 0)
                        _remove.Add(s);
                }
            }

            foreach (SkillVO s in _remove)
                player.skills.Remove(s);
        }

    }*/

    public SkillVO GetVO(int id, int type)
    {
        for (int i = 0; i < player.skills.Count; i++)
        {
            SkillVO skillVO = player.skills[i];
            if (skillVO.Count == 0)
                continue;
            else if (id == 0 && type > 0)
            {
                SkillMeta sd = Services.Data.GetSkillMeta(skillVO.Id);
                if (sd.Type == type)
                    return skillVO;
            }
            else if (skillVO.Id == id)
                return skillVO;
        }
        return null;
    }

    public List<RewardData> GetIncrease(SkillVO skillVO, SkillMeta skillData)
    {
        /*if (skillVO == null)
            return new List<RewardData>();
        if (skillVO.Count == 1)
            return skillData.increase1;
        else if (skillVO.Count == 2)
            return skillData.increase2;
        else if (skillVO.Count == 3)
            return skillData.increase3;
        */
        return new List<RewardData>();
    }

    public List<RewardData> GetChance(SkillVO skillVO, SkillMeta skillData)
    {
        /*if (skillVO == null)
            return new List<RewardData>();
        if (skillVO.Count == 1)
            return skillData.chance1;
        else if (skillVO.Count == 2)
            return skillData.chance2;
        else if (skillVO.Count == 3)
            return skillData.chance3;*/
        return new List<RewardData>();
    }
    public List<RewardData> GetDrop(SkillVO skillVO, SkillMeta skillData)
    {
        /*if (skillVO == null)
            return new List<RewardData>();
        if (skillVO.Count == 1)
            return skillData.drop1;
        else if (skillVO.Count == 2)
            return skillData.drop2;
        else if (skillVO.Count == 3)
            return skillData.drop3;*/
        return new List<RewardData>();
    }

    public List<SkillVO> GetListVOByType(int type)
    {
        List<SkillVO> result = new List<SkillVO>();
        for (int i = 0; i < player.skills.Count; i++)
        {
            SkillVO skillVO = player.skills[i];
            if (skillVO.Count == 0)
                continue;

            SkillMeta sd = Services.Data.GetSkillMeta(skillVO.Id);
            if (sd.Type == type)
                result.Add(skillVO);
        }
        return result;
    }

    public SkillVO Change(SkillMeta data, int time)
    {
        throw new System.NotImplementedException();
    }

    public int AvailableItem(int i, int type = -1)
    {
        throw new NotImplementedException();
    }
}