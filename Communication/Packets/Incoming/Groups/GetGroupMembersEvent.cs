using StarBlue.Communication.Packets.Outgoing.Groups;
using StarBlue.HabboHotel.Groups;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.Communication.Packets.Incoming.Groups
{
    internal class GetGroupMembersEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            int GroupId = Packet.PopInt();
            int Page = Packet.PopInt();
            string SearchVal = Packet.PopString();
            int RequestType = Packet.PopInt();

            if (!StarBlueServer.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group Group))
            {
                return;
            }

            List<GroupMember> Members = new List<GroupMember>();

            switch (RequestType)
            {
                case 0:
                    {
                        foreach (KeyValuePair<int, string> Data in Group.GetAllMembers)
                        {
                            GroupMember GroupMember = Group.GetGroupMember(Data.Key, Group.Id);
                            if (GroupMember == null)
                            {
                                continue;
                            }

                            if (!Members.Contains(GroupMember))
                            {
                                Members.Add(GroupMember);
                            }
                        }
                        break;
                    }

                case 1:
                    {
                        ConcurrentDictionary<int, string> AdminIds = Group.GetAdministrators;
                        foreach (int Id in AdminIds.Keys)
                        {
                            GroupMember GroupMember = Group.GetGroupMember(Id, Group.Id);
                            if (GroupMember == null)
                            {
                                continue;
                            }

                            if (!Members.Contains(GroupMember))
                            {
                                Members.Add(GroupMember);
                            }
                        }
                        break;
                    }

                case 2:
                    {
                        ConcurrentDictionary<int, string> RequestIds = Group.GetRequests;
                        foreach (int Id in RequestIds.Keys)
                        {
                            GroupMember GroupMember = Group.GetGroupMember(Id, Group.Id);
                            if (GroupMember == null)
                            {
                                continue;
                            }

                            if (!Members.Contains(GroupMember))
                            {
                                Members.Add(GroupMember);
                            }
                        }
                        break;
                    }
            }

            if (!string.IsNullOrEmpty(SearchVal))
            {
                Members = Members.Where(x => x.Username.StartsWith(SearchVal)).ToList();
            }

            int StartIndex = ((Page - 1) * 14 + 14);
            int FinishIndex = Members.Count;

            Session.SendMessage(new GroupMembersComposer(Group, Members.Skip(StartIndex).Take(FinishIndex - StartIndex).ToList(), Members.Count, Page, (Group.CreatorId == Session.GetHabbo().Id || Group.IsAdmin(Session.GetHabbo().Id)), RequestType, SearchVal));
        }
    }
}