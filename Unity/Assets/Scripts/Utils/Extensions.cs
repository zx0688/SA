using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Cysharp.Text;
using haxe.lang;
using haxe.root;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class Extensions
{
    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }

    public static string Localize(this string key, LocalizePartEnum part = LocalizePartEnum.GUI) => Services.Assets.Localize(key ?? "", part);
    public static void Localize(this Text textField, string key, LocalizePartEnum part = LocalizePartEnum.GUI) => textField.text = Services.Assets.Localize(key ?? "", part);

    public static RewardMeta[] GetOnlyConstantReward(this RewardMeta[] reward) => reward.Where(r => r.Chance == 0).ToArray();

    public static string ColorizeHH(this string text, string color) => $"<color=#{color}>{text}</color>";

    public static List<ItemTypeData> Merge(this List<ItemTypeData> conditions, List<ItemTypeData> other)
    {
        other.ForEach(m =>
        {
            ItemTypeData c = conditions.Find(c => c.Id == m.Id && c.Type == m.Type);
            if (c == null)
                conditions.Add(m);
        });
        return conditions;
    }

    public static bool Exists<T>(this T[] value) => value != null && value.Length > 0;

    public static bool Check(this ConditionMeta condition) => SL.CheckCondition(new ConditionMeta[] { condition }, Services.Meta.Game, Services.Player.Profile, null);
    public static bool Check(this ConditionMeta[] conditions) => SL.CheckCondition(conditions, Services.Meta.Game, Services.Player.Profile, null);

    public static T Find<T>(this T[] array, Predicate<T> match) where T : class
    {
        foreach (T i in array)
            if (match.Invoke(i))
                return i;
        return null;
    }

    public static void LoadItemIcon(this Image icon, string id, Action callback = null)
    {
        Services.Assets.SetSpriteIntoImage(icon, ZString.Format("Items/{0}", id), true, null, callback).Forget();
    }

    public static void LoadCardImage(this Image icon, string name, Action callback = null)
    {
        Services.Assets.SetSpriteIntoImage(icon, ZString.Format("Cards/{0}", name), true, null, callback).Forget();
    }

    public static void LoadSkillImage(this Image icon, string name, Action callback = null)
    {
        Services.Assets.SetSpriteIntoImage(icon, ZString.Format("Skills/{0}", name), true, null, callback).Forget();
    }

    public static void LoadHeroImage(this Image icon, string name, Action callback = null)
    {
        Services.Assets.SetSpriteIntoImage(icon, ZString.Format("Heroes/{0}", name), true, null, callback).Forget();
    }

    public static string ToJson<T>(this T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }
    public static bool TryGet<T>(this T[] value, int index, out T result) where T : class
    {
        result = null;
        if (value != null && index >= 0 && index < value.Length)
        {
            result = value[index];
            return true;
        }
        return false;
    }

    public static bool HasCondition(this ConditionMeta[][] cond) => cond != null && cond.Length > 0 && cond[0].Length > 0;
    public static bool HasTexts(this string[] texts) => texts != null && texts.Length > 0;
    public static bool HasTriggers(this TriggerMeta[] trigger) => trigger != null && trigger.Length > 0;

    public static bool HasText(this string text) => text != null && text.Length > 0;

    public static bool TryGetRandom<T>(this IEnumerable<T> source, out T value) where T : class
    {
        value = null;
        if (source == null || source.Count() == 0)
            return false;
        value = source.PickRandom(1).Single();
        return true;
    }


    public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T> source) where T : class
    => source ?? Enumerable.Empty<T>();


    // public static T PickRandomOrNull<T>(this IEnumerable<T> source) where T : class
    // {
    //     if (source == null || source.Count() == 0)
    //         return null;
    //     return source.PickRandom(1).Single();
    // }

    public static T PickRandom<T>(this IEnumerable<T> source)
    {
        return source.PickRandom(1).Single();
    }

    public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
    {
        return source.Shuffle().Take(count);
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        return source.OrderBy(x => Guid.NewGuid());
    }
}
