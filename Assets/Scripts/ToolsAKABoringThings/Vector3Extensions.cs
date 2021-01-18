using UnityEngine;

public static class Vector3Extensions
{
    /// <summary>
    /// Shorthand to overrite just one of the values in a vector 3.
    /// Ex. of use: 
    /// Vector3 myVector = new Vector(0, 1, 0);
    /// print(myVector.With(x = 2));
    /// Will print: (2, 1, 0)
    /// </summary>
    /// <returns>New Vector with overwritten given value, the rest is the same as in the original</returns>
    public static Vector3 With(this Vector3 original, float? x = null, float? y = null, float? z = null)
    {
        return new Vector3(x ?? original.x, y ?? original.y, z ?? original.z);
    }

    public static Vector3 With(this Vector3 original, Vector2 newVector)
    {
        return new Vector3(newVector.x, newVector.y, original.z);
    }

    public static Vector3 RandomVec(this Vector3 original, float rangeMin, float rangeMax)
    {
        return new Vector3(Random.Range(rangeMin, rangeMax), Random.Range(rangeMin, rangeMax), Random.Range(rangeMin, rangeMax));
    }

    public static Vector3 RandomXYWithinBounds(this Vector3 original, Rect bounds)
    {
        float x = Random.Range(bounds.xMin, bounds.xMax);
        float y = Random.Range(bounds.yMin, bounds.yMax);

        return new Vector3(x, y, original.z);
    }
}
