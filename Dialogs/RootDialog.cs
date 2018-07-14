﻿using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using SimpleEchoBot.Dialogs;
using SimpleEchoBot.Models;
using SimpleEchoBot.Utils;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private User user = new User();

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            await this.SendWelcomeMessageAsync(context);
        }

        private async Task SendWelcomeMessageAsync(IDialogContext context)
        {
            user = new User();
            //PromptDialog.Choice(context, this.OnOptionSelected, new List<string>() { "Yes", "No" }, "Hi, Do you want to know which car model will suit your life style? ", "Not a valid option", 3);
            PromptDialog.Choice(context, this.OnOptionSelected, new List<string>() { "ใช่", "ไม่" }, "สวัสดีออเจ้า ออเจ้าอยากให้ข้าแนะนำรุ่นรถยนต์ที่เหมาะกับออเจ้าหรือไม่?", "ออเจ้าเลือกไม่ถูกต้อง", 3);
        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = await result;

                switch (optionSelected)
                {
                    //case "Yes":
                    case "ใช่":
                        context.Call(new FaceDialog(this.user), this.FaceDialogResumeAfter);
                        break;

                    //case "No":
                    case "ไม่":
                        //await context.PostAsync($"Oh, I'm sorry to hear that. You can chat to me again anytime.");
                        await context.PostAsync($"ข้าเสียใจยิ่ง แต่ถึงอย่างไรออเจ้าคุยกับข้าได้ทุกเมื่อหนา");
                        context.Wait(this.MessageReceivedAsync);
                        break;
                }
            }
            catch (TooManyAttemptsException ex)
            {
                //await context.PostAsync($"Ooops! Too many attempts :(. But don't worry, I'm handling that exception and you can try again!");
                await context.PostAsync($"ข้าเสียใจยิ่ง ระบบขัดข้อง ลองเริ่มกันใหม่นะเจ้าคะ");
                context.Wait(this.MessageReceivedAsync);
            }
        }

        private async Task FaceDialogResumeAfter(IDialogContext context, IAwaitable<User> result)
        {
            try
            {
                this.user = await result;
                context.Call(new SuggestDialog(this.user), this.SuggestDialogResumeAfter);
            }
            catch (TooManyAttemptsException)
            {
                //await context.PostAsync("I'm sorry, I'm having issues understanding you. Let's try again.");
                await context.PostAsync("ข้าเสียใจยิ่ง ข้าไม่เข้าใจ ลองเริ่มกันใหม่นะเจ้าคะ");
                await this.SendWelcomeMessageAsync(context);
            }
        }

        private async Task SuggestDialogResumeAfter(IDialogContext context, IAwaitable<User> result)
        {
            try
            {
                this.user = await result;
                DBUser.addUser(this.user);
                context.Wait(this.MessageReceivedAsync);
            }
            catch (TooManyAttemptsException)
            {
                //await context.PostAsync("I'm sorry, I'm having issues understanding you. Let's try again.");
                await context.PostAsync("ข้าเสียใจยิ่ง ข้าไม่เข้าใจ ลองเริ่มกันใหม่นะเจ้าคะ");
                await this.SendWelcomeMessageAsync(context);
            }
        }
    }
}