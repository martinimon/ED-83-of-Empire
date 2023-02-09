using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdgeOfEmpireBot.IService
{
    public interface IMessageService
    {
        /// <summary>
        /// Sends a message to Discord.
        /// </summary>
        Task SendMessage(string messageResponse, ISocketMessageChannel messageResponseChannel);

        /// <summary>
        /// Processes a message. 
        /// </summary>
        /// <param name="message"></param>
        Task ProcessMessage(SocketMessage message);

        /// <summary>
        /// A bool to indicate if the message is safe.
        /// </summary>
        /// <returns>bool</returns>
        bool MessageIsSafe();
    }
}
