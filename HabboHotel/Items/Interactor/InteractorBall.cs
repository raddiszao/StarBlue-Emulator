using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Rooms.Games.Football;
using System.Threading.Tasks;

namespace StarBlue.HabboHotel.Items.Interactor
{
    public class InteractorBall : IFurniInteractor
    {
        public void BeforeRoomLoad(Item Item)
        {

        }

        public void OnPlace(GameClient Session, Item Item)
        {

        }

        public void OnRemove(GameClient Session, Item Item)
        {

        }

        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            Soccer Soccer = new Soccer(Session.GetHabbo().CurrentRoom);
            RoomUser User = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            int NewX = 0;
            int NewY = 0;
            int differenceX = User.X - Item.GetX;
            int differenceY = User.Y - Item.GetY;
            int Chute = 1;
            if (User.SamePath)
                Chute = 3;

            if (differenceX <= 1 && differenceX >= -1 && differenceY <= 1 && differenceY >= -1)
            {
                if (!User.SamePath)
                {
                    if (User.RotBody == 0)
                    {
                        NewX = Item.GetX;
                        NewY = Item.GetY - 1;
                    }
                    if (User.RotBody == 1)
                    {
                        NewX = Item.GetX + 1;
                        NewY = Item.GetY - 1;
                    }
                    if (User.RotBody == 2)
                    {
                        NewX = Item.GetX + 1;
                        NewY = Item.GetY;
                    }
                    if (User.RotBody == 3)
                    {
                        NewX = Item.GetX + 1;
                        NewY = Item.GetY + 1;
                    }
                    if (User.RotBody == 4)
                    {
                        NewX = Item.GetX;
                        NewY = Item.GetY + 1;
                    }
                    if (User.RotBody == 5)
                    {
                        NewX = Item.GetX - 1;
                        NewY = Item.GetY + 1;
                    }
                    if (User.RotBody == 6)
                    {
                        NewX = Item.GetX - 1;
                        NewY = Item.GetY;
                    }
                    if (User.RotBody == 7)
                    {
                        NewX = Item.GetX - 1;
                        NewY = Item.GetY - 1;
                    }

                    if (Item.GetRoom().GetGameMap().ValidTile(NewX, NewY))
                    {
                        Soccer.MoveBall(Item, NewX, NewY, User);
                    }
                }
                else
                {
                    Task t1 = Task.Factory.StartNew(() => TaskMoveBall(User, Item, Chute, Soccer));
                }
            }

        }
        public void TaskMoveBall(RoomUser User, Item item, int Chute, Soccer Soccer)
        {
            int NewX = 0;
            int NewY = 0;
            int i = 0;
            item.ballstop = false;
            item.ejex = false;
            item.ejey = false;
            item.BallDireccion = User.RotBody;
            while (i < Chute && !item.ballstop)
            {
                if (item.BallDireccion == 4)
                {
                    NewX = item.GetX;
                    NewY = item.GetY + 1;

                }
                else if (item.BallDireccion == 6)
                {
                    NewX = item.GetX - 1;
                    NewY = item.GetY;

                }
                else if (item.BallDireccion == 0)
                {
                    NewX = item.GetX;
                    NewY = item.GetY - 1;

                }
                else if (item.BallDireccion == 2)
                {
                    NewX = item.GetX + 1;
                    NewY = item.GetY;

                }
                else if (item.BallDireccion == 1)
                {
                    NewX = item.GetX + 1;
                    NewY = item.GetY - 1;

                }
                else if (item.BallDireccion == 7)
                {
                    NewX = item.GetX - 1;
                    NewY = item.GetY - 1;

                }
                else if (item.BallDireccion == 3)
                {
                    NewX = item.GetX + 1;
                    NewY = item.GetY + 1;

                }
                else if (item.BallDireccion == 5)
                {
                    NewX = item.GetX - 1;
                    NewY = item.GetY + 1;
                }

                if (item.GetRoom().GetGameMap().ValidTile(NewX, NewY))
                {
                    Soccer.MoveBall(item, NewX, NewY, User, false, false);
                }

                i++;
                Task t = Task.Factory.StartNew(() => Soccer.TaskWaiting());
                t.Wait();
            }
        }

        public void OnWiredTrigger(Item Item)
        {

        }
    }
}