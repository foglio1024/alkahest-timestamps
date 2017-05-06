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

        bool showSeconds = true;
        bool enabled = true;

        public void Start(GameProxy[] proxies)
        {
            foreach (var p in proxies.Select(x => x.Processor))
            {
                p.AddHandler<SChatPacket>(HandleChat);
                p.AddHandler<SPrivateChatPacket>(HandlePrivateChat);
                p.AddHandler<CChatPacket>(HandleCommand);
                //p.AddHandler<SWhisperPacket>(HandleWhisper);
            }
        }
        public void Stop(GameProxy[] proxies)
        {
            foreach (var p in proxies.Select(x => x.Processor))
            {
                p.RemoveHandler<SChatPacket>(HandleChat);
                p.RemoveHandler<SPrivateChatPacket>(HandlePrivateChat);
                p.RemoveHandler<CChatPacket>(HandleCommand);
                //p.AddHandler<SWhisperPacket>(HandleWhisper);

            }
        }

        private bool HandleWhisper(GameClient client, Direction direction, SWhisperPacket packet)
        {
            if (!enabled) return true;
            string format = (showSeconds) ? "T" : "t";
            packet.Message = String.Format("[{0}] {1}", DateTime.Now.ToString(format), packet.Message);
            client.SendToClient(packet);
            return false;
        }

        private bool HandleCommand(GameClient client, Direction direction, CChatPacket packet)
        {
            var c = packet.Message.Replace("<FONT>", "");
            c = c.Replace("</FONT>", "");
            if (c.StartsWith(".ts "))
            {
                var arg0 = c.Substring(4);
                if (arg0.Equals("on"))
                {
                    enabled = true;
                    client.SendToClient(new SWhisperPacket() { SenderName = "timestamps-plugin", RecipientName="user", Message = "Timestamps on" });
                    _log.Basic("Timestamps on");
                }
                else if (arg0.Equals("off"))
                {
                    enabled = false;
                    client.SendToClient(new SWhisperPacket() { SenderName = "timestamps-plugin", RecipientName="user", Message = "Timestamps off" });
                    _log.Basic("Timestamps off");
                }
                else if (arg0.StartsWith("sec"))
                {
                    var arg1 = arg0.Substring(4);
                    if (arg1.Equals("on"))
                    {
                        showSeconds = true;
                        client.SendToClient(new SWhisperPacket() { SenderName = "timestamps-plugin", RecipientName="user", Message = "Seconds on" });
                        _log.Basic("Seconds on");


                    }
                    else if (arg1.Equals("off"))
                    {
                        showSeconds = false;
                        client.SendToClient(new SWhisperPacket() { SenderName = "timestamps-plugin", RecipientName = "user", Message = "Seconds off" });
                        _log.Basic("Seconds off");

                    }
                }
                else
                {

                }
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool HandleChat(GameClient client, Direction direction, SChatPacket packet)
        {
            if (packet.Channel == Core.Game.ChatChannel.Emote || !enabled) return true;
            string format = (showSeconds) ? "T" : "t";
            packet.SenderName = String.Format("</a>{0}][<a href='asfunction:chatNameAction,{1}@0@0'>{1}</a>", DateTime.Now.ToString(format), packet.SenderName);
            client.SendToClient(packet);
            return false;
        }

        private bool HandlePrivateChat(GameClient client, Direction direction, SPrivateChatPacket packet)
        {
            if (!enabled) return true;
            string format = (showSeconds) ? "T" : "t";
            packet.SenderName = String.Format("</a>{0}][<a href='asfunction:chatNameAction,{1}@0@0'>{1}</a>", DateTime.Now.ToString(format), packet.SenderName);
            client.SendToClient(packet);
            return false;
        }

    }
}
