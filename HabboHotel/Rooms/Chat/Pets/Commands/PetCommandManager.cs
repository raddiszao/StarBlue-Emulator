using StarBlue.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;


namespace StarBlue.HabboHotel.Rooms.Chat.Pets.Commands
{
    public class PetCommandManager
    {
        private Dictionary<int, string> _commandRegister;
        private Dictionary<string, string> _commandDatabase;
        private Dictionary<string, PetCommand> _petCommands;

        public PetCommandManager()
        {
            _petCommands = new Dictionary<string, PetCommand>();
            _commandRegister = new Dictionary<int, string>();
            _commandDatabase = new Dictionary<string, string>();

            Init();
        }

        public void Init()
        {
            _petCommands.Clear();
            _commandRegister.Clear();
            _commandDatabase.Clear();

            DataTable Table = null;
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `bots_pet_commands`");
                Table = dbClient.GetTable();

                if (Table != null)
                {
                    foreach (DataRow row in Table.Rows)
                    {
                        _commandRegister.Add(Convert.ToInt32(row[0]), row[1].ToString());
                        _commandDatabase.Add(row[1] + ".input", row[2].ToString());
                    }
                }
            }

            foreach (KeyValuePair<int, string> pair in _commandRegister)
            {
                int commandID = pair.Key;
                string commandStringedID = pair.Value;
                string[] commandInput = _commandDatabase[commandStringedID + ".input"].Split(',');

                foreach (string command in commandInput)
                {
                    _petCommands.Add(command, new PetCommand(commandID, command));
                }
            }
        }

        public int TryInvoke(string Input)
        {
            if (_petCommands.TryGetValue(Input.ToLower(), out PetCommand Command))
            {
                return Command.Id;
            }

            return 0;
        }
    }
}