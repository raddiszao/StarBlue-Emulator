using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Sound
{
    internal class SoundSettingsComposer : MessageComposer
    {
        private ICollection<int> ClientVolumes { get; }
        private bool ChatPreference { get; }
        private bool InvitesStatus { get; }
        private bool FocusPreference { get; }
        private int FriendBarState { get; }


        public SoundSettingsComposer(ICollection<int> ClientVolumes, bool ChatPreference, bool InvitesStatus, bool FocusPreference, int FriendBarState)
            : base(Composers.SoundSettingsMessageComposer)
        {
            this.ClientVolumes = ClientVolumes;
            this.ChatPreference = ChatPreference;
            this.InvitesStatus = InvitesStatus;
            this.FocusPreference = FocusPreference;
            this.FriendBarState = FriendBarState;
        }

        public override void Compose(Composer packet)
        {
            foreach (int VolumeValue in ClientVolumes)
            {
                packet.WriteInteger(VolumeValue);
            }

            packet.WriteBoolean(ChatPreference);
            packet.WriteBoolean(InvitesStatus);
            packet.WriteBoolean(FocusPreference);
            packet.WriteInteger(FriendBarState);
            packet.WriteInteger(0);
            packet.WriteInteger(0);
        }
    }
}