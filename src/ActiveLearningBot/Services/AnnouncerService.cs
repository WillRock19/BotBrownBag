using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace ActiveLearningBot.Services
{
    [Serializable]
    public class AnnouncerService
    {
        private string botImageUrl;
        private string botImageUrlHappy;
        private string myNameIs;
        public static IMessageActivity Argument { get; set; }

        public AnnouncerService()
        {
            myNameIs = "Hey, eu sou Paul Bot!";
            botImageUrl = ConfigurationManager.AppSettings["BotImageUrl"];
            botImageUrlHappy = ConfigurationManager.AppSettings["BotImageUrlCongrats"];

        }

        public HeroCard GenerateGoodbye() => new HeroCard()
        {
            Title = "TO DE OLHO",
            Subtitle = "Continue apontando suas horas 🕵",
            Text = "Quando vc estiver na cozinha, eu estarei lá.... Quando vc estiver no metrô, eu estarei lá...... Quando vc estiver no cliente, eu estarei lá",
            Images = new List<CardImage>()
            {
                new CardImage(botImageUrl, "Bot Paul")
            }
        };

        public HeroCard GenerateCongratulations() => new HeroCard()
        {
            Title = "Aeee PARABÉNS!!!",
            Text = "HORAS APONTADAS, AGORA ESTOU FELIZ hehe",
            Images = new List<CardImage>()
            {
                new CardImage(botImageUrlHappy, "eu sou Paul Bot")
            },
        };

    }
}