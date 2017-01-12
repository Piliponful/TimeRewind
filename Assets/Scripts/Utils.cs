using System.Collections.Generic;

public static class Utils
{
    public static IEnumerable<int> NaturalNumbers()
    {
        int value = 1;
        for (;;)
        {
            value++;
            yield return value;
        }
    }
}