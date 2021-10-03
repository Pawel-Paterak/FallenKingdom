using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KingdomData
{
    public static class PacketsConnection
    {
        public enum PacketServer {Information = 1, Visible, Game, Invisible, Veryfication, Disconect = 100};
        public enum PacketClient {Information = 1, Visible, Game, Veryfication, Disconect = 100 };

        #region "Inputs Server"
        public enum InputServerInformation {  };
        public enum InputServerVisible {login, Register, CreateCharacter, JoinToGame};
        public enum InputServerGame {MovePlayer, AtackPlayer, Chat, MoveSlots, InteractionFromServer};
        //public enum InputServerInvisible {};
        #endregion

        #region "Inputs Client"
        public enum InputClientInformation {Error, Login, Register, CreateCharacter};
        public enum InputClientVisible { Login, Register, AddCharacter, JoinToGame, ExitTheGame};
        public enum InputClientGame {MovePlayer, ChangeField, Chat, PlayersAssets, AddInventory, RemoveInventory, MoveSlots, NewEngine};
        public enum InputClientChangeField {Health, Mana};
        #endregion
    }
}
