using StarBlue.Communication.Packets.Outgoing.Rooms.Chat;
using StarBlue.Communication.Packets.Outgoing.Rooms.Engine;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class EmojiCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_normal"; }
        }
        public string Parameters
        {
            get { return "Número do 1 ao 189."; }
        }
        public string Description
        {
            get { return "Manda um emoji"; }
        }
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Oops, você deve escrever um número de 1 - 189! Para ver a lista de emojis escreva :emoji list", 34);
                return;
            }
            string emoji = Params[1];

            if (emoji.Equals("list"))
            {
                Session.SendMessage(new MassEventComposer("habbopages/chat/emoji.txt"));
            }
            else
            {
                bool isNumeric = int.TryParse(emoji, out int emojiNum);
                if (isNumeric)
                {
                    string chatcolor = Session.GetHabbo().chatHTMLColour;
                    int chatsize = Session.GetHabbo().chatHTMLSize;

                    Session.GetHabbo().chatHTMLColour = "";
                    Session.GetHabbo().chatHTMLSize = 12;
                    switch (emojiNum)
                    {
                        default:
                            bool isValid = true;
                            if (emojiNum < 1)
                            {
                                isValid = false;
                            }

                            if (emojiNum > 189 && Session.GetHabbo().Rank < 6)
                            {
                                isValid = false;
                            }
                            if (isValid)
                            {
                                string Username;
                                RoomUser TargetUser = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Username);
                                if (emojiNum < 10)
                                {
                                    Username = "<img src='/swf/c_images/emoji/" + emojiNum + ".png' height='20' width='20'><br>    ";
                                }
                                else
                                {
                                    Username = "<img src='/swf/c_images/emoji/" + emojiNum + ".png' height='20' width='20'><br>    ";
                                }
                                if (Room != null)
                                {
                                    Room.SendMessage(new UserNameChangeComposer(Session.GetHabbo().CurrentRoomId, TargetUser.VirtualId, Username));
                                }

                                string Message = " ";
                                Room.SendMessage(new ChatComposer(TargetUser.VirtualId, Message, 0, TargetUser.LastBubble));
                                TargetUser.SendNamePacket();

                            }
                            else
                            {
                                Session.SendWhisper("Emoji invalido, deve ser numero de 1-189. Para ver a lista de emoji digite :emoji list", 34);
                            }

                            break;
                    }
                    Session.GetHabbo().chatHTMLColour = chatcolor;
                    Session.GetHabbo().chatHTMLSize = chatsize;
                }
                else
                {
                    Session.SendWhisper("Emoji invalido, deve ser numero de 1-189. Para ver a lista de emoji digite :emoji list", 34);
                }
            }
            return;
        }
    }
}
