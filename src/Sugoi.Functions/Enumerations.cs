namespace Sugoi.Functions;

public class Enumerations
{
    public enum InteractionTypes
    {
        Ping = 1,
        ApplicationCommand = 2,
        MessageComponent = 3,
        ApplicationCommand_AutoComplete = 4,
        ModalSubmit = 5
    }

    public enum InteractionResponseTypes
    {
        Pong = 1,
        ChannelMessageWithSoruce = 4,
        DeferredChannelMessageWithSource = 5,
        DeferredUpdateMessage = 6,
        UpdateMessage = 7,
        ApplicationCommand_AutoCompleteResult = 8,
        Modal = 9,
    }
}
