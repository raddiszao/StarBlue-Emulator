using StarBlue.Communication.Packets.Incoming;
using StarBlue.HabboHotel.Items.Data.Moodlight;
using StarBlue.HabboHotel.Rooms;
using System.Collections.Concurrent;
using System.Linq;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Effects
{
    internal class ActionDimmerBox : IWiredItem, IWiredCycle
    {
        public Room Instance { get; set; }

        public Item Item { get; set; }

        public WiredBoxType Type => WiredBoxType.EffectActionDimmer;

        public ConcurrentDictionary<int, Item> SetItems { get; set; }

        public string StringData { get; set; }

        public bool BoolData { get; set; }

        public string ItemsData { get; set; }

        public int Delay { get; set; } = 0 * 500;

        public int TickCount { get; set; }

        private long _next;
        private int counter = 0;
        private bool Requested = false;

        public ActionDimmerBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(MessageEvent Packet)
        {
            int Unknown = Packet.PopInt();
            string Type = Packet.PopString();
            int Unknow2 = Packet.PopInt();
            Delay = Packet.PopInt() * 500;
            counter = 0;
            TickCount = 0;
            StringData = Type;
        }

        public bool Execute(params object[] Params)
        {
            if (_next == 0 || _next < StarBlueServer.Now())
            {
                _next = StarBlueServer.Now() + Delay;
            }

            if (!Requested)
            {
                counter = 0;
                Requested = true;
            }

            return true;
        }

        public bool OnCycle()
        {
            if (Instance == null || !Requested || _next == 0)
            {
                return false;
            }

            counter += 500;
            if (counter > Delay)
            {
                counter = 0;
                if (Instance.MoodlightData == null)
                {
                    foreach (Item item in Instance.GetRoomItemHandler().GetWall.ToList())
                    {
                        if (item.GetBaseItem().InteractionType == InteractionType.MOODLIGHT)
                        {
                            Instance.MoodlightData = new MoodlightData(item.Id);
                        }
                    }
                }

                if (Instance.MoodlightData == null)
                    return false;

                if (StringData.Equals("on"))
                {
                    if (!Instance.MoodlightData.Enabled)
                    {
                        Instance.MoodlightData.Enable();
                    }
                }
                else if (StringData.Equals("off"))
                {
                    if (Instance.MoodlightData.Enabled)
                    {
                        Instance.MoodlightData.Disable();
                    }
                }
                else
                {
                    if (Instance.MoodlightData.Enabled)
                    {
                        Instance.MoodlightData.Disable();
                    }
                    else
                    {
                        Instance.MoodlightData.Enable();
                    }
                }

                Item MoodlightItem = Instance.GetRoomItemHandler().GetWall.Where(x => x != null && x.Data.InteractionType == InteractionType.MOODLIGHT).First();
                if (MoodlightItem != null)
                {
                    MoodlightItem.ExtraData = Instance.MoodlightData.GenerateExtraData();
                    MoodlightItem.UpdateState();
                }

                Requested = false;
                _next = 0;
                return true;
            }

            return false;
        }
    }
}