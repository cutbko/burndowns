using GalaSoft.MvvmLight.Messaging;

namespace MoneyBurnDown.Model.Messages
{
    public class TypeCreatedMessage : MessageBase
    {
        public string Name { get; set; }
    }
}