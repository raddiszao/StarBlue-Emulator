using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Users
{
    public class IgnoredUsersComposer : MessageComposer
    {
        private IReadOnlyCollection<string> ignoredUsers { get; }

        public IgnoredUsersComposer(IReadOnlyCollection<string> ignoredUsers)
            : base(Composers.IgnoredUsersMessageComposer)
        {
            this.ignoredUsers = ignoredUsers;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(ignoredUsers.Count);
            foreach (string username in ignoredUsers)
            {
                packet.WriteString(username);
            }
        }
    }
}
