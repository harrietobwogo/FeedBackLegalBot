using FeedBackLegalBot.Models;
using FeedBackLegalBot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FeedBackLegalBot.Dialogs
{
    public class WelcomeDialog:ComponentDialog
    {
        private readonly BotStateService _botStateService;

        public WelcomeDialog(string dialogId, BotStateService botStateService) : base(dialogId)
        {
            _botStateService = botStateService ?? throw new ArgumentNullException(nameof(botStateService));

            InitializeWaterfallDialog();
        }

        private void InitializeWaterfallDialog()
        { //create waterfall steps
            var waterfallSteps = new WaterfallStep[]
            {
                InitialStepAsync,
                FinalStepAsync
            };
            //Add named Dialogs
            AddDialog(new WaterfallDialog($"{nameof(WelcomeDialog)}.mainFlow", waterfallSteps));
            AddDialog(new TextPrompt($"{nameof(WelcomeDialog)}.name"));
            //Set the starting Dialog
            InitialDialogId = $"{nameof(WelcomeDialog)}.mainFlow";
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            UserProfile userProfile = await _botStateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            if (string.IsNullOrEmpty(userProfile.Name))
            {
                return await stepContext.PromptAsync($"{nameof(WelcomeDialog)}.name",
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("What  is your name?")
                }, cancellationToken);
            }

            else
            {
                return await stepContext.NextAsync(null, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            UserProfile userProfile = await _botStateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            if (string.IsNullOrEmpty(userProfile.Name))
            { //set the name 
                userProfile.Name = (string)stepContext.Result;
                //save any state changes tat might have occured during the turn
                await _botStateService.UserProfileAccessor.SetAsync(stepContext.Context, userProfile);
            }

            await stepContext.Context.SendActivityAsync(MessageFactory.Text(String.Format("Hi {0}. My name is Lily and I'm the virtual assistant for Microsoft's legal services.welcome to our service. Type next to continue.", userProfile.Name)), cancellationToken);
            return await stepContext.EndDialogAsync(null, cancellationToken);

        }
    }
}
