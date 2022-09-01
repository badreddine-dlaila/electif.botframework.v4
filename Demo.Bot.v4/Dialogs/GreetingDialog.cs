﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Demo.Bot.v4.Models;
using Demo.Bot.v4.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Demo.Bot.v4.Dialogs;

public class GreetingDialog : ComponentDialog
{
    private readonly StateService _botStateService;
    private readonly GithubApi    _githubApi;

    public GreetingDialog(StateService botStateService, GithubApi githubApi) : base(nameof(GreetingDialog))
    {
        _botStateService = botStateService;
        _githubApi       = githubApi;
        InitWaterfallDialog();
    }

    // Simple pattern for simple usage
    // Setup waterfall dialog to establish all the steps that contains what methods will get called in what order within the flow
    private void InitWaterfallDialog()
    {
        // Create waterfall steps
        // Create waterfall steps (waterfall = back and forth template to utilize for conversation)
        var waterfallSteps = new WaterfallStep[]
        {
            InitialStep, // <-- waterfall step (method order is important)
            FinalStep
        };

        // Add named dialogs
        AddDialog(new WaterfallDialog($"{nameof(GreetingDialog)}.mainFlow", waterfallSteps)); // <-- subDialog1
        AddDialog(new TextPrompt($"{nameof(GreetingDialog)}.signin"));                        // <-- subDialog2  
        AddDialog(new BugReportDialog($"{nameof(MainDialog)}.bugReport", _botStateService));  // <-- bugReport subDialog

        // set the starting dialog
        InitialDialogId = $"{nameof(GreetingDialog)}.mainFlow";
    }

    private async Task<DialogTurnResult> InitialStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        // Check if we have the user's name in state
        var userProfile = await _botStateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

        // If we have the user's name, move to the next step of the dialog
        if (!string.IsNullOrEmpty(userProfile.Name))
            return await stepContext.NextAsync(null, cancellationToken);

        // If no user name found, kick start a github sign-in 
        const string signinLink = "https://github.com/login/oauth/authorize?client_id=7154fa8c5e000e28fe87&scope=user%20repo&redirect_uri=http://localhost:3978/api/oauth/callback";
        var signinCard = new SigninCard
        {
            Text    = "Github sign-in",
            Buttons = new List<CardAction> { new(ActionTypes.Signin, "Sign-in", value: signinLink) }
        };

        await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(signinCard.ToAttachment()), cancellationToken);
        var promptOptions = new PromptOptions { Prompt = new Activity { Type = ActivityTypes.Message } };
        return await stepContext.PromptAsync($"{nameof(GreetingDialog)}.signin", promptOptions, cancellationToken);
    }

    private async Task<DialogTurnResult> FinalStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        var githubCode = (string)stepContext.Result;
        var userName   = await _githubApi.GetUserName(githubCode);

        // Check if we have the user's name in state
        var userProfile = await _botStateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

        // Get the previously typed response from the user 
        if (string.IsNullOrEmpty(userProfile.Name))
        {
            // Set the name
            userProfile.Name = userName;

            // Save any changes that might have occurred during the run
            await _botStateService.UserProfileAccessor.SetAsync(stepContext.Context, userProfile, cancellationToken);
        }

        var activity = MessageFactory.Text($"Hello {userProfile.Name} 😊");
        await stepContext.Context.SendActivityAsync(activity, cancellationToken);

        // End dialog
        //return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.bugReport", null, cancellationToken);
        return await stepContext.EndDialogAsync(null, cancellationToken);
    }
}
