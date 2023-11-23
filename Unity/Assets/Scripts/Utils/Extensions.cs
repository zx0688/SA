using System;
using System.Collections.Generic;
using System.Linq;
using Assets.SimpleLocalization;
using Meta;
using UnityEngine;
using UnityEngine.UI;

public static class Extensions
{
    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }

    public static string Localize(this string key) => LocalizationManager.Localize(key);
    public static void Localize(this Text textField, string key) => textField.text = LocalizationManager.Localize(key);

    public static List<RewardMeta> Merge(this List<RewardMeta> reward, List<RewardMeta> concated)
    {
        for (int i = 0; i < concated.Count; i++)
        {
            RewardMeta c = concated[i];
            RewardMeta r = reward.Find(_r => _r.Id == c.Id && _r.Tp == c.Tp);
            if (r == null)
            {
                r = c.Clone();
                reward.Add(r);
            }
            else
            {
                // r.chance = r.chance > 0 ? (r.chance + c.chance) / 2 : 0;
                r.Count += c.Count; //Mathf.FloorToInt (c.count * multi);
            }
        }
        return reward;
    }

    public static void SetImage(this Image icon, string address)
    {
        Services.Assets.SetSpriteIntoImage(icon, address, true).Forget();
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

    public static string Translate(this string key)
    {
        return LocalizationManager.HasKey(key) ? LocalizationManager.Localize(key) : key;
    }

    public static string Translate(this string key, object arg1)
    {
        return LocalizationManager.HasKey(key) ? LocalizationManager.Localize(key, arg1) : key;
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
