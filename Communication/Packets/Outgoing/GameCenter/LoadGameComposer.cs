namespace StarBlue.Communication.Packets.Outgoing.GameCenter
{
    internal class LoadGameComposer : MessageComposer
    {
        private string SSOTicket { get; }

        public LoadGameComposer(string SSOTicket)
            : base(Composers.LoadGameMessageComposer)
        {
            this.SSOTicket = SSOTicket;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(1);
            packet.WriteString("1365260055982");
            packet.WriteString("https://www.heibbo.com/game/games/gamecenter_basejump/BaseJump.swf");
            packet.WriteString("best");
            packet.WriteString("showAll");
            packet.WriteInteger(60);//FPS?   
            packet.WriteInteger(10);
            packet.WriteInteger(8);
            packet.WriteInteger(6);//Asset count
            packet.WriteString("assetUrl");
            packet.WriteString("https://www.heibbo.com/game/games/gamecenter_basejump/BasicAssets.swf");
            packet.WriteString("habboHost");
            packet.WriteString("http://fuseus-private-httpd-fe-1");
            packet.WriteString("accessToken");
            packet.WriteString(SSOTicket);
            packet.WriteString("gameServerHost");
            packet.WriteString("37.59.173.22");
            packet.WriteString("gameServerPort");
            packet.WriteString("3030");
            packet.WriteString("socketPolicyPort");
            packet.WriteString("37.59.173.22");
        }
    }
}
