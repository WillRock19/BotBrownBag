using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ActiveLearningBot.Form
{
    [Serializable]
    [Template(TemplateUsage.NotUnderstood, "Desculpe, \"**{0}**\" não é aceitável. Favor, escolha uma opção válida (◑‿◐).")]
    public class ApontarHoras
    {
        [Prompt("Qual o {&}? {||}")]
        [Describe("Nome do Projeto")]
        public string ProjectName { get; set; }

        [Prompt("Qual o {&}? {||}")]
        [Describe("Dia")]
        public DateTime Day { get; set; }

        [Prompt("O que {&}? {||}")]
        [Describe("você fez")]
        public string TaskExecuted { get; set; }

        [Prompt("Quantas {&}? {||}")]
        [Describe("Horas")]
        public int Hours { get; set; }

        public static IForm<ApontarHoras> BuildForm()
        {
            var form = new FormBuilder<ApontarHoras>();
            form.Configuration.DefaultPrompt.ChoiceStyle = ChoiceStyleOptions.Buttons;
            form.Configuration.Yes = new string[] { "sim", "yes", "s", "y", "yep", "é", "ye", "eh", "claro", "yeap", "quero", "quero sim", "isso", "é isso aí", "uhum" };
            form.Configuration.No = new string[] { "não", "nao", "no", "not", "n", "nem", "nop", "not", "errado", "mudei de ideia", "nem" };
            return form.Build();
        }
    }
}