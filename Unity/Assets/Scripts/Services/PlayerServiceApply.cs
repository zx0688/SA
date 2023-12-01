// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using System.Reflection;
// 
// using Core;
// using Cysharp.Threading.Tasks;
// using GameServer;
// using UnityEngine;
// using UnityEngine.Networking;

// public partial class PlayerService
// {



//     private static List<CardMeta> CANDIDATES = new List<CardMeta>();

//     public void CreateSwipeData()
//     {
//         if (playerVO.Layers.Count == 0)
//             throw new Exception("=== GAME: Have no card in queue");

//         CardMeta currentCardMeta = Services.Meta.GetCardMetaByID(playerVO.Layers.Last());
//         CardVO currentCardVO = GetCardVOByIDOrCreate(currentCardMeta.Id);

// #if UNITY_EDITOR
//         Debug.Log("CREATE CARD:" + playerVO.Layers.ToArray().ToJson());
// #endif
//         //set new card
//         SwipeData.Choice = -1;
//         SwipeData.Card = currentCardMeta;
//         SwipeData.CurrentCardVO = currentCardVO;
//         SwipeData.PrevLayer = playerVO.Layers.Count > 1 ?
//             Services.Meta.GetCardMetaByID(playerVO.Layers[playerVO.Layers.Count - 2]) : null;

//         CreateChoiceView();



//         OnCardActivated?.Invoke(currentCardMeta, currentCardVO);
//     }

//     public void ApplyChangeLocation(CardMeta location)
//     {
//         playerVO.Location = location.Id;

//         if (false)//!CheckConditions(location.Act.Con, location, null, DataService.EMPTY_REWARD))
//         {
//             throw new Exception("contition do not right for change location");
//         }

//         OnChangedLocation?.Invoke(location);
//         OnProfileUpdated?.Invoke();
//     }

//     public void ApplyReroll(int timestamp, int accelerateValue = 0)
//     {
//         playerVO.SwipeReroll = playerVO.SwipeCount;

//         // ActivateNextCard(Services.Data.CardDeckMeta
//         //     .Where(cm => CheckTrigger(TriggerMeta.REROLL, 0, 0, cm.Act) && CheckConditions(cm.Act.Con, cm, null, null))
//         //     .PickRandom());

//         InitLeftRightChoice();

//         if (accelerateValue > 0)
//         {
//             //RewardMeta r = Services.Data.GameMeta.Config.ItemReroll[0].Clone();
//             //r.Count = accelerateValue;

//             //wwwApplyReward(new List<RewardMeta>() { r });
//         }

//     }



//     private void ApplyReward(List<RewardMeta> reward)
//     {
//         for (int i = 0; i < reward.Count; i++)
//         {
//             RewardMeta r = reward[i];
//             switch (r.Tp)
//             {
//                 // case ConditionMeta.ITEM:
//                 //     //ApplyItemReward(r);
//                 //     break;
//                 // case GameMeta.SKILL:
//                 //     //ApplySkillReward(r);
//                 //     break;

//             }
//         }

//         OnItemReceived?.Invoke(reward);
//     }

//     // private void ApplyItemReward(RewardMeta reward)
//     // {
//     //     if (reward.Random.Length > 0)
//     //         reward.Id = reward.Random.PickRandom();

//     //     ItemVO itemVO = GetItemVOByID(reward.Id);
//     //     if (itemVO == null)
//     //     {
//     //         itemVO = new ItemVO(reward.Id, 0);
//     //         playerVO.Items.Add(itemVO);
//     //     }
//     //     int dif = reward.Count;//itemVO.Count + reward.Count < 0 ? itemVO.Count : reward.Count;
//     //     itemVO.Count += dif;
//     //     if (itemVO.Count < 0)
//     //         itemVO.Count = 0;

//     //     OnItemChanged?.Invoke(itemVO, dif);
//     // }

//     // private void ApplySkillReward(RewardMeta reward)
//     // {
//     //     SkillVO skillVO = GetSkillVOByID(reward.Id);
//     //     if (skillVO == null)
//     //     {
//     //         skillVO = new SkillVO(reward.Id, 0);
//     //         playerVO.Skills.Add(skillVO);
//     //     }
//     //     SkillMeta meta = Services.Meta.SkillInfo(reward.Id);
//     //     // int current = playerVO.Slots[meta.Slot];
//     //     // playerVO.Slots[meta.Slot] = meta.Id;
//     //     skillVO.Count = reward.Count;

//     //     // if (current > 0)
//     //     //     GetSkillVOByID(current).Count = 0;

//     //     OnItemChanged?.Invoke(skillVO, reward.Count);
//     // }

//     private void InitLeftRightChoice()
//     {
//         // if (playerVO.Layers.Count == 0)
//         // {
//         //     playerVO.Left = playerVO.Right = null;
//         //     return;
//         // }

//         // int Id = playerVO.Layers.Last();

