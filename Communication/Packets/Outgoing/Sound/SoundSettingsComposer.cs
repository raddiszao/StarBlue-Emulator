using System;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Sound
{
    class SoundSettingsComposer : ServerPacket
    {
        public SoundSettingsComposer(ICollection<int> ClientVolumes, Boolean ChatPreference, Boolean InvitesStatus, Boolean FocusPreference, int FriendBarState)
            : base(ServerPacketHeader.SoundSettingsMessageComposer)
        {
            foreach (int VolumeValue in ClientVolumes)
            {
                base.WriteInteger(VolumeValue);
            }

            base.WriteBoolean(ChatPreference);
            base.WriteBoolean(InvitesStatus);
            base.WriteBoolean(FocusPreference);
            base.WriteInteger(FriendBarState);
            base.WriteInteger(0);
            base.WriteInteger(0);
        }
    }
}