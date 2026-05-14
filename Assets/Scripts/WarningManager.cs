using UnityEngine;
using UnityEngine.Rendering;

public static class WarningManager
{
    private static int totalWarnings = 0;

    public static int GetTotalWarnings()
    {
        return totalWarnings;
    }

    public static void IncreaseTotalWarnings()
    {
        totalWarnings++;
    }

    public static void ResetTTotalWarnings()
    {
        totalWarnings = 0;
    }
}
