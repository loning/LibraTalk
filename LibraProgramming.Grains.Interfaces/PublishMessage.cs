using System;

namespace LibraProgramming.Grains.Interfaces
{
    [Serializable]
    public class PublishMessage
    {
         public string Text
         {
             get;
             set;
         }
    }
}