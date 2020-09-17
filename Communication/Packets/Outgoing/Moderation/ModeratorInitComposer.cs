using StarBlue.HabboHotel.Moderation;
using StarBlue.Utilities;
using System;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorInitComposer : MessageComposer
    {
        private ICollection<string> UserPresets { get; }
        private ICollection<string> RoomPresets { get; }
        private ICollection<ModerationTicket> Tickets { get; }

        public ModeratorInitComposer(ICollection<string> UserPresets, ICollection<string> RoomPresets, ICollection<ModerationTicket> Tickets)
            : base(Composers.ModeratorInitMessageComposer)
        {
            this.UserPresets = UserPresets;
            this.RoomPresets = RoomPresets;
            this.Tickets = Tickets;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Tickets.Count);
            foreach (ModerationTicket Ticket in Tickets)
            {
                packet.WriteInteger(Ticket.Id); // Id
                packet.WriteInteger(Ticket.GetStatus(Header)); // Tab ID
                packet.WriteInteger(Ticket.Type); // Type
                packet.WriteInteger(Ticket.Category); // Category
                packet.WriteInteger(Convert.ToInt32((DateTime.Now - UnixTimestamp.FromUnixTimestamp(Ticket.Timestamp)).TotalMilliseconds)); // This should fix the overflow?
                packet.WriteInteger(Ticket.Priority); // Priority
                packet.WriteInteger(Ticket.Sender == null ? 0 : Ticket.Sender.Id); // Sender ID
                packet.WriteInteger(1);
                packet.WriteString(Ticket.Sender == null ? string.Empty : Ticket.Sender.Username); // Sender Name
                packet.WriteInteger(Ticket.Reported == null ? 0 : Ticket.Reported.Id); // Reported ID
                packet.WriteString(Ticket.Reported == null ? string.Empty : Ticket.Reported.Username); // Reported Name
                packet.WriteInteger(Ticket.Moderator == null ? 0 : Ticket.Moderator.Id); // Moderator ID
                packet.WriteString(Ticket.Moderator == null ? string.Empty : Ticket.Moderator.Username); // Mod Name
                packet.WriteString(Ticket.Issue); // Issue
                packet.WriteInteger(Ticket.Room == null ? 0 : Ticket.Room.Id); // Room Id
                packet.WriteInteger(0);//LOOP
            }

            packet.WriteInteger(UserPresets.Count);
            foreach (string pre in UserPresets)
            {
                packet.WriteString(pre);
            }

            /*packet.WriteInteger(UserActionPresets.Count);
            foreach (KeyValuePair<string, List<ModerationPresetActionMessages>> Cat in UserActionPresets.ToList())
            {
                packet.WriteString(Cat.Key);
                packet.WriteBoolean(true);
                packet.WriteInteger(Cat.Value.Count);
                foreach (ModerationPresetActionMessages Preset in Cat.Value.ToList())
                {
                    packet.WriteString(Preset.Caption);
                    packet.WriteString(Preset.MessageText);
                    packet.WriteInteger(Preset.BanTime); // Account Ban Hours
                    packet.WriteInteger(Preset.IPBanTime); // IP Ban Hours
                    packet.WriteInteger(Preset.MuteTime); // Mute in Hours
                    packet.WriteInteger(0);//Trading lock duration
                    packet.WriteString(Preset.Notice + "\n\nPlease Note: Avatar ban is an IP ban!");
                    packet.WriteBoolean(false);//Show HabboWay
                }
            }*/

            // TODO: Figure out
            packet.WriteInteger(0);
            {
                //Loop a string.
            }

            packet.WriteBoolean(true); // Ticket right
            packet.WriteBoolean(true); // Chatlogs
            packet.WriteBoolean(true); // User actions alert etc
            packet.WriteBoolean(true); // Kick users
            packet.WriteBoolean(true); // Ban users
            packet.WriteBoolean(true); // Caution etc
            packet.WriteBoolean(true); // Love you, Tom

            packet.WriteInteger(RoomPresets.Count);
            foreach (string pre in RoomPresets)
            {
                packet.WriteString(pre);
            }
        }
    }
}
