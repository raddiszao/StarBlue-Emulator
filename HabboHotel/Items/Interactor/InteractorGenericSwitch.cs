﻿using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Quests;


namespace StarBlue.HabboHotel.Items.Interactor
{
    public class InteractorGenericSwitch : IFurniInteractor
    {
        public void OnPlace(GameClient Session, Item Item)
        {
        }

        public void OnRemove(GameClient Session, Item Item)
        {
        }

        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            int Modes = Item.GetBaseItem().Modes - 1;

            if (Session == null || !HasRights || Modes <= 0)
            {
                return;
            }

            StarBlueServer.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.FURNI_SWITCH);

            int NewMode = 0;

            if (!int.TryParse(Item.ExtraData, out int CurrentMode))
            {
            }

            if (CurrentMode <= 0)
            {
                NewMode = 1;
            }
            else if (CurrentMode >= Modes)
            {
                NewMode = 0;
            }
            else
            {
                NewMode = CurrentMode + 1;
            }

            Item.ExtraData = NewMode.ToString();
            Item.UpdateState();
        }

        public void OnWiredTrigger(Item Item)
        {
            int Modes = Item.GetBaseItem().Modes - 1;

            if (Modes == 0)
            {
                return;
            }

            int NewMode = 0;

            if (string.IsNullOrEmpty(Item.ExtraData))
            {
                Item.ExtraData = "0";
            }

            if (!int.TryParse(Item.ExtraData, out int CurrentMode))
            {
                return;
            }

            if (CurrentMode <= 0)
            {
                NewMode = 1;
            }
            else if (CurrentMode >= Modes)
            {
                NewMode = 0;
            }
            else
            {
                NewMode = CurrentMode + 1;
            }

            Item.ExtraData = NewMode.ToString();
            Item.UpdateState();
        }
    }
}