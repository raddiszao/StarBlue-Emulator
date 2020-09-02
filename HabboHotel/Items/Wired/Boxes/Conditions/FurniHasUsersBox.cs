using StarBlue.Communication.Packets.Incoming;
using StarBlue.HabboHotel.Rooms;
using System.Collections.Concurrent;
using System.Drawing;
using System.Linq;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Conditions
{
    internal class FurniHasUsersBox : IWiredItem
    {
        public Room Instance { get; set; }

        public Item Item { get; set; }

        public WiredBoxType Type => WiredBoxType.ConditionFurniHasUsers;

        public ConcurrentDictionary<int, Item> SetItems { get; set; }

        public string StringData { get; set; }

        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public FurniHasUsersBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            string Unknown2 = Packet.PopString();

            if (SetItems.Count > 0)
            {
                SetItems.Clear();
            }

            int FurniCount = Packet.PopInt();
            for (int i = 0; i < FurniCount; i++)
            {
                Item SelectedItem = Instance.GetRoomItemHandler().GetItem(Packet.PopInt());
                if (SelectedItem != null)
                {
                    SetItems.TryAdd(SelectedItem.Id, SelectedItem);
                }
            }
        }

        public bool Execute(params object[] Params)
        {
            bool flag = false;
            foreach (Item Item in SetItems.Values.ToList())
            {
                if (Item == null || !Instance.GetRoomItemHandler().GetFloor.Contains(Item))
                {
                    continue;
                }

                foreach (Point coord in Item.GetCoords) if (Instance.GetGameMap().GetRoomUsers(coord).Count != 0) flag = true;

                if (!flag) return false;

                flag = false;
            }
            return true;
        }
    }
}