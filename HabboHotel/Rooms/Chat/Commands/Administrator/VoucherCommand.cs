using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.HabboHotel.GameClients;
using System;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Administrator
{
    internal class VoucherCommand : IChatCommand
    {
        public string PermissionRequired => "user_16";

        public string Parameters => "[MENSAGEM]";

        public string Description => "Envia uma mensagem a todos os Staffs online.";

        public void Execute(GameClient Session, Rooms.Room Room, string[] Params)
        {
            #region Parametros
            string type = Params[1];
            int value = int.Parse(Params[2]);
            int uses = int.Parse(Params[3]);
            #endregion

            int Voucher = 10;
            string _CaracteresPermitidos = "abcdefghijklmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789!@$?";
            byte[] randomBytes = new byte[Voucher];
            char[] Caracter = new char[Voucher];
            int CuentaPermitida = _CaracteresPermitidos.Length;

            for (int i = 0; i < Voucher; i++)
            {
                Random randomObj = new Random();
                randomObj.NextBytes(randomBytes);
                Caracter[i] = _CaracteresPermitidos[randomBytes[i] % CuentaPermitida];
            }

            string code = new string(Caracter);

            StarBlueServer.GetGame().GetCatalog().GetVoucherManager().AddVoucher(code, type, value, uses);

            StarBlueServer.GetGame().GetClientManager().SendMessage(new RoomCustomizedAlertComposer("AVISO: Um novo voucher foi divulgado, para pega-lo, vá ao catálogo, na aba 'Inicio' na parte inferior, no quadrado, insira: \n\n" +
                "Código: " + code + "\nA recompensa é: " + type + "\n Pode ser usado até mesmo em " + uses + " ocasiones\n\n Sorte, resgatá-lo!"));

        }
    }
}
