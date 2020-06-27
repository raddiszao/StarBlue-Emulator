﻿using StarBlue.HabboHotel.GameClients;
using System;

namespace StarBlue.HabboHotel.Items.Interactor
{
    class InteractorCounter : IFurniInteractor
    {
        public void OnPlace(GameClient Session, Item Item)
        {
            Item.ExtraData = "30";
            Item.UpdateState();
        }

        public void OnRemove(GameClient Session, Item Item)
        {
        }

        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            if (!HasRights)
            {
                return;
            }


            if (!int.TryParse(Item.ExtraData, out int oldValue))
            {
                Item.ExtraData = "30";
                oldValue = 30;
            }

            if (Request == 0 && oldValue == 0)
            {
                oldValue = 30;
            }
            else if (Request == 2)
            {
                if (Item.GetRoom().GetSoccer().GameIsStarted && Item.pendingReset && oldValue > 0)
                {
                    oldValue = 0;
                    Item.pendingReset = false;
                }
                else
                {
                    if (oldValue < 30)
                    {
                        oldValue = 30;
                    }
                    else if (oldValue == 30)
                    {
                        oldValue = 60;
                    }
                    else if (oldValue == 60)
                    {
                        oldValue = 120;
                    }
                    else if (oldValue == 120)
                    {
                        oldValue = 180;
                    }
                    else if (oldValue == 180)
                    {
                        oldValue = 300;
                    }
                    else if (oldValue == 300)
                    {
                        oldValue = 600;
                    }
                    else
                    {
                        oldValue = 0;
                    }

                    Item.UpdateNeeded = false;
                }
            }
            else if (Request == 1 || Request == 0)
            {
                if (Request == 1 && oldValue == 0)
                {
                    Item.ExtraData = "30";
                    oldValue = 30;
                }

                if (!Item.GetRoom().GetSoccer().GameIsStarted)
                {
                    Item.UpdateNeeded = !Item.UpdateNeeded;

                    if (Item.UpdateNeeded)
                    {
                        Item.GetRoom().GetSoccer().StartGame();
                    }

                    Item.pendingReset = true;
                }
                else
                {
                    Item.UpdateNeeded = !Item.UpdateNeeded;

                    if (Item.UpdateNeeded)
                    {
                        Item.GetRoom().GetSoccer().StopGame(true);
                    }

                    Item.pendingReset = true;
                }
            }


            Item.ExtraData = Convert.ToString(oldValue);
            Item.UpdateState();
        }

        public void OnWiredTrigger(Item Item)
        {
            if (Item.GetRoom().GetSoccer().GameIsStarted)
            {
                Item.GetRoom().GetSoccer().StopGame(true);
            }

            Item.pendingReset = true;
            Item.UpdateNeeded = true;
            Item.ExtraData = "30";
            Item.UpdateState();

            Item.GetRoom().GetSoccer().StartGame();
        }
    }
}
