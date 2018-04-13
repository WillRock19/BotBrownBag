using ActiveLearningBot.Form;
using ActiveLearningBot.Services;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace ActiveLearningBot.Dialogs
{
    [Serializable]
    public class MainDialog : BaseLuisDialog<object>
    {
        string qnaSubscriptionKey = ConfigurationManager.AppSettings["QnaSubscriptionKey"];
        string qnaKnowledgebaseId = ConfigurationManager.AppSettings["QnaKnowledgebaseId"];

        [NonSerialized]
        private LuisLearnService luisLearnService;

        [NonSerialized]
        private AnnouncerService announcerService;

        [NonSerialized]
        private WhaIDoService whatIDo;

        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await NotUnderstandMessage(context, result);
        }

        [LuisIntent("Saudacao")]
        public async Task Saudacao(IDialogContext context, LuisResult result)
        {
            LearnLatestMessageSended(result.TopScoringIntent.Intent, context);

            var qnaService = new QnAMakerService(new QnAMakerAttribute(qnaSubscriptionKey, qnaKnowledgebaseId, "Buguei aqui, pera!  ¯＼(º_o)/¯"));
            var qnaMaker = new QnAMakerDialog(qnaService);
            await qnaMaker.MessageReceivedAsync(context, Awaitable.FromItem(AnnouncerService.Argument));
        }

        [LuisIntent("Adeus")]
        public async Task Adeus(IDialogContext context, LuisResult result)
        {
            LearnLatestMessageSended(result.TopScoringIntent.Intent, context);

            announcerService = new AnnouncerService();
            var card = announcerService.GenerateGoodbye();
            var message = context.MakeMessage();
            message.Attachments.Add(card.ToAttachment());

            await context.PostAsync(message);
        }


        [LuisIntent("Requisitar.Informacoes.Bot")]
        public async Task UsuarioQuerSaberOQueBotFaz(IDialogContext context, LuisResult result)
        {
            LearnLatestMessageSended(result.TopScoringIntent.Intent, context);

            PromptDialog.Confirm(
                context: context,
                resume: ChecarSeUsuarioQuerSaber,
                prompt: "Quer mesmo saber...?",
                attempts: 0,
                promptStyle: PromptStyle.Auto
            );
        }

        private static async Task NotUnderstandMessage(IDialogContext context, LuisResult result)
        {
            context.UserData.SetValue("MessageId", result.Query);
            var card = new HeroCard
            {
                Title = "Desculpe...",
                Text = "Ainda estou aprendendo, qual dessas opções representa o que você deseja?",
                Buttons = GetCardActions()
            };

            var activity = context.MakeMessage();
            activity.Id = new Random().Next().ToString();
            activity.Attachments.Add(card.ToAttachment());

            await context.PostAsync(activity);
        }

        private static List<CardAction> GetCardActions()
        {
            return new List<CardAction>
            {
                new CardAction
                {
                    Title = "Calma Fera, só to te cumprimentando",
                    Type = ActionTypes.ImBack,
                    Value = "Oi"
                },
                new CardAction
                {
                    Title = "Quero saber o que tu faz",
                    Type = ActionTypes.ImBack,
                    Value = "Oq vc faz?"
                },
                new CardAction
                {
                    Title = "Só te dei tchau parça!",
                    Type = ActionTypes.ImBack,
                    Value = "tchau"
                },
            };
        }

        private async Task SendMessageCongratulations(IDialogContext context)
        {
            announcerService = new AnnouncerService();
            var card = announcerService.GenerateCongratulations().ToAttachment();
            var msg = context.MakeMessage();
            msg.Attachments.Add(card);
            await context.PostAsync(msg);
        }


        private async Task ChecarSeUsuarioQuerSaber(IDialogContext context, IAwaitable<bool> result)
        {
            try
            {
                var querSaber = await result;

                if (querSaber)
                {
                    await SendCrazyImages(context);
                }
                else
                {
                    await context.PostAsync("Ok, então. Se quiser, pergunta outra vez.");
                    context.Done(string.Empty);
                }
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync("Não posso aceitar essa resposta u.u");
                await context.PostAsync("Me pergunta de novo quando quiser mesmo saber xP");
                context.Done(string.Empty);
            }
        }

        private async Task SendCrazyImages(IDialogContext context)
        {
            whatIDo = new WhaIDoService();
            await whatIDo.SendWhatIDoMessages(context);

            PromptDialog.Confirm(
                context: context,
                resume: ChecarSeUsuarioQuerApontarHoras,
                prompt: "Bora?",
                attempts: 1,
                promptStyle: PromptStyle.Auto
            );
        }

        private async Task ChecarSeUsuarioQuerApontarHoras(IDialogContext context, IAwaitable<bool> result)
        {
            try
            {
                var querSaber = await result;

                if (querSaber)
                {
                    var formDialog = new FormDialog<ApontarHoras>(new ApontarHoras(), ApontarHoras.BuildForm, FormOptions.PromptInStart);
                    context.Call(formDialog, ExecuteAfterForm); ;
                }
                else
                {
                    await context.PostAsync("Ok, então. Se quiser, pergunta outra vez.");
                    context.Done(string.Empty);
                }
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync("Não posso aceitar essa resposta u.u");
                await context.PostAsync("Me pergunta de novo quando quiser mesmo saber xP");
                context.Done(string.Empty);
            }
        }

        private async Task ExecuteAfterForm(IDialogContext context, IAwaitable<ApontarHoras> result)
        {
            var message = context.MakeMessage();

            announcerService = new AnnouncerService();
            message.Attachments.Add(announcerService.GenerateCongratulations().ToAttachment());

            await context.PostAsync(message);
        }

        private void LearnLatestMessageSended(string intent, IDialogContext dialogContext)
        {
            luisLearnService = new LuisLearnService();

            HostingEnvironment.QueueBackgroundWorkItem(workItem =>
                luisLearnService?.LearnLatestMessageSended(intent, dialogContext));
        }


    }
}