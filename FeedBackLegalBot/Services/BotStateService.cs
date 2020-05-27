using FeedBackLegalBot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;

namespace FeedBackLegalBot.Services
{
    public class BotStateService
    {
        #region Variables
        public UserState _userState { get; }
        public ConversationState _conversationState { get; }

        //IDs
        public static string UserProfileId { get; } = $"{nameof(BotStateService)}.UserProfile";
       
        public static string ConversationId { get; set; } = $"{nameof(BotStateService)}.ConversationData";
        public static string DialogStateId { get; } = $"{nameof(BotStateService)}.DialogState";

        //Accessors
        public IStatePropertyAccessor<UserProfile> UserProfileAccessor { get; set; }

        public IStatePropertyAccessor<ConversationData> ConversationDataAccessor { get; set; }
        public IStatePropertyAccessor<DialogState> DialogStateAccessor { get; set; }

        #endregion
        public BotStateService(UserState userState, ConversationState conversationState)
        {
            _conversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            _userState = userState ?? throw new ArgumentNullException(nameof(userState));

            InitializeAccessors();
        }

        public void InitializeAccessors()
        {     
              //initialize the accessor

            ConversationDataAccessor = _conversationState.CreateProperty<ConversationData>(ConversationId);
            DialogStateAccessor = _conversationState.CreateProperty<DialogState>(DialogStateId);
            UserProfileAccessor = _userState.CreateProperty<UserProfile>(UserProfileId);
       

        }
    }
}
