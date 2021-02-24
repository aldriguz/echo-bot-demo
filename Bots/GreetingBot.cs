using EchoBotDemo.Models;
using EchoBotDemo.Service;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EchoBotDemo.Bots
{
    public class GreetingBot : ActivityHandler
    {
        #region Variables
        private readonly StateService _stateService;
        #endregion
        public GreetingBot(StateService stateService)
        {
            _stateService = stateService ?? throw new ArgumentNullException(nameof(StateService));
        }

        private async Task GetName(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            UserProfile userProfile = await _stateService.UserProfileAccesor.GetAsync(turnContext, () => new UserProfile());
            ConversationData conversationData = await _stateService.ConversationDataAccessor.GetAsync(turnContext, () => new ConversationData());

            if (!string.IsNullOrEmpty(userProfile.Name))
            {
                await turnContext.SendActivityAsync(MessageFactory.Text($"Hi {userProfile.Name}. How I can help you today?"), cancellationToken);
            }
            else
            {
                if (conversationData.PromptedUserForName)
                {
                    // Set the name to what the user provided
                    userProfile.Name = turnContext.Activity.Text?.Trim();

                    // Acknowledge that we got their name
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Thanks {userProfile.Name}. How I can help you today?"), cancellationToken);

                    // Reset the flag to allow the bot to go though the cycle again
                    conversationData.PromptedUserForName = false;
                }
                else
                {
                    // Prompt the user for their name
                    await turnContext.SendActivityAsync(MessageFactory.Text($"What is your name?"), cancellationToken);

                    // Set the flag to true, so we don't prompt in the next turn
                    conversationData.PromptedUserForName = true;
                }
            }

            // Save any state changes that might have ocurred during this turn 
            await _stateService.UserProfileAccesor.SetAsync(turnContext, userProfile);
            await _stateService.ConversationDataAccessor.SetAsync(turnContext, conversationData);

            await _stateService.UserState.SaveChangesAsync(turnContext);
            await _stateService.ConversationState.SaveChangesAsync(turnContext);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await GetName(turnContext, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                await GetName(turnContext, cancellationToken);
            }
        }
    }
}
