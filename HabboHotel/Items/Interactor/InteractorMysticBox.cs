﻿using StarBlue.Communication.Packets.Outgoing.Rooms.Furni;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.HabboHotel.Items.Interactor
{
    class InteractorMysticBox : IFurniInteractor
    {
        private string eColor;

        public void OnPlace(GameClient Session, Item Item)
        {
            if (Session.GetHabbo().Id != Item.UserID)
            {
                Session.SendWhisper("Parece que você está tentando colocar uma caixa que não é sua.", 33);
                return;
            }
            foreach (string key in Session.GetHabbo().MysticBoxes)
            {
                switch (key)
                {
                    case "purple":
                        eColor = "1";
                        break;
                    case "blue":
                        eColor = "4";
                        break;
                    case "green":
                        eColor = "7";
                        break;
                    case "yellow":
                        eColor = "10";
                        break;
                    case "lilac":
                        eColor = "13";
                        break;
                    case "orange":
                        eColor = "16";
                        break;
                    case "turquoise":
                        eColor = "19";
                        break;
                    case "red":
                        eColor = "22";
                        break;
                }
                Session.GetHabbo().eColor = eColor;
            }
            Item.ExtraData = eColor;
            Item.UpdateState(false, true);
        }

        public void OnRemove(GameClient Session, Item Item)
        {
        }

        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            Session.SendMessage(new MysticBoxRewardComposer("s", 230));
        }

        public void OnWiredTrigger(Item Item)
        {
        }
    }
}
