using System;
using Demo.Bot.v4.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace Demo.Bot.v4.Services
{
    public class StateService
    {
        // State variables
        public UserState         UserState         { get; }
        public ConversationState ConversationState { get; }

        // IDs
        // Identifies user profile data inside UserState bucket
        public static string UserProfileId      => $"{nameof(StateService)}.{nameof(UserProfile)}";
        public static string ConversationDataId => $"{nameof(StateService)}.{nameof(ConversationData)}";
        public static string DialogStateId      => $"{nameof(StateService)}.{nameof(DialogState)}";

        // Accessors
        public IStatePropertyAccessor<UserProfile>      UserProfileAccessor      { get; set; }
        public IStatePropertyAccessor<ConversationData> ConversationDataAccessor { get; set; }
        public IStatePropertyAccessor<DialogState>      DialogStateAccessor      { get; set; }

        public StateService(UserState userState, ConversationState conversationState)
        {
            UserState         = userState ?? throw new ArgumentException(nameof(userState));
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));

            InitializeAccessors();
        }

        private void InitializeAccessors()
        {
            // Tell the bucket about the variable UserProfile, identified by UserProfileId
            UserProfileAccessor = UserState.CreateProperty<UserProfile>(UserProfileId);

            // Tell the bucket about the variable ConversationData, identified by ConversationDataId
            ConversationDataAccessor = ConversationState.CreateProperty<ConversationData>(ConversationDataId);
            DialogStateAccessor      = ConversationState.CreateProperty<DialogState>(DialogStateId);
        }
    }
}
