using System;

public class PanelEvents
{
    public static Action OnPanelManagerInitialized;

    public static Action<string> OnPanelShown;

    public static Action OnPanelHidden;
}
