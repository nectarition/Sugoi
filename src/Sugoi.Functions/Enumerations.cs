namespace Sugoi.Functions;

public class Enumerations
{
    public enum InteractionTypes
    {
        Ping = 1,
        ApplicationCommand = 2,
        MessageComponent = 3,
        ApplicationCommand_AutoComplete = 4,
        ModalSubmit = 5,
    }

    public enum ApplicationCommandTypes
    {
        ChatInput = 1,
        User = 2,
        Message = 3,
    }

    public enum ApplicationCommandOptionTypes
    {
        SubCommand = 1,
        SubCommandGroup = 2,
        String = 3,
        Integer = 4,
        Boolean = 5,
        User = 6,
        Channel = 7,
        Role = 8,
        Mentionable = 9,
        Number = 10,
        Attachment = 11,
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

    public enum DiscordChannelTypes
    {
        GuildText = 0,
        DM = 1,
        GuildVoice = 2,
        GroupDM = 3,
        GuildCategory = 4,
        GuildAnnouncement = 5,
        AnnouncementThread = 6,
        PublicThread = 10,
        PrivateThread = 11,
        GuildStageVoice = 13,
        GuildDirectory = 14,
        GuildForum = 15,
        GuildMedia = 16
    }
}
