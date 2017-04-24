using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackLibrary
{
    /// <summary>
    /// Esta clase se envía como parámetro en el evento cuando arriba un mensaje al cliente. 
    /// Contiene la carta jugada y el mensaje del servidor.
    /// </summary>
    public class GameMessageEventArgs : EventArgs
    {
        private GameMessage _gm;

        public GameMessage GM
        {
            get { return _gm; }
            set { _gm = value; }
        }

        public GameMessageEventArgs(GameMessage gm)
        {
            this._gm = gm;
        }
    }
    public delegate void MessageReceivedEventHandler(object sender, GameMessageEventArgs e);
}
