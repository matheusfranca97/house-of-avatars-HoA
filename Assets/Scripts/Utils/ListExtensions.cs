using System;
using System.Collections.Generic;

public static class ListExtensions
{
    public static T GetRandomItem<T>(this List<T> list)
    {
        int randomIndex = UnityEngine.Random.Range(0, list.Count);
        return list[randomIndex];
    }

    public static bool TryFind<T>(this List<T> list, Predicate<T> predicate, out T item)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (!predicate(list[i]))
                continue;

            item = list[i];
            return true;
        }

        item = default;
        return false;
    }

    public static T GetRandomItem<T>(this List<T> list, Random random)
    {
        int randomIndex = random.Next(list.Count);
        return list[randomIndex];
    }
}
