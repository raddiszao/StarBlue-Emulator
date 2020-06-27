
using log4net;
using StarBlue.HabboHotel.Rooms.Chat.Commands;
using StarBlue.HabboHotel.Rooms.Chat.Emotions;
using StarBlue.HabboHotel.Rooms.Chat.Filter;
using StarBlue.HabboHotel.Rooms.Chat.Logs;
using StarBlue.HabboHotel.Rooms.Chat.Pets.Commands;
using StarBlue.HabboHotel.Rooms.Chat.Pets.Locale;
using StarBlue.HabboHotel.Rooms.Chat.Styles;

namespace StarBlue.HabboHotel.Rooms.Chat
{
    public sealed class ChatManager
    {
        private static readonly ILog log = LogManager.GetLogger("StarBlue.HabboHotel.Rooms.Chat.ChatManager");

        /// <summary>
        /// Chat Emoticons.
        /// </summary>
        private ChatEmotionsManager _emotions;

        /// <summary>
        /// Chatlog Manager
        /// </summary>
        private ChatlogManager _logs;

        /// <summary>
        /// Filter Manager.
        /// </summary>
        private WordFilterManager _filter;

        /// <summary>
        /// Commands.
        /// </summary>
        private CommandManager _commands;

        /// <summary>
        /// Pet Commands.
        /// </summary>
        private PetCommandManager _petCommands;

        /// <summary>
        /// Pet Locale.
        /// </summary>
        private PetLocale _petLocale;

        /// <summary>
        /// Chat styles.
        /// </summary>
        private ChatStyleManager _chatStyles;

        /// <summary>
        /// Initializes a new instance of the ChatManager class.
        /// </summary>
        public ChatManager()
        {
            _emotions = new ChatEmotionsManager();
            _logs = new ChatlogManager();

            _filter = new WordFilterManager();
            _filter.InitWords();
            _filter.InitCharacters();

            _commands = new CommandManager(":");
            _petCommands = new PetCommandManager();
            _petLocale = new PetLocale();

            _chatStyles = new ChatStyleManager();
            _chatStyles.Init();

            log.Info(">> Chat Manager -> READY!");
        }

        public ChatEmotionsManager GetEmotions()
        {
            return _emotions;
        }

        public ChatlogManager GetLogs()
        {
            return _logs;
        }

        public WordFilterManager GetFilter()
        {
            return _filter;
        }

        public CommandManager GetCommands()
        {
            return _commands;
        }

        public PetCommandManager GetPetCommands()
        {
            return _petCommands;
        }

        public PetLocale GetPetLocale()
        {
            return _petLocale;
        }

        public ChatStyleManager GetChatStyles()
        {
            return _chatStyles;
        }
    }
}
