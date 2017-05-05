using Alkahest.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alkahest.Core.Net;
using Alkahest.Core.Net.Protocol.Packets;
using Alkahest.Core;
using Alkahest.Core.Logging;

namespace Alkahest.Plugins.Timestamps
{
    public class TimestampsPlugin : IPlugin
    {
        public string Name => "timestamps";

        static readonly Log _log = new Log(typeof(TimestampsPlugin));

        public void Start(GameProxy[] proxies)
        {
            foreach (var p in proxies.Select(x => x.Processor))
            {
                p.AddHandler<SChatPacket>(HandleChat);
                p.AddHandler<SPrivateChatPacket>(HandlePrivateChat);
                //p.AddHandler<SWhisperPacket>(HandleWhisper);
            }
        }
        public void Stop(GameProxy[] proxies)
        {
            foreach (var p in proxies.Select(x => x.Processor))
            {
                p.RemoveHandler<SChatPacket>(HandleChat);
                p.RemoveHandler<SPrivateChatPacket>(HandlePrivateChat);
                //p.AddHandler<SWhisperPacket>(HandleWhisper);

            }
        }

        private bool HandleWhisper(GameClient client, Direction direction, SWhisperPacket packet)
        {
            packet.SenderName = String.Format("</a>{0}][<a href='asfunction:chatNameAction,{1}@0@0'>{1}</a>", DateTime.Now.ToString("t"), packet.SenderName);
            client.SendToClient(packet);
            return false;
        }


        private bool HandleChat(GameClient client, Direction direction, SChatPacket packet)
        {
            if (packet.Channel == Core.Game.ChatChannel.Emote) return true;
            packet.SenderName = String.Format("</a>{0}][<a href='asfunction:chatNameAction,{1}@0@0'>{1}</a>", DateTime.Now.ToString("t"), packet.SenderName);
            client.SendToClient(packet);
            return false;
        }

        private bool HandlePrivateChat(GameClient client, Direction direction, SPrivateChatPacket packet)
        {
            packet.SenderName = String.Format("</a>{0}][<a href='asfunction:chatNameAction,{1}@0@0'>{1}</a>", DateTime.Now.ToString("t"), packet.SenderName);
            client.SendToClient(packet);
            return false;
        }

    }
}
