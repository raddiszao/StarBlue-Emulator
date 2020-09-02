using StarBlue.Communication.Packets.Incoming;
using StarBlue.HabboHotel.Rooms;
using System;
using System.Collections.Concurrent;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Conditions
{
    internal class DateRangeIsActiveBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type => WiredBoxType.ConditionDateRangeActive;
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }
        public int StartDate { get; set; }
        public int EndDate { get; set; }

        public DateRangeIsActiveBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;

            SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            int Date1 = Packet.PopInt();
            int Date2 = Packet.PopInt();

            StringData = Convert.ToString(Date1 + ";" + Date2);
            StartDate = Date1;
            EndDate = Date2;

        }

        public bool Execute(params object[] Params)
        {
            if (Params.Length == 0 || Instance == null || string.IsNullOrEmpty(StringData))
            {
                return false;
            }

            int TimeStamp = StarBlueServer.GetIUnixTimestamp();
            if (TimeStamp < StartDate || TimeStamp > EndDate)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}