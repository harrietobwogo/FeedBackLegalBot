using FeedBackLegalBot.Models;
using FeedBackLegalBot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FeedBackLegalBot.Bots
{
    public class WelcomeBot: ActivityHandler
    {

        private readonly BotStateService _botStateService;

        public WelcomeBot(BotStateService botStateService)
        {
            _botStateService = botStateService ?? throw new System.ArgumentNullException(nameof(BotStateService));
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await GetName(turnContext, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await GetName(turnContext, cancellationToken);
                }
            }
        }

        private async Task GetName(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            UserProfile userProfile = await _botStateService.UserProfileAccessor
                                        .GetAsync(turnContext, () => new UserProfile());

            ConversationData conversationData = await _botStateService.ConversationDataAccessor
                                        .GetAsync(turnContext, () => new ConversationData());

            if (!string.IsNullOrEmpty(userProfile.Name))
            {
                await turnContext.SendActivityAsync(MessageFactory.Text($"Hey {userProfile.Name}. I'm Microsoft's Legal Bot and I provide pro-bono legal information and services to Kenyans."), cancellationToken);
            }
            else
            {
                if (conversationData.PromptedUserForName)
                {//set the name to what the user provided
                    userProfile.Name = turnContext.Activity.Text?.Trim();
                    //acknowledge that we got their name.
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Thanks {userProfile.Name}. How can I help you today?"));
                   //reset the flag to allow the bot to go through the cycle again
                    conversationData.PromptedUserForName = false;
                }
                else
                {
                    //prompt the user for their name.
                    await turnContext.SendActivityAsync(MessageFactory.Text($"What is your name?"));
                   //set the flag so that we don't prompt next time
                    conversationData.PromptedUserForName = true;
                }


                await _botStateService.UserProfileAccessor.SetAsync(turnContext, userProfile);
                await _botStateService.ConversationDataAccessor.SetAsync(turnContext, conversationData);

                await _botStateService._userState.SaveChangesAsync(turnContext);
                await _botStateService._conversationState.SaveChangesAsync(turnContext);
            }
        }
    }
}

