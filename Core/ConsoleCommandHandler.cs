using log4net;
using StarBlue.Communication.Packets.Outgoing.Moderation;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace StarBlue.Core
{
    public class ConsoleCommandHandler
    {
        private static readonly ILog log = LogManager.GetLogger("StarBlue.Core.ConsoleCommandHandler");

        public static void InvokeCommand(string inputData)
        {
            if (string.IsNullOrEmpty(inputData))
            {
                return;
            }

            try
            {
                #region Command parsing
                string[] parameters = inputData.Split(' ');

                switch (parameters[0].ToLower())
                {
                    #region stop
                    case "shutdown":
                        {
                            string time = "1";
                            if (parameters.Length > 1)
                            {
                                time = parameters[1];
                            }

                            int total_time = int.Parse(time) * 60 * 1000;
                            Logging.WriteLine("O servidor irá fechar em " + time + " minutos.", ConsoleColor.Yellow);
                            StarBlueServer.GetGame().GetClientManager().SendMessage(new BroadcastMessageAlertComposer("O " + StarBlueServer.HotelName + " Hotel irá reinicializar em " + time + " minutos.\r\n" + "- Equipe " + StarBlueServer.HotelName));
                            StarBlueServer.GetConfig().data["going.is.to.be.close"] = "true";
                            Task t = Task.Factory.StartNew(() => ShutdownIn(total_time));
                            break;
                        }
                    #endregion

                    #region restartt
                    case "restart":
                    case "reiniciar":
                        {
                            StarBlueServer.GetGame().GetClientManager().SendMessage(new RoomCustomizedAlertComposer(StarBlueServer.HotelName + " fará um reinício rápido, para aplicar todas as atualizações.\n\nVoltaremos em seguida :)"));
                            StarBlueServer.PerformRestart();
                            break;
                        }
                    #endregion

                    #region openhotel
                    case "openhotel":
                        {
                            StarBlueServer.GetConfig().data["hotel.open.for.users"] = "true";
                            break;
                        }
                    #endregion

                    #region alert
                    case "alert":
                        {
                            string Notice = inputData.Substring(6);

                            StarBlueServer.GetGame().GetClientManager().SendMessage(new BroadcastMessageAlertComposer(StarBlueServer.GetLanguageManager().TryGetValue("console.noticefromadmin") + "\n\n" + Notice));

                            log.Info(">> [SEND] Alerta enviado com sucesso.");
                            break;
                        }
                    #endregion

                    default:
                        {
                            log.Error(parameters[0].ToLower() + "? Não foi encontrado esse comando.");
                            break;
                        }
                }
                #endregion
            }
            catch (Exception e)
            {
                log.Error("Erro no comando [" + inputData + "]: " + e);
            }
        }

        public static void ShutdownIn(int time)
        {
            Thread.Sleep(time);

            Logging.DisablePrimaryWriting(true);
            Logging.WriteLine("The server is saving users furniture, rooms, etc. WAIT FOR THE SERVER TO CLOSE, DO NOT EXIT THE PROCESS IN TASK MANAGER!!", ConsoleColor.Yellow);
            StarBlueServer.PerformShutDown();
        }
    }
}