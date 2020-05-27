using FeedBackLegalBot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;

using System;
using System.Collections.Generic;

using System.Threading;
using System.Threading.Tasks;

namespace FeedBackLegalBot.Dialogs
{
    public class SurveyDialog : ComponentDialog
    {
        private readonly BotStateService _botStateService;

        public SurveyDialog(string dialogId, BotStateService botStateService) : base(dialogId)
        {
            _botStateService = botStateService ?? throw new ArgumentNullException(nameof(botStateService));

            InitializeWaterfallDialog();
        }

        private void InitializeWaterfallDialog()
        {
            var waterfallSteps = new WaterfallStep[]
            { 
                //Create Waterfall steps
               MainMenuStepAsync,
               ServiceMenuStepAsync,
               ServiceSubMenuStepAsync,
               QuestionStepAsync,
               ProvisionServiceStepAsync,
               AccessibilityStepAsync,
               ImproveAccessStepAsync,
               RecommendationStepAsync,
               OverallexperienceStepAsync,
               ImproveExperienceStepAsync,
               SurveySummaryStepAsync

            };
            //Add Named Dialogs
            AddDialog(new WaterfallDialog($"{nameof(SurveyDialog)}.mainFlow", waterfallSteps));
            AddDialog(new ChoicePrompt($"{nameof(SurveyDialog)}.MainMenu"));
            AddDialog(new ChoicePrompt($"{nameof(SurveyDialog)}.ServiceMenu"));
            AddDialog(new ChoicePrompt($"{nameof(SurveyDialog)}.ServiceSubMenu"));
            AddDialog(new ChoicePrompt($"{nameof(SurveyDialog)}.Question"));
            AddDialog(new TextPrompt($"{nameof(SurveyDialog)}.ProvisionService"));
            AddDialog(new ChoicePrompt($"{nameof(SurveyDialog)}.Accessibility"));
            AddDialog(new TextPrompt($"{nameof(SurveyDialog)}.ImproveAccess"));
            AddDialog(new ChoicePrompt($"{nameof(SurveyDialog)}.recommendation"));
            AddDialog(new ChoicePrompt($"{nameof(SurveyDialog)}.Overallexperience"));
            AddDialog(new TextPrompt($"{nameof(SurveyDialog)}.ImproveExperience"));
            AddDialog(new ChoicePrompt($"{nameof(SurveyDialog)}.SurveySummary"));
            //set the starting Dialog
            InitialDialogId = $"{nameof(UserRegistrationDialog)}.mainFlow";
        }
        //waterfall steps
        //Welcome message
        private async Task<DialogTurnResult> MainMenuStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.Mainmenu",
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Good Morning, Welcome back. Please choose from:"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "1. INFORMATION", " 2. NEWS", "3. REFERAL", "4. SURVEY", "5. UPDATE PROFILE", "6. SHARE" }),
                }, cancellationToken);
        }
        //Get User Name
        private async Task<DialogTurnResult> ServiceMenuStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["MainMenu"] = ((FoundChoice)stepContext.Result).Value;

            return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.ServiceMenu", new PromptOptions
            {

                Prompt = MessageFactory.Text("Please Choose from:"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "1. EMPLOYMENT LAW", " 2. PROPERTY LAW", "3. DOMESTIC LAW" }),

            }, cancellationToken);

        }
        //Get county Location
        private async Task<DialogTurnResult> ServiceSubMenuStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["ServiceMenu"] = ((FoundChoice)stepContext.Result).Value;

            return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.ServiceSubMenu", new PromptOptions
            {
                Prompt = MessageFactory.Text("Please Choose from:"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "1. INFORMATION", " 2. REQUEST A LAWYER", "3. GIVE FEEDBACK" }),
            }, cancellationToken);
        }
        //get sub-county location
        private async Task<DialogTurnResult> QuestionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["ServiceSubMenu"] = ((FoundChoice)stepContext.Result).Value;

            return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.Question", new PromptOptions
            {
                Prompt = MessageFactory.Text("Answer a few questions to tell us how we can improve. How useful was the information or service you received?"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "1. USEFUL", "2. NOT USEFUL" }),
            }, cancellationToken);
        }
        //get ward location
        private async Task<DialogTurnResult> ProvisionServiceStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["Question"] = ((FoundChoice)stepContext.Result).Value;

            return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.ProvisionService", new PromptOptions
            {
                Prompt = MessageFactory.Text("Okay, what information or service would you like us to provide? please type your response."),
            }, cancellationToken);
        }
        private async Task<DialogTurnResult> AccessibilityStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["ProvisionService"] = (string)stepContext.Result;

            return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.Accessibility", new PromptOptions
            {
                Prompt = MessageFactory.Text("Well noted. How easy was it for you to access the information you required? Please choose:"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "1. EASY", "2. HARD" }),
            }, cancellationToken);
        }
        private async Task<DialogTurnResult> ImproveAccessStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["Accessibility"] = ((FoundChoice)stepContext.Result).Value;

            return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.ImproveAccess", new PromptOptions
            {
                Prompt = MessageFactory.Text("Almost done! How can I make it easier? Please type your Response"),
            }, cancellationToken);
        }
        private async Task<DialogTurnResult> RecommendationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["ImproveAccess"] = (string)stepContext.Result;

            return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.recommendation", new PromptOptions
            {
                Prompt = MessageFactory.Text("How likely are you to recommend the service to your family and friends? 0-not likely, 10-very likely"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "0", "10" }),
            }, cancellationToken);
        }
        private async Task<DialogTurnResult> OverallexperienceStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["recommendation"] = ((FoundChoice)stepContext.Result).Value;

            return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.Overallexperience", new PromptOptions
            {
                Prompt = MessageFactory.Text("Thank you.how would you rate your overall experience with our service? 0-not likely, 10-very likely"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "0", "10" }),
            }, cancellationToken);
        }
        private async Task<DialogTurnResult> ImproveExperienceStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["Overallexperience"] = ((FoundChoice)stepContext.Result).Value;

            return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.ImproveExperience", new PromptOptions
            {
                Prompt = MessageFactory.Text("Lastly how can I make your overall experience better? please type your response."),
            }, cancellationToken);
        }
        private async Task<DialogTurnResult> SurveySummaryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["ImproveExperience"] = (string)stepContext.Result;

            return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.SurveySummary", new PromptOptions
            {
                Prompt = MessageFactory.Text("Thank you very much for your feedback.we appreciate your support.please choose from. MAIN MENU to continue using the service"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "1. INFORMATION", " 2. NEWS", "3. REFERAL", "4. SURVEY", "5. UPDATE PROFILE", "6. SHARE" }),
            }, cancellationToken);
            stepContext.Values["SurveySummary"] = (FoundChoice)stepContext.Result;
            //waterfallStep always finishes with the end of the waterfall or with another dialog here it is the end
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);

        }
    }







    
}
