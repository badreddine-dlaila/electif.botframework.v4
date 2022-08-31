﻿using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Demo.Bot.v4.Helpers;
using Demo.Bot.v4.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DialogBot.Bots
{
    public class DialogBot<T> : ActivityHandler where T : Dialog
    {
        private readonly StateService          _stateService;
        private readonly T                     _dialog;
        private readonly ILogger<DialogBot<T>> _logger;

        // Dependency injected dictionary for storing ConversationReference objects used in NotifyController to proactively message users
        private readonly ConcurrentDictionary<string, ConversationReference> _conversationReferences;

        public DialogBot(StateService stateService,
                         T dialog,
                         ConcurrentDictionary<string, ConversationReference> conversationReferences,
                         ILogger<DialogBot<T>> logger)
        {
            _stateService           = stateService ?? throw new ArgumentNullException(nameof(stateService));
            _dialog                 = dialog ?? throw new ArgumentNullException(nameof(dialog));
            _conversationReferences = conversationReferences ?? throw new ArgumentNullException(nameof(conversationReferences));
            _logger                 = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any changes that might have occurred during the turn
            await _stateService.UserState.SaveChangesAsync(turnContext, false, cancellationToken);
            await _stateService.ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Running dialog with Message Activity.");

            var conversationReference = (turnContext.Activity as Activity)?.GetConversationReference();
            _conversationReferences.AddOrUpdate(conversationReference?.User.Id, conversationReference, (key, newValue) => conversationReference);

            var activity = turnContext.Activity;
            if (string.IsNullOrWhiteSpace(activity.Text) && activity.Value != null)
                activity.Text = JsonConvert.SerializeObject(activity.Value);

            // Run the dialog with the new message activity
            //await _dialog.Run(turnContext, _stateService.DialogStateAccessor, cancellationToken);
        }
    }
}
