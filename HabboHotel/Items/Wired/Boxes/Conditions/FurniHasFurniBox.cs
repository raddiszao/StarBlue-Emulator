using StarBlue.Communication.Packets.Incoming;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Rooms.PathFinding;
using System.Collections.Concurrent;
using System.Linq;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Conditions
{
    internal class FurniHasFurniBox : IWiredItem
    {
        public Room Instance { get; set; }

        public Item Item { get; set; }

        public WiredBoxType Type => WiredBoxType.ConditionFurniHasFurni;

        public ConcurrentDictionary<int, Item> SetItems { get; set; }

        public string StringData { get; set; }

        public bool BoolData { get; set; }

        public string ItemsData { get; set; }

        public FurniHasFurniBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(MessageEvent Packet)
        {
            int Unknown = Packet.PopInt();
            int Option = Packet.PopInt();
            string Unknown2 = Packet.PopString();

            BoolData = Option == 1;

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
            if (SetItems.Count == 0)
                return false;

            return BoolData ? AllFurniHaveFurniOn() : SomeFurniHaveFurniOn();
        }

        public bool SomeFurniHaveFurniOn()
        {
            bool HasFurni = false;
            Gamemap map = Instance.GetGameMap();
            foreach (Item Item in SetItems.Values.ToList())
            {
                if (Item == null || !Instance.GetRoomItemHandler().GetFloor.Contains(Item))
                    continue;

                foreach (ThreeDCoord coord in Item.GetAffectedTiles.Values)
                {
                    if (!map.ValidTile(coord.X, coord.Y))
                        return false;

                    if (map.Model.SqFloorHeight[coord.X, coord.Y] + map.ItemHeightMap[coord.X, coord.Y] > Item.TotalHeight)
                        HasFurni = true;
                }
            }

            if (HasFurni)
                return true;

            return false;
        }

        public bool AllFurniHaveFurniOn()
        {
            Gamemap map = Instance.GetGameMap();
            foreach (Item Item in SetItems.Values.ToList())
            {
                if (Item == null || !Instance.GetRoomItemHandler().GetFloor.Contains(Item))
                    continue;

                foreach (ThreeDCoord coord in Item.GetAffectedTiles.Values)
                {
                    if (!map.ValidTile(coord.X, coord.Y))
                        return false;

                    if (map.Model.SqFloorHeight[coord.X, coord.Y] + map.ItemHeightMap[coord.X, coord.Y] > Item.TotalHeight)
                        return true;
                }
            }

            return true;
        }
    }
}