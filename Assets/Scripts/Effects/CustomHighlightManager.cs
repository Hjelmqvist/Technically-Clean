using HighlightPlus;

public static class CustomHighlightManager
{
    private static HighlightEffect selected;

    public static void SetSelected(HighlightEffect newSelected)
    {
        if (selected != null)
            selected.SetHighlighted(false);
        selected = newSelected;
        if (newSelected != null)
            selected.SetHighlighted(true);
    }
}