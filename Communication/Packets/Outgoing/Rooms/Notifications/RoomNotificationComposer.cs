using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Notifications
{
    internal class RoomNotificationComposer : MessageComposer
    {
        public string Type { get; }
        public string Key { get; }
        public string Value { get; }

        public string Title { get; }
        public string Message { get; }
        public string Image { get; }
        public string HotelName { get; }
        public string HotelUrl { get; }
        public Dictionary<string, string> Keys { get; }
        public bool isBubble { get; }

        public RoomNotificationComposer(string Type)
            : base(Composers.RoomNotificationMessageComposer)
        {
            this.Type = Type;
        }

        public RoomNotificationComposer(string Title, string Message, string Image, string LinkTitle = "", string LinkUrl = "")
            : base(Composers.RoomNotificationMessageComposer)
        {
            this.Title = Title;
            this.Message = Message;
            this.Image = Image;
            this.HotelName = LinkTitle;
            this.HotelUrl = LinkUrl;
        }

        public RoomNotificationComposer(string Type, Dictionary<string, string> Keys) : base(Composers.RoomNotificationMessageComposer)
        {
            this.Type = Type;
            this.Keys = Keys;
        }

        public RoomNotificationComposer(string image, string message, bool isBubble, string linkUrl = "")
            : base(Composers.RoomNotificationMessageComposer)
        {
            this.Image = image;
            this.Message = message;
            this.HotelUrl = linkUrl;
            this.isBubble = isBubble;
        }

        public override void Compose(Composer packet)
        {
            if (!string.IsNullOrEmpty(Message) && !isBubble)
            {
                packet.WriteString(Image);
                packet.WriteInteger(string.IsNullOrEmpty(HotelName) ? 2 : 4);
                packet.WriteString("title");
                packet.WriteString(Title);
                packet.WriteString("message");
                packet.WriteString(Message);

                if (!string.IsNullOrEmpty(HotelName))
                {
                    packet.WriteString("linkUrl");
                    packet.WriteString(HotelUrl);
                    packet.WriteString("linkTitle");
                    packet.WriteString(HotelName);
                }
            }
            else if (!string.IsNullOrEmpty(Key))
            {
                packet.WriteString(Type);
                packet.WriteInteger(1);//Count
                {
                    packet.WriteString(Key);//Type of message
                    packet.WriteString(Value);
                }
            }
            else if (Keys != null)
            {
                packet.WriteString(Type);
                packet.WriteInteger(Keys.Count);
                foreach (KeyValuePair<string, string> current in Keys)
                {
                    packet.WriteString(current.Key);
                    packet.WriteString(current.Value);
                }
            }
            else if (isBubble)
            {
                packet.WriteString(Image);
                packet.WriteInteger(string.IsNullOrEmpty(HotelUrl) ? 2 : 3);
                packet.WriteString("display");
                packet.WriteString("BUBBLE");
                packet.WriteString("message");
                packet.WriteString(Message);
                if (!string.IsNullOrEmpty(HotelUrl))
                {
                    packet.WriteString("linkUrl");
                    packet.WriteString(HotelUrl);
                }
            }
            else
            {
                packet.WriteString(Type);
                packet.WriteInteger(0);//Count
            }
        }

        public static MessageComposer SendBubble(string image, string message, string linkUrl = "")
        {
            return new RoomNotificationComposer(image, message, true, linkUrl);
        }
    }
}
