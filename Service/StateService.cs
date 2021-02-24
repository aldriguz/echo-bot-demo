using EchoBotDemo.Models;
using Microsoft.Bot.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EchoBotDemo.Service
{
    public class StateService
    {
        #region
        //State services
        public ConversationState ConversationState { get; }
        public UserState UserState { get; }

        // IDs
        public static string UserProfileId { get; } = $"{nameof(StateService)}.UserProfile" ;
        public static string ConversationDataId { get; } = $"{nameof(ConversationState)}.ConversationData";
        // Accesor
        public IStatePropertyAccessor<UserProfile> UserProfileAccesor { get; set; }
        public IStatePropertyAccessor<ConversationData> ConversationDataAccessor { get; set; }
        #endregion

        public StateService(UserState userState, ConversationState conversationState)
        {
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(ConversationState));
            UserState = userState ?? throw new ArgumentNullException(nameof(UserState));

            InitializeAccessors();
        }

        private void InitializeAccessors()
        {
            //Initialize user state
            UserProfileAccesor = UserState.CreateProperty<UserProfile>(UserProfileId);
            ConversationDataAccessor = ConversationState.CreateProperty<ConversationData>(ConversationDataId);
        }
    }
}
