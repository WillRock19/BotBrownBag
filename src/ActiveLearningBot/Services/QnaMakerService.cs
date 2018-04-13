using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;


namespace ActiveLearningBot.Services
{
    public class QnaMakerService
    {
        private string qnaKnowledgeBaseId { get; set; }
        private string qnaSubscriptionKey { get; set; }
        private string defaultNotFindMessage { get; set; }
        private double precisionScore { get; set; }
        //private QnAMakerService service { get; set; }

    }
}