using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Maui.Views;

namespace InventoryManagement.Views;

public partial class MessagePage : Popup
{
    public enum MessageType
    {
        Success,
        Warning,
        Danger
    }

    public MessagePage()
    {
        InitializeComponent();
    }

    private void CloseClicked(object sender, EventArgs e)
    {
        Close();
    }

    public MessagePage ShowPopu(MessageType messageType, string message, string title = null)
    {
        this.message.Text = message;
        switch (messageType)
        {
            case MessageType.Success:
                this.title.Text = title ?? "成功";
                this.message.BackgroundColor = Color.FromRgb(133, 206, 97);
                break;
            case MessageType.Warning:
                this.title.Text = title ?? "警告";
                this.message.BackgroundColor = Color.FromRgb(235, 181, 99);
                break;
            case MessageType.Danger:
                this.title.Text = title ?? "错误";
                this.message.BackgroundColor = Color.FromRgb(247, 137, 137);
                break;
            default:
                break;
        }
        return this;
    }
    public async Task<MessagePage> ShowPopuAsync(MessageType messageType, string message, string title = null)
    {
        this.message.Text = message;
        switch (messageType)
        {
            case MessageType.Success:
                this.title.Text = title ?? "成功";
                this.message.TextColor = Color.FromRgb(133, 206, 97);
                break;
            case MessageType.Warning:
                this.title.Text = title ?? "警告";
                this.message.TextColor = Color.FromRgb(235, 181, 99);
                break;
            case MessageType.Danger:
                this.title.Text = title ?? "错误";
                this.message.TextColor = Color.FromRgb(247, 137, 137);
                break;
            default:
                break;
        }
        return await Task.Run(() => this);
    }
    public MessagePage ShowPopuSuccess(string message, string title = null)
    {
        return ShowPopu(MessageType.Success, message, title);
    }
    public MessagePage ShowPopuWarning(string message, string title = null)
    {
        return ShowPopu(MessageType.Warning, message, title);
    }
    public MessagePage ShowPopuDanger(string message, string title = null)
    {
        return ShowPopu(MessageType.Danger, message, title);
    }
    public async Task<MessagePage> ShowPopuSuccessAsync(string message, string title = null)
    {
        return await ShowPopuAsync(MessageType.Success, message, title);
    }
    public async Task<MessagePage> ShowPopuWarningAsync(string message, string title = null)
    {
        return await ShowPopuAsync(MessageType.Warning, message, title);
    }
    public async Task<MessagePage> ShowPopuDangerAsync(string message, string title = null)
    {
        return await ShowPopuAsync(MessageType.Danger, message, title);
    }
}