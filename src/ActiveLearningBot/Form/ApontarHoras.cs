using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ActiveLearningBot.Form
{
    [Template(TemplateUsage.NotUnderstood, "Desculpe, \"**{0}**\" não é aceitável. Favor, escolha uma opção válida (◑‿◐).")]
    public class ApontarHoras
    {
        [Prompt("Qual o {&}? {||}")]
        [Describe("nome do projeto")]
        private string ProjectName { get; set; }



        public static IForm<ApontarHoras> BuildForm()
        {
            var form = new FormBuilder<ApontarHoras>();

            form.Configuration.DefaultPrompt.ChoiceStyle = ChoiceStyleOptions.Buttons;
            form.Configuration.Yes = new string[] { "sim", "yes", "s", "y", "yep", "é", "ye", "eh", "claro", "yeap", "quero", "quero sim", "isso", "é isso aí" };
            form.Configuration.No = new string[] { "não", "nao", "no", "not", "n", "nem", "nop", "not", "errado", "mudei de ideia", "nem" };
            //form.Message("Você vai ter que me ajudar (∪ ◡ ∪)")
                //.Field(nameof(JokeType))
                //.Confirm(async (state) =>
                //{
                //    var joke = state.JokeType.GetDescribe();
                //    var jokeMessage = joke.Equals(JokeCategory.DadJokes) || joke.Equals(JokeCategory.SuperHeroes) ? $"piada de **'{joke}'**" : $"piada **'{joke}'**";
                //    return new PromptAttribute($"Então, você quer ouvir uma {jokeMessage}? (◔,◔)");
                //});

            return form.Build();
        }
    }
}