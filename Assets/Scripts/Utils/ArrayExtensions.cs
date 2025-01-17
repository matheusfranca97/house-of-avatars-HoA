using System;

public static class ArrayExtensions
{
    public static T[] RemoveAt<T>(this T[] array, int index)
    {
        T[] dest = new T[array.Length - 1];
        if (index > 0)
            Array.Copy(array, 0, dest, 0, index);

        if (index < array.Length - 1)
            Array.Copy(array, index + 1, dest, index, array.Length - index - 1);

        return dest;
    }

    public static T GetRandomItem<T>(this T[] array)
    {
        int randomIndex = UnityEngine.Random.Range(0, array.Length);
        return array[randomIndex];
    }

    public static bool TryFind<T>(this T[] array, Predicate<T> predicate, out T item)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (!predicate(array[i]))
                continue;

            item = array[i];
            return true;
        }

        item = default;
        return false;
    }

    public static T GetRandomItem<T>(this T[] array, Random random)
    {
        int randomIndex = random.Next(array.Length);
        return array[randomIndex];
    }
}
