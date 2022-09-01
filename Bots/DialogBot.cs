using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Demo.Bot.v4.Helpers;
using Demo.Bot.v4.Models;
using Demo.Bot.v4.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Demo.Bot.v4.Bots
{
    public class DialogBot<T> : ActivityHandler where T : Dialog
    {
        private readonly StateService          _stateService;
        private readonly T                     _dialog;
        private readonly ILogger<DialogBot<T>> _logger;

        public DialogBot(StateService stateService, T dialog, ILogger<DialogBot<T>> logger)
        {
            _stateService = stateService ?? throw new ArgumentNullException(nameof(stateService));
            _dialog       = dialog ?? throw new ArgumentNullException(nameof(dialog));
            _logger       = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any changes that might have occurred during the turn
            await _stateService.UserState.SaveChangesAsync(turnContext, false, cancellationToken);
            await _stateService.ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            // Check if we have the user's name in state
            var userProfile = await _stateService.UserProfileAccessor.GetAsync(turnContext, () => new UserProfile(), cancellationToken);

            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id && string.IsNullOrEmpty(userProfile.Login))
                    await _dialog.Run(turnContext, _stateService.DialogStateAccessor, cancellationToken);
            }
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Running dialog with Message Activity.");

            // Run the dialog with the new message activity
            await _dialog.Run(turnContext, _stateService.DialogStateAccessor, cancellationToken);
        }
    }
}
