using System.Collections.Generic;

public static class Utils
{
    public static IEnumerable<int> NaturalNumbers()
    {
        int value = 0;
        for (;;)
        {
            value++;
            yield return value;
        }
    }
}