namespace LibraProgramming.Communication.Protocol.Packets
{
    public abstract class Packet
    {
         public abstract LibraTalkCommand Command
         {
             get;
         }
    }
}