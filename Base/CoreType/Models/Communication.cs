using CoreType.DBModels;

namespace CoreType.Models
{
    public class Communication
    {
        public CommunicationDefinitions CommunicationDefinitions { get; set; }
        public CommunicationTemplates CommunicationTemplates { get; set; }
    }
}