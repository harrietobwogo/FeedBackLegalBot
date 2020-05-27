using FeedBackLegalBot.Models;
using FeedBackLegalBot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FeedBackLegalBot.Dialogs
{
    public class UserRegistrationDialog : ComponentDialog
    {
        private readonly BotStateService _botStateService;

        public UserRegistrationDialog(string dialogId, BotStateService botStateService) : base(dialogId)
        {
            _botStateService = botStateService ?? throw new ArgumentNullException(nameof(botStateService));

            InitializeWaterfallDialog();
        }

        private void InitializeWaterfallDialog()
        {
            var waterfallSteps = new WaterfallStep[]
            { 
                //Create Waterfall steps
                LanguageStepAsync,
                AccessStepAsync,
                DescriptionStepAsync,
                CountyStepAsync,
                SubCountyStepAsync,
                WardStepAsync,
                UserNameStepAsync,
                PasswordStepAsync,
                MainMenuStepAsync
            };
            //Add Named Dialogs
            AddDialog(new WaterfallDialog($"{nameof(UserRegistrationDialog)}.mainFlow", waterfallSteps));
            AddDialog(new ChoicePrompt($"{nameof(UserRegistrationDialog)}.Language"));
            AddDialog(new ChoicePrompt($"{nameof(UserRegistrationDialog)}.access"));
            AddDialog(new TextPrompt($"{nameof(UserRegistrationDialog)}.Name"));
            AddDialog(new ChoicePrompt($"{nameof(UserRegistrationDialog)}.county"));
            AddDialog(new ChoicePrompt($"{nameof(UserRegistrationDialog)}.subCounty"));
            AddDialog(new ChoicePrompt($"{nameof(UserRegistrationDialog)}.ward"));
            AddDialog(new TextPrompt($"{nameof(UserRegistrationDialog)}.userName"));
            AddDialog(new TextPrompt($"{nameof(UserRegistrationDialog)}.password"));
            AddDialog(new ChoicePrompt($"{nameof(UserRegistrationDialog)}.mainMenu"));
            //set the starting Dialog
            InitialDialogId = $"{nameof(UserRegistrationDialog)}.mainFlow";
        }
        //waterfall steps
        //Welcome message
        private async Task<DialogTurnResult> LanguageStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.Language",
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("welcome to our service. Please choose your preferred language."),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "KISWAHILI", "ENGLISH" }),
                }, cancellationToken);
        }
        private async Task<DialogTurnResult> AccessStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["Language"] = ((FoundChoice)stepContext.Result).Value;
            if (stepContext.Values["Language"] == "KISWAHILI")
            {
                return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.access",

                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Ikiwa unayo akaunti tafadhali ingia.Mgeni kwa huduma hii kwanza unda akaunti"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "SIGN UP ", "LOG IN" }),
                }, cancellationToken);
            }
            else
            {

                return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.access",

                new PromptOptions
                {
                    Prompt = MessageFactory.Text("If you have an account please log in. New to this service first create an account"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "SIGN UP ", "LOG IN" }),
                }, cancellationToken);
            }
        }
        //Get User Name
        private async Task<DialogTurnResult> DescriptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["access"] = ((FoundChoice)stepContext.Result).Value;
            if (stepContext.Values["Language"] == "KISWAHILI")
            {
                return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.Name", new PromptOptions
                {

                    Prompt = MessageFactory.Text("Karibu katika huduma yetu, tungependa kukuuliza maswali machache kwa madhumuni ya usajili. Jina lako kamili ni nani?"),
                }, cancellationToken);


            }
            else
            {

                return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.Name", new PromptOptions
                {

                    Prompt = MessageFactory.Text("Welcome to our service, we would like to ask you a few questions for the purpose of registration. What is your full name?"),
                }, cancellationToken);
            }
        }
        //Get county Location
        private async Task<DialogTurnResult> CountyStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //Get location information
            var counties = GetLocations("county");
            //Filter for administrative location 
            //We'll need present list to user
            stepContext.Values["Name"] = (string)stepContext.Result;
            if (stepContext.Values["Language"] == "KISWAHILI")
            {

                return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.county", new PromptOptions
                {
                    Prompt = MessageFactory.Text("Unaishi kaunti gani? "),
                    Choices = ChoiceFactory.ToChoices(counties),
                }, cancellationToken);
            }
            else
            {
                return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.county", new PromptOptions
                {
                    Prompt = MessageFactory.Text("Which county do you live in? "),
                    Choices = ChoiceFactory.ToChoices(counties),
                }, cancellationToken);

            }
        }
        //get sub-county location
        private async Task<DialogTurnResult> SubCountyStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string result = ((FoundChoice)stepContext.Result).Value;
            stepContext.Values["county"] = result;

            var subCounties = GetLocations("constituency", "county", result);
          
            if (stepContext.Values["Language"] == "KISWAHILI")
            {

                return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.subCounty", new PromptOptions
                {
                    Prompt = MessageFactory.Text("Je! Uko kaunti gani ndogo ? "),
                    Choices = ChoiceFactory.ToChoices(subCounties),
                }, cancellationToken);
            }
            else
            {
                return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.subCounty", new PromptOptions
                {
                    Prompt = MessageFactory.Text("Which sub-county are you located at?"),
                    Choices = ChoiceFactory.ToChoices(subCounties),
                }, cancellationToken);

            }
        }
        //get ward location
        private async Task<DialogTurnResult> WardStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string result = ((FoundChoice)stepContext.Result).Value;
            stepContext.Values["subCounty"] = result;

            var wards = GetLocations("ward", "constituency", result);
            if (stepContext.Values["Language"] == "KISWAHILI")
            {

                return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.ward", new PromptOptions
                {
                    Prompt = MessageFactory.Text("Unaishi katika wadi gani ? "),
                    Choices = ChoiceFactory.ToChoices(wards),
                }, cancellationToken);
            }
            else
            {
                return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.ward", new PromptOptions
                {
                    Prompt = MessageFactory.Text("Which ward do you live in?"),
                    Choices = ChoiceFactory.ToChoices(wards),
                }, cancellationToken);

            }
        }
        private async Task<DialogTurnResult> UserNameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            stepContext.Values["ward"] = ((FoundChoice)stepContext.Result).Value;
            if (stepContext.Values["Language"] == "KISWAHILI")
            {
                return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.userName",
                  new PromptOptions
                  {
                      Prompt = MessageFactory.Text("Ingiza jina la mtumiaji linalopendekezwa"),
                  }, cancellationToken);
            }
            else
            {

                return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.userName",
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Enter preferred username "),
                }, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> PasswordStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            stepContext.Values["userName"] = (string)stepContext.Result;
            if (stepContext.Values["Language"] == "KISWAHILI")
            {
                return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.password",
                  new PromptOptions
                  {
                      Prompt = MessageFactory.Text("Ingiza nenosiri la kipekee"),
                  }, cancellationToken);
            }
            else
            {

                return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.password",
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Enter a unique pasword"),
                }, cancellationToken);
            }
        }


        private async Task<DialogTurnResult> MainMenuStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["password"] = (string)stepContext.Result;
            if (stepContext.Values["Language"] == "KISWAHILI")
            {
                //Get the current profile object from user state
                var userProfile = await _botStateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);
                //var Location = await _botStateService.LocationAccessor.GetAsync(stepContext.Context, () => new Location(), cancellationToken);
                //save all of the data inside the user profile
                userProfile.name = (string)stepContext.Values["Name"];
                userProfile.County = (string)stepContext.Values["county"];
                userProfile.SubCounty = (string)stepContext.Values["subCounty"];
                userProfile.Ward = (string)stepContext.Values["ward"];
                userProfile.UserName = (string)stepContext.Values["userName"];
                userProfile.Password = (string)stepContext.Values["password"];

                //show Summary to the user
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($" Huu Hapa muhtasari wa Profaili yako: "), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(string.Format("Jina:{0}", userProfile.name)), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(string.Format("Kaunti:{0}", userProfile.County)), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(string.Format("Kaunti ndogo:{0}", userProfile.SubCounty)), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(string.Format("Wadi:{0}", userProfile.Ward)), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(String.Format("Your Username: {0}", userProfile.UserName)), cancellationToken);

                //await stepContext.Context.SendActivityAsync(MessageFactory.Text(string.Format("Details:{0}", GetUserDetails())), cancellationToken);

                await stepContext.Context.SendActivityAsync(MessageFactory.Text(string.Format(" Hongera {0}!Umesajiliwa kutumia huduma yetu.Tafadhali chagua(1.MAIN MENU) kuendelea kutumia huduma", userProfile.name)), cancellationToken);
                //save data in userstate
                await _botStateService.UserProfileAccessor.SetAsync(stepContext.Context, userProfile);
                //Write user details to a json file
                var userDetails = JsonConvert.SerializeObject(userProfile, Formatting.Indented);
                var filePath = @"C:\Users\Tech Jargon\source\repos\FeedBackLegalBot\FeedBackLegalBot\Data\UserDetails.json";
                if (!File.Exists(filePath))
                {
                    File.WriteAllText(filePath, userDetails);
                }
                else
                {
                    File.AppendAllText(filePath, userDetails);
                }



                //display main menu
                return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.mainMenu",
                   new PromptOptions
                   {
                       Prompt = MessageFactory.Text("MAIN MENU"),
                       Choices = ChoiceFactory.ToChoices(new List<string> { "TAARIFA ", "HABARI", "RUFAA", "UTAFITI", "SASISHA PROFAILI", "SHARE" }),
                   }, cancellationToken);

                stepContext.Values["mainMenu"] = (FoundChoice)stepContext.Result;
                //waterfallStep always finishes with the end of the waterfall or with another dialog here it is the end
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
           
            else
            {
                //Get the current profile object from user state
                var userProfile = await _botStateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);
                //var Location = await _botStateService.LocationAccessor.GetAsync(stepContext.Context, () => new Location(), cancellationToken);
                //save all of the data inside the user profile
                userProfile.name = (string)stepContext.Values["Name"];
                userProfile.County = (string)stepContext.Values["county"];
                userProfile.SubCounty = (string)stepContext.Values["subCounty"];
                userProfile.Ward = (string)stepContext.Values["ward"];
                userProfile.UserName = (string)stepContext.Values["userName"];
                userProfile.Password = (string)stepContext.Values["password"];

                //show Summary to the user
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Here is a summary of your Profile:"), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(string.Format("Name:{0}", userProfile.name)), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(string.Format("County:{0}", userProfile.County)), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(string.Format("SubCounty:{0}", userProfile.SubCounty)), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(string.Format("Ward:{0}", userProfile.Ward)), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(String.Format("Your Username: {0}", userProfile.UserName)), cancellationToken);

                //await stepContext.Context.SendActivityAsync(MessageFactory.Text(string.Format("Details:{0}", GetUserDetails())), cancellationToken);

                await stepContext.Context.SendActivityAsync(MessageFactory.Text(string.Format("Congratulations {0}! You are now registered to use our service. Please choose (1.MAIN MENU ) to continue using the service.", userProfile.name)), cancellationToken);


                //save data in userstate
                await _botStateService.UserProfileAccessor.SetAsync(stepContext.Context, userProfile);
                //Write user details to a json file
                var userDetails = JsonConvert.SerializeObject(userProfile, Formatting.Indented);
                var filePath = @"C:\Users\Tech Jargon\source\repos\FeedBackLegalBot\FeedBackLegalBot\Data\UserDetails.json";
                if (!File.Exists(filePath))
                {
                    File.WriteAllText(filePath, userDetails);
                }
                else
                {
                    File.AppendAllText(filePath, userDetails); 
                }



                //display main menu
                return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.mainMenu",
                   new PromptOptions
                   {
                       Prompt = MessageFactory.Text("MAIN MENU"),
                       Choices = ChoiceFactory.ToChoices(new List<string> { "INFORMATION", "NEWS", "REFERAL", "SURVEY", "UPDATE PROFILE", "SHARE" }),
                   }, cancellationToken);

                stepContext.Values["mainMenu"] = (FoundChoice)stepContext.Result;
                //waterfallStep always finishes with the end of the waterfall or with another dialog here it is the end
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);

            }
        }
        private string ReadUserDetails()
        {
            var filePath = @"C:\Users\Tech Jargon\source\repos\FeedBackLegalBot\FeedBackLegalBot\Data\UserDetails.json";
            var profile = JsonConvert.DeserializeObject<UserProfile>(File.ReadAllText(filePath));
            return profile.ToString(); 
        }
        private List<string> GetLocations(string administrativeKey, string filterKey = null, string filterValue = null)
        {
            //Source
            //var file = AppContext.BaseDirectory + "/files/locations.json";
            var file = @".\Data\locations.json";
            //Read values and convert to json
            var locations = JsonConvert.DeserializeObject<JArray>(File.ReadAllText(file));


            var items = new List<string>();

            //Loop through array
            foreach (var obj in locations)
            {
                if (filterKey != null && obj.Value<string>(filterKey) != filterValue)
                {
                    continue;
                }
                items.Add(obj.Value<string>(administrativeKey));

            }
            return items.Distinct().ToList();
        }

    }
}
    
    
