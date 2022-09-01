using Demo.Bot.v4.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System.Collections.Generic;

namespace Demo.Bot.v4.Dialogs.SpaceX
{
    public class MainSpaceXDialog : ComponentDialog
    {
        private readonly StateService              _stateService;
        private readonly GithubApi                 _githubApi;
        private readonly SpaceXApi                 _spaceXApi;
        private readonly ILogger<MainSpaceXDialog> _logger;

        public MainSpaceXDialog(StateService stateService, GithubApi githubApi, SpaceXApi spaceXApi, ILogger<MainSpaceXDialog> logger)
        {
            _stateService = stateService;
            _githubApi    = githubApi;
            _spaceXApi    = spaceXApi;
            _logger       = logger;

            InitializeWaterfallDialog();
        }

        private void InitializeWaterfallDialog()
        {
            // Waterfall steps
            var waterfallSteps = new WaterfallStep[]
            {
                //GreetingStep,
                InitialStep, FinalStep
            };

            // Add named dialogs
            AddDialog(new WaterfallDialog($"{nameof(MainSpaceXDialog)}.mainFlow", waterfallSteps));
            AddDialog(new GreetingDialog($"{nameof(MainSpaceXDialog)}.greeting", _stateService, _githubApi));
            AddDialog(new ChoicePrompt($"{nameof(MainSpaceXDialog)}.choice"));

            // Set the starting dialog
            InitialDialogId = $"{nameof(MainSpaceXDialog)}.mainFlow";
        }

        private static async Task<DialogTurnResult> GreetingStep(WaterfallStepContext stepContext, CancellationToken cancellationToken) => await stepContext.BeginDialogAsync($"{nameof(MainSpaceXDialog)}.greeting", null, cancellationToken);

        private async Task<DialogTurnResult> InitialStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var company  = await _spaceXApi.GetCompanyInfo();
            var activity = MessageFactory.Text($"Hello there, SpaceX Bot here 🤖\r\n{company.Summary}");
            await stepContext.Context.SendActivityAsync(activity, cancellationToken);

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("➡️ here is what I can do gor ya !"),
                Choices = ChoiceFactory.ToChoices(new List<string>
                {
                    "Next Launch 🚀",
                    "All Launches",
                    "Crew ",
                    "Launchpad"
                })
            };
            return await stepContext.PromptAsync($"{nameof(MainSpaceXDialog)}.choice", promptOptions, cancellationToken);
        }

        private static async Task<DialogTurnResult> FinalStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
