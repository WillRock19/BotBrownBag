using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace ActiveLearningBot.Services
{
    public class WhaIDoService
    {
        private string FirstKnock;
        private string SecondKnock;
        private string HereIAmKnock;
        

        public WhaIDoService()
        {
            FirstKnock = $"{AppDomain.CurrentDomain.BaseDirectory}{ConfigurationManager.AppSettings["FirstKnockUrl"]}";
            SecondKnock = $"{AppDomain.CurrentDomain.BaseDirectory}{ConfigurationManager.AppSettings["SecondKnockUrl"]}";
            HereIAmKnock = $"{AppDomain.CurrentDomain.BaseDirectory}{ConfigurationManager.AppSettings["HereIAmKnockUrl"]}";
        }

        public async Task SendWhatIDoMessages(IDialogContext context)
        {
            await MakeFirstKnock(context);

            var second = await Task.Delay(2000).ContinueWith(async t =>
            {
                await MakeSecondKnock(context);
            });

            var third = await Task.Delay(4000).ContinueWith(async p =>
            {
                await MakeThirdKnock(context);
            });

            await Task.WhenAll(new[] { second, third });
        }

        private async Task MakeFirstKnock(IDialogContext context)
        {
            var firstKnock = GenerateFirstKnock().ToAttachment();
            var message = context.MakeMessage();
            message.Attachments.Add(firstKnock);

            await context.PostAsync(message);
        }

        private async Task MakeSecondKnock(IDialogContext context)
        {
            var firstKnock = GenerateSecondtKnock().ToAttachment();
            var message = context.MakeMessage();
            message.Attachments.Add(firstKnock);

            await context.PostAsync(message);
        }

        private async Task MakeThirdKnock(IDialogContext context)
        {
            var firstKnock = GenerateHereIAm().ToAttachment();
            var message = context.MakeMessage();
            message.Attachments.Add(firstKnock);

            await context.PostAsync(message);
        }

        private HeroCard GenerateFirstKnock() => new HeroCard()
        {
            Title = "...",
            //Text = "..",
            Images = new List<CardImage>()
            {
                new CardImage(FirstKnock)
            },
        };

        private HeroCard GenerateSecondtKnock() => new HeroCard()
        {
            Title = "...",
            //Text = "..",
            Images = new List<CardImage>()
            {
                new CardImage(SecondKnock)
            },
        };

        private HeroCard GenerateHereIAm() => new HeroCard()
        {
            Title = " eu sou BOT PAUL",
            Subtitle = "Já apontou suas horas hoje? 🕵",
            Text = "Ta esperando o que? Vamos apontar as horas, vamos la já que você não aponta de maneira nenhuma fui criado para te ajudar a apontar essas benditas horas!!!!",
            Images = new List<CardImage>()
            {
                new CardImage(HereIAmKnock)
            },
        };

    }
}