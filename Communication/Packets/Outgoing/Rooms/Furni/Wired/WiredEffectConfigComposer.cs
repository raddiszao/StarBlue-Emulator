using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Items.Wired;
using System;
using System.Collections.Generic;
using System.Linq;


namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni.Wired
{
    internal class WiredEffectConfigComposer : ServerPacket
    {
        public WiredEffectConfigComposer(IWiredItem Box, List<int> BlockedItems)
            : base(ServerPacketHeader.WiredEffectConfigMessageComposer)
        {
            base.WriteBoolean(false);
            if (Box.Type == WiredBoxType.EffectProgressUserAchievement || Box.Type == WiredBoxType.EffectTimerReset)
            {
                base.WriteInteger(0);
            }
            else
            {
                base.WriteInteger(20);
            }

            base.WriteInteger(Box.SetItems.Count);
            foreach (Item Item in Box.SetItems.Values.ToList())
            {
                base.WriteInteger(Item.Id);
            }

            base.WriteInteger(Box.Item.GetBaseItem().SpriteId);
            base.WriteInteger(Box.Item.Id);

            if (Box.Type == WiredBoxType.EffectBotGivesHanditemBox)
            {
                if (string.IsNullOrEmpty(Box.StringData))
                {
                    Box.StringData = "Nome do Bot;0";
                }

                base.WriteString(Box.StringData != null ? (Box.StringData.Split(';')[0]) : "");
                base.WriteInteger(1);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[1]) : 0);
            }
            else if (Box.Type == WiredBoxType.EffectAddActorToTeam)
            {
                WriteString("");
                WriteInteger(1);
                WriteInteger(Convert.ToInt32(string.IsNullOrEmpty(Box.StringData) ? "0" : Box.StringData));
            }
            else if (Box.Type == WiredBoxType.EffectGiveScoreTeam)
            {
                if (String.IsNullOrEmpty(Box.StringData))
                    Box.StringData = "1;1;2";

                base.WriteString(Box.StringData != null ? (Box.StringData.Split(';')[0]) : "");
            }
            else if (Box.Type == WiredBoxType.EffectBotCommunicatesToAllBox || Box.Type == WiredBoxType.EffectBotCommunicatesToUserBox)
            {
                if (string.IsNullOrEmpty(Box.StringData))
                    Box.StringData = "Nome do Bot;Mensagem";

                WriteString(Box.StringData != null ? Box.StringData.Split(';')[0] : "");
                WriteInteger(1);
                WriteInteger(Box.BoolData ? 1 : 0);
            }
            else if (Box.Type == WiredBoxType.EffectBotFollowsUserBox)
            {
                if (string.IsNullOrEmpty(Box.StringData))
                {
                    Box.StringData = "0;Nome do Bot";
                }

                base.WriteString(Box.StringData != null ? (Box.StringData.Split(';')[1]) : "");
            }
            else if (Box.Type == WiredBoxType.EffectGiveReward)
            {
                if (string.IsNullOrEmpty(Box.StringData))
                {
                    Box.StringData = "1,,;1,,;1,,;1,,;1,,-0-0-0";
                }

                base.WriteString(Box.StringData != null ? (Box.StringData.Split('-')[0]) : "");
            }
            else if (Box.Type == WiredBoxType.EffectMoveToDir)
            {
                base.WriteString("");
            }
            else if (Box.Type == WiredBoxType.EffectTimerReset)
            {
                base.WriteString("");
            }
            else if (Box.Type == WiredBoxType.EffectMuteTriggerer)
            {
                if (string.IsNullOrEmpty(Box.StringData))
                {
                    Box.StringData = "0;Mensagem";
                }
                base.WriteString(Box.StringData != null ? (Box.StringData.Split(';')[1]) : "");
            }
            else
            {
                base.WriteString(Box.StringData);
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
                base.WriteInteger(0); // Loop

            else if (Box.Type == WiredBoxType.EffectMatchPosition)
            {
                if (string.IsNullOrEmpty(Box.StringData))
                {
                    Box.StringData = "0;0;0";
                }

                base.WriteInteger(3);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 0);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[1]) : 0);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[2]) : 0);
            }
            else if (Box.Type == WiredBoxType.EffectMoveToDir)
            {
                if (string.IsNullOrEmpty(Box.StringData))
                {
                    Box.StringData = "0;0";
                }

                base.WriteInteger(2);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 50);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[1]) : 5);
            }
            else if (Box.Type == WiredBoxType.EffectGiveReward)
            {
                base.WriteInteger(4);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split('-')[1]) : 0);
                base.WriteInteger(Box.BoolData ? 1 : 0);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split('-')[2]) : 0);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split('-')[3]) : 1);
            }
            else if (Box.Type == WiredBoxType.EffectGiveScoreTeam)
            {
                if (string.IsNullOrEmpty(Box.StringData))
                {
                    Box.StringData = "1;1;0";
                }

                base.WriteInteger(3);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 0);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[1]) : 0);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[2]) : 0);
            }
            else if (Box.Type == WiredBoxType.EffectAddScore || Box.Type == WiredBoxType.EffectAddRewardPoints)
            {

                if (string.IsNullOrEmpty(Box.StringData))
                {
                    Box.StringData = "1;1";
                }

                base.WriteInteger(2);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 0);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[1]) : 0);
            }
            else if (Box.Type == WiredBoxType.EffectMoveAndRotate)
            {
                if (string.IsNullOrEmpty(Box.StringData))
                {
                    Box.StringData = "0;0";
                }

                base.WriteInteger(2);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 0);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[1]) : 0);
            }
            else if (Box.Type == WiredBoxType.EffectMuteTriggerer)
            {
                base.WriteInteger(1);//Count, for the time.
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 0);
            }
            else if (Box.Type == WiredBoxType.EffectBotFollowsUserBox)
            {
                base.WriteInteger(1);//Count, for the time.
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 0);
            }

            if (Box is IWiredCycle && Box.Type != WiredBoxType.EffectGiveScoreTeam && Box.Type != WiredBoxType.EffectGiveUserDance && Box.Type != WiredBoxType.EffectMuteTriggerer && Box.Type != WiredBoxType.SendCustomMessageBox && Box.Type != WiredBoxType.EffectShowMessageNux && Box.Type != WiredBoxType.EffectShowMessageCustom && Box.Type != WiredBoxType.EffectBotCommunicatesToUserBox && Box.Type != WiredBoxType.EffectTeleportBotToFurniBox && Box.Type != WiredBoxType.EffectAddActorToTeam && Box.Type != WiredBoxType.EffectRemoveActorFromTeam && Box.Type != WiredBoxType.EffectProgressUserAchievement && Box.Type != WiredBoxType.EffectRoomForward && Box.Type != WiredBoxType.EffectRegenerateMaps && Box.Type != WiredBoxType.EffectSetRollerSpeed && Box.Type != WiredBoxType.EffectSendYouTubeVideo && Box.Type != WiredBoxType.EffectBotGivesHanditemBox && Box.Type != WiredBoxType.EffectBotFollowsUserBox && Box.Type != WiredBoxType.EffectBotMovesToFurniBox && Box.Type != WiredBoxType.EffectBotChangesClothesBox && Box.Type != WiredBoxType.EffectActionDimmer && Box.Type != WiredBoxType.EffectBotCommunicatesToAllBox && Box.Type != WiredBoxType.EffectKickUser && Box.Type != WiredBoxType.EffectMatchPosition && Box.Type != WiredBoxType.EffectMoveAndRotate && Box.Type != WiredBoxType.EffectSetRollerSpeed && Box.Type != WiredBoxType.EffectAddScore && Box.Type != WiredBoxType.EffectAddRewardPoints &&
                Box.Type != WiredBoxType.EffectMoveToDir && Box.Type != WiredBoxType.EffectShowMessage && Box.Type != WiredBoxType.EffectGiveUserHanditem && Box.Type != WiredBoxType.EffectGiveUserEnable && Box.Type != WiredBoxType.EffectTimerReset
                && Box.Type != WiredBoxType.EffectGiveUserFreeze && Box.Type != WiredBoxType.EffectExecuteWiredStacks)
            {
                IWiredCycle Cycle = (IWiredCycle)Box;
                base.WriteInteger(WiredBoxTypeUtility.GetWiredId(Box.Type));
                base.WriteInteger(0);
                if (Box.Type == WiredBoxType.EffectCloseDices || Box.Type == WiredBoxType.EffectLowerFurni || Box.Type == WiredBoxType.EffectRaiseFurni || Box.Type == WiredBoxType.EffectMoveFurniFromNearestUser || Box.Type == WiredBoxType.EffectMoveFurniToAwayUser || Box.Type == WiredBoxType.EffectToggleNegativeFurniState || Box.Type == WiredBoxType.EffectToggleFurniState || Box.Type == WiredBoxType.EffectMoveFurniToNearestUser || Box.Type == WiredBoxType.EffectMoveFurniToAwayUser)
                {
                    base.WriteInteger(Cycle.Delay / 500);
                }
                else
                {
                    base.WriteInteger(Cycle.Delay);
                }
            }
            else if (Box.Type == WiredBoxType.EffectGiveScoreTeam || Box.Type == WiredBoxType.EffectGiveUserDance || Box.Type == WiredBoxType.EffectMuteTriggerer || Box.Type == WiredBoxType.SendCustomMessageBox || Box.Type == WiredBoxType.EffectShowMessageNux || Box.Type == WiredBoxType.EffectShowMessageCustom || Box.Type == WiredBoxType.EffectBotCommunicatesToUserBox || Box.Type == WiredBoxType.EffectTeleportBotToFurniBox || Box.Type == WiredBoxType.EffectAddActorToTeam || Box.Type == WiredBoxType.EffectRemoveActorFromTeam || Box.Type == WiredBoxType.EffectProgressUserAchievement || Box.Type == WiredBoxType.EffectRoomForward || Box.Type == WiredBoxType.EffectRegenerateMaps || Box.Type == WiredBoxType.EffectSetRollerSpeed || Box.Type == WiredBoxType.EffectSendYouTubeVideo || Box.Type == WiredBoxType.EffectBotGivesHanditemBox || Box.Type == WiredBoxType.EffectBotFollowsUserBox || Box.Type == WiredBoxType.EffectBotMovesToFurniBox || Box.Type == WiredBoxType.EffectBotChangesClothesBox || Box.Type == WiredBoxType.EffectActionDimmer || Box.Type == WiredBoxType.EffectBotCommunicatesToAllBox || Box.Type == WiredBoxType.EffectKickUser || Box.Type == WiredBoxType.EffectMatchPosition || Box.Type == WiredBoxType.EffectMoveAndRotate || Box.Type == WiredBoxType.EffectAddScore || Box.Type == WiredBoxType.EffectAddRewardPoints || Box.Type == WiredBoxType.EffectMoveToDir || Box.Type == WiredBoxType.EffectShowMessage || Box.Type == WiredBoxType.EffectGiveUserHanditem || Box.Type == WiredBoxType.EffectGiveUserEnable || Box.Type == WiredBoxType.EffectTimerReset || Box.Type == WiredBoxType.EffectGiveUserFreeze || Box.Type == WiredBoxType.EffectExecuteWiredStacks)
            {
                IWiredCycle Cycle = (IWiredCycle)Box;
                base.WriteInteger(0);
                base.WriteInteger(WiredBoxTypeUtility.GetWiredId(Box.Type));
                if (Box.Type == WiredBoxType.EffectTimerReset || Box.Type == WiredBoxType.EffectGiveScoreTeam || Box.Type == WiredBoxType.EffectTeleportBotToFurniBox || Box.Type == WiredBoxType.EffectRegenerateMaps || Box.Type == WiredBoxType.EffectSetRollerSpeed || Box.Type == WiredBoxType.EffectBotMovesToFurniBox || Box.Type == WiredBoxType.EffectBotChangesClothesBox || Box.Type == WiredBoxType.EffectActionDimmer || Box.Type == WiredBoxType.EffectMoveToDir || Box.Type == WiredBoxType.EffectMoveAndRotate)
                {
                    base.WriteInteger(Cycle.Delay / 500);
                }
                else
                {
                    base.WriteInteger(Cycle.Delay);
                }
            }

            else
            {
                base.WriteInteger(0);
                base.WriteInteger(WiredBoxTypeUtility.GetWiredId(Box.Type));
                base.WriteInteger(0);
            }

            base.WriteInteger(BlockedItems.Count());
            if (BlockedItems.Count() > 0)
            {
                foreach (int ItemId in BlockedItems.ToList())
                {
                    base.WriteInteger(ItemId);
                }
            }
        }
    }
}