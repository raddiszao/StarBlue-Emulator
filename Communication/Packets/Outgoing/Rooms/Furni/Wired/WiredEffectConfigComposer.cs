using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Items.Wired;
using System;
using System.Collections.Generic;
using System.Linq;


namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni.Wired
{
    internal class WiredEffectConfigComposer : MessageComposer
    {
        public IWiredItem Box { get; }
        public List<int> BlockedItems { get; }

        public WiredEffectConfigComposer(IWiredItem Box, List<int> BlockedItems)
            : base(Composers.WiredEffectConfigMessageComposer)
        {
            this.Box = Box;
            this.BlockedItems = BlockedItems;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteBoolean(false);
            if (Box.Type == WiredBoxType.EffectProgressUserAchievement || Box.Type == WiredBoxType.EffectTimerReset)
            {
                packet.WriteInteger(0);
            }
            else
            {
                packet.WriteInteger(20);
            }

            packet.WriteInteger(Box.SetItems.Count);
            foreach (Item Item in Box.SetItems.Values.ToList())
            {
                packet.WriteInteger(Item.Id);
            }

            packet.WriteInteger(Box.Item.GetBaseItem().SpriteId);
            packet.WriteInteger(Box.Item.Id);

            if (Box.Type == WiredBoxType.EffectBotGivesHanditemBox)
            {
                if (string.IsNullOrEmpty(Box.StringData))
                {
                    Box.StringData = "Nome do Bot;0";
                }

                packet.WriteString(Box.StringData != null ? (Box.StringData.Split(';')[0]) : "");
                packet.WriteInteger(1);
                packet.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[1]) : 0);
            }
            else if (Box.Type == WiredBoxType.EffectAddActorToTeam)
            {
                packet.WriteString("");
                packet.WriteInteger(1);
                packet.WriteInteger(Convert.ToInt32(string.IsNullOrEmpty(Box.StringData) ? "0" : Box.StringData));
            }
            else if (Box.Type == WiredBoxType.EffectGiveScoreTeam)
            {
                if (String.IsNullOrEmpty(Box.StringData))
                    Box.StringData = "1;1;2";

                packet.WriteString(Box.StringData != null ? (Box.StringData.Split(';')[0]) : "");
            }
            else if (Box.Type == WiredBoxType.EffectBotCommunicatesToAllBox || Box.Type == WiredBoxType.EffectBotCommunicatesToUserBox)
            {
                if (string.IsNullOrEmpty(Box.StringData))
                    Box.StringData = "Nome do Bot;Mensagem";

                packet.WriteString(Box.StringData != null ? Box.StringData.Split(';')[0] : "");
                packet.WriteInteger(1);
                packet.WriteInteger(Box.BoolData ? 1 : 0);
            }
            else if (Box.Type == WiredBoxType.EffectBotFollowsUserBox)
            {
                if (string.IsNullOrEmpty(Box.StringData))
                {
                    Box.StringData = "0;Nome do Bot";
                }

                packet.WriteString(Box.StringData != null ? (Box.StringData.Split(';')[1]) : "");
            }
            else if (Box.Type == WiredBoxType.EffectGiveReward)
            {
                if (string.IsNullOrEmpty(Box.StringData))
                {
                    Box.StringData = "1,,;1,,;1,,;1,,;1,,-0-0-0";
                }

                packet.WriteString(Box.StringData != null ? (Box.StringData.Split('-')[0]) : "");
            }
            else if (Box.Type == WiredBoxType.EffectMoveToDir)
            {
                packet.WriteString("");
            }
            else if (Box.Type == WiredBoxType.EffectTimerReset)
            {
                packet.WriteString("");
            }
            else if (Box.Type == WiredBoxType.EffectMuteTriggerer)
            {
                if (string.IsNullOrEmpty(Box.StringData))
                {
                    Box.StringData = "0;Mensagem";
                }
                packet.WriteString(Box.StringData != null ? (Box.StringData.Split(';')[1]) : "");
            }
            else
            {
                packet.WriteString(Box.StringData);
            }

            if (Box.Type != WiredBoxType.EffectMatchPosition &&
                Box.Type != WiredBoxType.EffectMoveAndRotate &&
                Box.Type != WiredBoxType.EffectMuteTriggerer &&
                Box.Type != WiredBoxType.EffectBotFollowsUserBox &&
                Box.Type != WiredBoxType.EffectAddScore &&
                Box.Type != WiredBoxType.EffectMoveToDir &&
                Box.Type != WiredBoxType.EffectGiveReward &&
                Box.Type != WiredBoxType.EffectAddRewardPoints &&
                Box.Type != WiredBoxType.EffectAddActorToTeam &&
                Box.Type != WiredBoxType.EffectGiveScoreTeam &&
                Box.Type != WiredBoxType.EffectBotCommunicatesToAllBox &&
                Box.Type != WiredBoxType.EffectBotCommunicatesToUserBox &&
                Box.Type != WiredBoxType.EffectBotGivesHanditemBox)
                packet.WriteInteger(0); // Loop

            else if (Box.Type == WiredBoxType.EffectMatchPosition)
            {
                if (string.IsNullOrEmpty(Box.StringData))
                {
                    Box.StringData = "0;0;0";
                }

                packet.WriteInteger(3);
                packet.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 0);
                packet.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[1]) : 0);
                packet.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[2]) : 0);
            }
            else if (Box.Type == WiredBoxType.EffectMoveToDir)
            {
                if (string.IsNullOrEmpty(Box.StringData))
                {
                    Box.StringData = "0;0";
                }

                packet.WriteInteger(2);
                packet.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 50);
                packet.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[1]) : 5);
            }
            else if (Box.Type == WiredBoxType.EffectGiveReward)
            {
                packet.WriteInteger(4);
                packet.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split('-')[1]) : 0);
                packet.WriteInteger(Box.BoolData ? 1 : 0);
                packet.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split('-')[2]) : 0);
                packet.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split('-')[3]) : 1);
            }
            else if (Box.Type == WiredBoxType.EffectGiveScoreTeam)
            {
                if (string.IsNullOrEmpty(Box.StringData))
                {
                    Box.StringData = "1;1;0";
                }

                packet.WriteInteger(3);
                packet.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 0);
                packet.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[1]) : 0);
                packet.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[2]) : 0);
            }
            else if (Box.Type == WiredBoxType.EffectAddScore || Box.Type == WiredBoxType.EffectAddRewardPoints)
            {

                if (string.IsNullOrEmpty(Box.StringData))
                {
                    Box.StringData = "1;1";
                }

                packet.WriteInteger(2);
                packet.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 0);
                packet.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[1]) : 0);
            }
            else if (Box.Type == WiredBoxType.EffectMoveAndRotate)
            {
                if (string.IsNullOrEmpty(Box.StringData))
                {
                    Box.StringData = "0;0";
                }

                packet.WriteInteger(2);
                packet.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 0);
                packet.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[1]) : 0);
            }
            else if (Box.Type == WiredBoxType.EffectMuteTriggerer)
            {
                packet.WriteInteger(1);//Count, for the time.
                packet.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 0);
            }
            else if (Box.Type == WiredBoxType.EffectBotFollowsUserBox)
            {
                packet.WriteInteger(1);//Count, for the time.
                packet.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 0);
            }

            if (Box is IWiredCycle && Box.Type != WiredBoxType.EffectGiveScoreTeam && Box.Type != WiredBoxType.EffectGiveUserDance && Box.Type != WiredBoxType.EffectMuteTriggerer && Box.Type != WiredBoxType.SendCustomMessageBox && Box.Type != WiredBoxType.EffectShowMessageNux && Box.Type != WiredBoxType.EffectShowMessageCustom && Box.Type != WiredBoxType.EffectBotCommunicatesToUserBox && Box.Type != WiredBoxType.EffectTeleportBotToFurniBox && Box.Type != WiredBoxType.EffectAddActorToTeam && Box.Type != WiredBoxType.EffectRemoveActorFromTeam && Box.Type != WiredBoxType.EffectProgressUserAchievement && Box.Type != WiredBoxType.EffectRoomForward && Box.Type != WiredBoxType.EffectRegenerateMaps && Box.Type != WiredBoxType.EffectSetRollerSpeed && Box.Type != WiredBoxType.EffectSendYouTubeVideo && Box.Type != WiredBoxType.EffectBotGivesHanditemBox && Box.Type != WiredBoxType.EffectBotFollowsUserBox && Box.Type != WiredBoxType.EffectBotMovesToFurniBox && Box.Type != WiredBoxType.EffectBotChangesClothesBox && Box.Type != WiredBoxType.EffectActionDimmer && Box.Type != WiredBoxType.EffectBotCommunicatesToAllBox && Box.Type != WiredBoxType.EffectKickUser && Box.Type != WiredBoxType.EffectMatchPosition && Box.Type != WiredBoxType.EffectMoveAndRotate && Box.Type != WiredBoxType.EffectSetRollerSpeed && Box.Type != WiredBoxType.EffectAddScore && Box.Type != WiredBoxType.EffectAddRewardPoints &&
                Box.Type != WiredBoxType.EffectMoveToDir && Box.Type != WiredBoxType.EffectShowMessage && Box.Type != WiredBoxType.EffectGiveUserHanditem && Box.Type != WiredBoxType.EffectGiveUserEnable && Box.Type != WiredBoxType.EffectTimerReset
                && Box.Type != WiredBoxType.EffectGiveUserFreeze && Box.Type != WiredBoxType.EffectExecuteWiredStacks)
            {
                IWiredCycle Cycle = (IWiredCycle)Box;
                packet.WriteInteger(WiredBoxTypeUtility.GetWiredId(Box.Type));
                packet.WriteInteger(0);
                if (Box.Type == WiredBoxType.EffectCloseDices || Box.Type == WiredBoxType.EffectLowerFurni || Box.Type == WiredBoxType.EffectRaiseFurni || Box.Type == WiredBoxType.EffectMoveFurniFromNearestUser || Box.Type == WiredBoxType.EffectMoveFurniToAwayUser || Box.Type == WiredBoxType.EffectToggleNegativeFurniState || Box.Type == WiredBoxType.EffectToggleFurniState || Box.Type == WiredBoxType.EffectMoveFurniToNearestUser || Box.Type == WiredBoxType.EffectMoveFurniToAwayUser)
                {
                    packet.WriteInteger(Cycle.Delay / 500);
                }
                else
                {
                    packet.WriteInteger(Cycle.Delay);
                }
            }
            else if (Box.Type == WiredBoxType.EffectGiveScoreTeam || Box.Type == WiredBoxType.EffectGiveUserDance || Box.Type == WiredBoxType.EffectMuteTriggerer || Box.Type == WiredBoxType.SendCustomMessageBox || Box.Type == WiredBoxType.EffectShowMessageNux || Box.Type == WiredBoxType.EffectShowMessageCustom || Box.Type == WiredBoxType.EffectBotCommunicatesToUserBox || Box.Type == WiredBoxType.EffectTeleportBotToFurniBox || Box.Type == WiredBoxType.EffectAddActorToTeam || Box.Type == WiredBoxType.EffectRemoveActorFromTeam || Box.Type == WiredBoxType.EffectProgressUserAchievement || Box.Type == WiredBoxType.EffectRoomForward || Box.Type == WiredBoxType.EffectRegenerateMaps || Box.Type == WiredBoxType.EffectSetRollerSpeed || Box.Type == WiredBoxType.EffectSendYouTubeVideo || Box.Type == WiredBoxType.EffectBotGivesHanditemBox || Box.Type == WiredBoxType.EffectBotFollowsUserBox || Box.Type == WiredBoxType.EffectBotMovesToFurniBox || Box.Type == WiredBoxType.EffectBotChangesClothesBox || Box.Type == WiredBoxType.EffectActionDimmer || Box.Type == WiredBoxType.EffectBotCommunicatesToAllBox || Box.Type == WiredBoxType.EffectKickUser || Box.Type == WiredBoxType.EffectMatchPosition || Box.Type == WiredBoxType.EffectMoveAndRotate || Box.Type == WiredBoxType.EffectAddScore || Box.Type == WiredBoxType.EffectAddRewardPoints || Box.Type == WiredBoxType.EffectMoveToDir || Box.Type == WiredBoxType.EffectShowMessage || Box.Type == WiredBoxType.EffectGiveUserHanditem || Box.Type == WiredBoxType.EffectGiveUserEnable || Box.Type == WiredBoxType.EffectTimerReset || Box.Type == WiredBoxType.EffectGiveUserFreeze || Box.Type == WiredBoxType.EffectExecuteWiredStacks)
            {
                IWiredCycle Cycle = (IWiredCycle)Box;
                packet.WriteInteger(0);
                packet.WriteInteger(WiredBoxTypeUtility.GetWiredId(Box.Type));
                if (Box.Type == WiredBoxType.EffectTimerReset || Box.Type == WiredBoxType.EffectGiveScoreTeam || Box.Type == WiredBoxType.EffectTeleportBotToFurniBox || Box.Type == WiredBoxType.EffectRegenerateMaps || Box.Type == WiredBoxType.EffectSetRollerSpeed || Box.Type == WiredBoxType.EffectBotMovesToFurniBox || Box.Type == WiredBoxType.EffectBotChangesClothesBox || Box.Type == WiredBoxType.EffectActionDimmer || Box.Type == WiredBoxType.EffectMoveToDir || Box.Type == WiredBoxType.EffectMoveAndRotate)
                {
                    packet.WriteInteger(Cycle.Delay / 500);
                }
                else
                {
                    packet.WriteInteger(Cycle.Delay);
                }
            }

            else
            {
                packet.WriteInteger(0);
                packet.WriteInteger(WiredBoxTypeUtility.GetWiredId(Box.Type));
                packet.WriteInteger(0);
            }

            packet.WriteInteger(BlockedItems.Count());
            if (BlockedItems.Count() > 0)
            {
                foreach (int ItemId in BlockedItems.ToList())
                {
                    packet.WriteInteger(ItemId);
                }
            }
        }
    }
}