using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;

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
        Task ProcessMessage(SocketMessage message);

        /// <summary>
        /// A bool to indicate if the message is safe.
        /// </summary>
        /// <returns>bool</returns>
        bool MessageIsSafe();
    }
}
