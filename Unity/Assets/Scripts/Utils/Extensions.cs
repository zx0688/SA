using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Cysharp.Text;
using haxe.lang;
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

    //public static List<RewardMeta> MakeCopy(this List<RewardMeta> key) => Services.Assets.Localize(key);

    public static List<ConditionMeta> Merge(this List<ConditionMeta> conditions, List<ConditionMeta> other)
    {
        foreach (ConditionMeta m in other)
        {
            ConditionMeta c = conditions.Find(c => c.Id == m.Id);
            if (c == null)
                conditions.Add(m);
        }
        return conditions;
    }



    public static T Find<T>(this T[] array, Predicate<T> match) where T : class
    {
        foreach (T i in array)
            if (match.Invoke(i))
                return i;
        return null;
    }

    public static void LoadItemIcon(this Image icon, string id, Action callback = null)
    {
        Services.Assets.SetSpriteIntoImage(icon, ZString.Format("Items/{0}", id), true).Forget();
    }

    public static void LoadCardImage(this Image icon, string name, Action callback = null)
    {
        Services.Assets.SetSpriteIntoImage(icon, ZString.Format("Cards/{0}", name), true).Forget();
    }

    public static void LoadHeroImage(this Image icon, string name, Action callback = null)
    {
        Services.Assets.SetSpriteIntoImage(icon, ZString.Format("Heroes/{0}", name), true).Forget();
    }

    public static string ToJson<T>(this T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static bool HasText(this string text)
    {
        return text != null && text.Length > 0;
    }

    public static T PickRandomOrNull<T>(this IEnumerable<T> source) where T : class
    {
        if (source == null || source.Count() == 0)
            return null;
        return source.PickRandom(1).Single();
    }

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