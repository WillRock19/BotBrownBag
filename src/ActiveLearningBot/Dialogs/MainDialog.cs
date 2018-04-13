using ActiveLearningBot.Form;
using ActiveLearningBot.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace ActiveLearningBot.Dialogs
{
    [Serializable]
    public class MainDialog : BaseLuisDialog<object>
    {
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

            await context.PostAsync($"Bem-vindo(a) a Lambda3! Sou o seu assistente virtual, ja apontou suas horas hoje? 🕵");
            context.Wait(MessageReceived);

            await SendMessageCongratulations(context);
        }


        [LuisIntent("Requisitar.Informacoes.Bot")]
        public async Task UsuarioQuerSaberOQueBotFaz(IDialogContext context, LuisResult result)
        {
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
                //new CardAction
                //{
                //    Title = "Desejo apontar as horas",
                //    Type = ActionTypes.ImBack,
                //    Value = "Desejo apontar as horas"
                //},
                new CardAction
                {
                    Title = "Oi",
                    Type = ActionTypes.ImBack,
                    Value = "Oi"
                },
                new CardAction
                {
                    Title = "Nenhuma",
                    Type = ActionTypes.ImBack,
                    Value = "Nenhuma"
                }
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
                prompt: "",
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