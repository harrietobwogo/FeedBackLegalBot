using FeedBackLegalBot.Models.Interfaces;
using FeedBackLegalBot.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using System;

using System.Threading;
using System.Threading.Tasks;

namespace FeedBackLegalBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly BotStateService _botStateService;
        private readonly IConfiguration _configuration;
        private readonly IFileUtility _fileUtility;


        public MainDialog(BotStateService botStateService,
                          IConfiguration configuration,
                          IFileUtility fileUtility)
        {
            _botStateService = botStateService ?? throw new ArgumentNullException(nameof(botStateService));
            _configuration = configuration;
            _fileUtility = fileUtility;

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
            AddDialog(new UserRegistrationDialog($"{nameof(MainDialog)}.FeedBackSurvey", _botStateService, _configuration, _fileUtility));
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
