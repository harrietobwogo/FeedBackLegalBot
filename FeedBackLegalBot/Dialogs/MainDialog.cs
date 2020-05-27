using FeedBackLegalBot.Services;
using Microsoft.Bot.Builder.Dialogs;
using System;

using System.Threading;
using System.Threading.Tasks;

namespace FeedBackLegalBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly BotStateService _botStateService;


        public MainDialog(BotStateService botStateService)
        {
            _botStateService = botStateService ?? throw new ArgumentNullException(nameof(botStateService));

            InitializeWaterfallDialog();
        }

        private void InitializeWaterfallDialog()
        {
            //create waterfall steps
            var waterfallSteps = new WaterfallStep[]
            {
                InitialSetupAsync,
                FinalStepAsync
            };
            //Add named dialogs
            //AddDialog(new SurveyDialog($"{nameof(MainDialog)}.Survey", _botStateService));
            AddDialog(new UserRegistrationDialog($"{nameof(MainDialog)}.FeedBackSurvey", _botStateService));
            AddDialog(new WaterfallDialog($"{nameof(MainDialog)}.mainFlow", waterfallSteps));

            //Set the starting Dialog
            InitialDialogId = $"{nameof(MainDialog)}.mainFlow";
        }

        private async Task<DialogTurnResult> InitialSetupAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            


                return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.FeedBackSurvey", null, cancellationToken);
               // return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.Survey", null, cancellationToken);
            
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
