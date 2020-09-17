using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.Communication.WebSocket;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.WebClient;

namespace StarBlue.Communication.Packets.Incoming.WebSocket
{
    class BuilderToolEvent : IPacketWebEvent
    {
        public void Parse(WebClient Session, MessageWebEvent Packet)
        {
            if (Session == null)
                return;

            GameClient Client = StarBlueServer.GetGame().GetClientManager().GetClientByUserID(Session.UserId);
            if (Client == null)
            {
                return;
            }

            if (!Client.GetHabbo().CurrentRoom.CheckRights(Client, false, true))
            {
                Client.SendMessage(RoomNotificationComposer.SendBubble("advice", "Você não tem permissão para usar a ferramenta aqui."));
                return;
            }

            string Action = Packet.PopString();
            string Value = Packet.PopString();

            switch (Action)
            {
                case "on":
                    Client.GetHabbo().BuilderTool = true;
                    Client.SendMessage(RoomNotificationComposer.SendBubble("buildertool", "Ferramenta de construção ativada."));
                    break;

                case "off":
                case "closed":
                    foreach (RoomUser User in Client.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUsers())
                    {
                        User.Frozen = false;
                    }

                    Client.GetHabbo().StackHeight = 0;
                    Client.GetHabbo().FurniRotation = -1;
                    Client.GetHabbo().FurniState = -1;
                    Client.SendMessage(RoomNotificationComposer.SendBubble("buildertool", "Ferramenta de construção desativada."));
                    Client.GetHabbo().BuilderTool = false;
                    break;

                case "stack":
                    if (Value.Equals("off"))
                    {
                        Client.GetHabbo().StackHeight = 0;
                        Client.SendMessage(RoomNotificationComposer.SendBubble("buildertool", "Ferramenta de altura do quarto desativada."));
                        return;
                    }

                    if (!double.TryParse(Value, out Client.GetHabbo().StackHeight))
                    {
                        Client.SendMessage(RoomNotificationComposer.SendBubble("advice", "Valor inválido, insira um valor númerico."));
                        return;
                    }

                    Client.SendMessage(RoomNotificationComposer.SendBubble("buildertool", "Sucesso, altura alterada para " + Client.GetHabbo().StackHeight));
                    break;

                case "rotation":
                    if (Value.Equals("off"))
                    {
                        Client.GetHabbo().FurniRotation = -1;
                        Client.SendMessage(RoomNotificationComposer.SendBubble("buildertool", "Ferramenta de direção desativada."));
                        return;
                    }

                    int newFurniRotation = -1;
                    if (!int.TryParse(Value, out newFurniRotation))
                    {
                        Client.SendMessage(RoomNotificationComposer.SendBubble("advice", "Valor inválido, insira um valor númerico."));
                        return;
                    }

                    if (newFurniRotation > 8)
                    {
                        Client.SendMessage(RoomNotificationComposer.SendBubble("advice", "Valor inválido, insira um valor menor que 8."));
                        return;
                    }

                    Client.GetHabbo().FurniRotation = newFurniRotation;
                    Client.SendMessage(RoomNotificationComposer.SendBubble("buildertool", "Sucesso, direção alterada para " + newFurniRotation));
                    break;

                case "state":
                    if (Value.Equals("off"))
                    {
                        Client.GetHabbo().FurniState = -1;
                        Client.SendMessage(RoomNotificationComposer.SendBubble("buildertool", "Ferramenta de estado desativada."));
                        return;
                    }

                    int newFurniState = -1;
                    if (!int.TryParse(Value, out newFurniState))
                    {
                        Client.SendMessage(RoomNotificationComposer.SendBubble("advice", "Valor inválido, insira um valor númerico."));
                        return;
                    }

                    Client.GetHabbo().FurniState = newFurniState;
                    Client.SendMessage(RoomNotificationComposer.SendBubble("buildertool", "Sucesso, estado alterado para " + newFurniState));
                    break;

                case "movement":
                    foreach (RoomUser User in Client.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUsers())
                    {
                        if (Value.Equals("on"))
                        {
                            User.Frozen = false;
                            User.GetClient().SendMessage(RoomNotificationComposer.SendBubble("buildertool", "Movimentação de usuários do quarto ativada."));
                        }
                        else
                        {
                            User.GetClient().SendMessage(RoomNotificationComposer.SendBubble("buildertool", "Movimentação de usuários do quarto desativada."));
                            User.Frozen = true;
                        }
                    }



                    break;

            }
        }
    }
}
