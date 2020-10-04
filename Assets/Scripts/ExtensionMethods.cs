using System;
using UnityEngine;

public static class ExtensionMethods {
    public static T[] SubArray<T>(this T[] data, int startIndex, int endIndexExclusive = 0) {
        if (endIndexExclusive <= 0) {
            endIndexExclusive = endIndexExclusive + data.Length;
        }
        var length = endIndexExclusive - startIndex;
        T[] result = new T[length];
        Array.Copy(data, startIndex, result, 0, length);
        return result;
    }

    public static Vector3 ToVector3(this (int x, int y) coord) {
        return new Vector3(coord.x, coord.y);
    }
}