//         // CardMeta[] temp = Services.Data.CardDeckMeta
//         //     .Where(cm =>
//         //         cm.Event == false &&
//         //         CheckTrigger(ConditionMeta.CARD, Id, Swipe.LEFT_CHOICE, cm.Act) &&
//         //         CheckConditions(cm.Act.Con, cm, GetCardVOByID(cm.Id), DataService.EMPTY_REWARD))
//         //     .OrderBy(cm => cm.Pri)
//         //     .Reverse()
//         //     .ToArray();

//         // CardMeta cardLeft = temp.Where(cm => cm.Pri == temp[0].Pri).PickRandomOrNull();

//         // temp = Services.Data.CardDeckMeta
//         //     .Where(cm =>
//         //         cm.Event == false &&
//         //         cm != cardLeft &&
//         //         CheckTrigger(ConditionMeta.CARD, Id, Swipe.RIGHT_CHOICE, cm.Act) &&
//         //         CheckConditions(cm.Act.Con, cm, GetCardVOByID(cm.Id), DataService.EMPTY_REWARD))
//         //     .OrderBy(cm => cm.Pri)
//         //     .Reverse()
//         //     .ToArray();

//         // CardMeta cardRight = temp.Where(cm => cm.Pri == temp[0].Pri).PickRandomOrNull();

//         // playerVO.Left = cardLeft.id;
//         // playerVO.Right = cardRight.id;

//         // if (playerVO.Left == null && playerVO.Right != null)
//         //     playerVO.Left = playerVO.Right;
//         // else if (playerVO.Right == null && playerVO.Left != null)
//         //     playerVO.Right = playerVO.Left;
//     }

//     private void CreateChoiceView()
//     {
//         ChoiceData Left = SwipeData.Left;
//         ChoiceData Right = SwipeData.Right;

//         CardMeta PrevLayer = SwipeData.PrevLayer;

//         // Left.NextCard = playerVO.Left.HasValue ? Services.Data.GetCardMetaByID(playerVO.Left.Value) : null;
//         // Left.Icon = Left.NextCard != null ? Left.NextCard.Image : (PrevLayer != null ? PrevLayer.Image : "endturn");
//         // Left.Text = Left.NextCard != null ?
//         //     (Left.NextCard.Act.Text.HasText() ? Left.NextCard.Act.Text : Left.NextCard.Name) :
//         //     (PrevLayer != null ? PrevLayer.Name : "Конец дня");

//         // Left.Reward = Left.NextCard != null && Left.NextCard.Reward != null ? Left.NextCard.Reward.Select(r => r.Clone()).ToList() : new List<RewardMeta>();
//         // if (SwipeData.CurrentCardMeta.Left != null && SwipeData.CurrentCardMeta.Left.Reward != null)
//         //     Left.Reward.Merge(SwipeData.CurrentCardMeta.Left.Reward);

//         // Right.NextCard = playerVO.Right.HasValue ? Services.Data.GetCardMetaByID(playerVO.Right.Value) : null;
//         // Right.Icon = Right.NextCard != null ? Right.NextCard.Image : (PrevLayer != null ? PrevLayer.Image : "endturn");
//         // Right.Text = Right.NextCard != null ?
//         //     (Right.NextCard.Act.Text.HasText() ? Right.NextCard.Act.Text : Right.NextCard.Name) :
//         //     (PrevLayer != null ? PrevLayer.Name : "Конец дня");

//         // Right.Reward = Right.NextCard != null && Right.NextCard.Reward != null ? Right.NextCard.Reward.Select(r => r.Clone()).ToList() : new List<RewardMeta>();
//         // if (SwipeData.CurrentCardMeta.Right != null && SwipeData.CurrentCardMeta.Right.Reward != null)
//         //     Right.Reward.Merge(SwipeData.CurrentCardMeta.Right.Reward);

//         // List<ConditionMeta> conditions = new List<ConditionMeta>();
//         // foreach (CardMeta cm in Services.Data.CardDeckMeta.Where(c =>
//         //     (CheckTrigger(ConditionMeta.CARD, SwipeData.CurrentCardMeta.Id, Swipe.LEFT_CHOICE, c.Act) ||
//         //     CheckTrigger(ConditionMeta.CARD, SwipeData.CurrentCardMeta.Id, Swipe.RIGHT_CHOICE, c.Act)) &&
//         //     !c.Act.Con.Exists(cc => !DoesPlayerKnow(cc.Id, cc.Tp))))
//         // {
//         //     ConditionMeta skill = cm.Act.Con.Find(c => c.Tp == GameMeta.SKILL);
//         //     if (skill != null)
//         //     {
//         //         if (conditions.Exists(c => c.Tp == GameMeta.SKILL && c.Id == skill.Id))
//         //             continue;

//         //         if (GetSkillVOByID(skill.Id).Count == 0)
//         //         {
//         //             conditions.Add(skill);
//         //             continue;
//         //         }
//         //     }
//         //     foreach (ConditionMeta con in cm.Act.Con)
//         //     {
//         //         if (conditions.Exists(c => con.Tp == c.Tp && c.Id == cm.Id))
//         //             continue;

//         //         if (CheckConditions(new List<ConditionMeta>() { con }, null, null, null))
//         //             continue;

//         //         conditions.Add(con);
//         //     }
//         // }

//         //SwipeData.Conditions = conditions;
//     }

// }
