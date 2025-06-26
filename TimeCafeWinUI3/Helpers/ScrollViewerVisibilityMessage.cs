using CommunityToolkit.Mvvm.Messaging.Messages;

namespace TimeCafeWinUI3.Helpers;

public enum ShellScrollViewerMode
{
    Default,
    Disabled
}

public class ShellScrollViewerVisibilityMessage : ValueChangedMessage<ShellScrollViewerMode>
{
    public ShellScrollViewerVisibilityMessage(ShellScrollViewerMode mode) : base(mode) { }
}