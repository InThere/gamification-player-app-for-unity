using System;
using GamificationPlayer.Session;

namespace GamificationPlayer.DTO.ExternalEvents
{
    public class FitnessContentOpenedDTO
    {
        [Serializable]
        public class Data : ILoggableData
        {
            public string Type { get => type; }
            public float Time { get; set; }

            public string type;

            public Attributes attributes;

            public Data()
            {
                attributes = new Attributes();
            }
        }

        [Serializable]
        public class Attributes
        {  
            [FitnessContentIdentifier]
            public string identifier;
        }

        public Data data;
        
        public FitnessContentOpenedDTO()
        {
            data = new Data();
        }
    }
}
