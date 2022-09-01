using Demo.Bot.v4.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;

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
                GreetingStep,
                InitialStep,
                FinalStep
            };

            // Add named dialogs
            AddDialog(new WaterfallDialog($"{nameof(MainSpaceXDialog)}.mainFlow", waterfallSteps));
            AddDialog(new GreetingDialog($"{nameof(MainSpaceXDialog)}.greeting", _stateService, _githubApi));
            AddDialog(new ChoicePrompt($"{nameof(MainSpaceXDialog)}.choice"));

            // Set the starting dialog
            InitialDialogId = $"{nameof(MainSpaceXDialog)}.mainFlow";
        }

        private static async Task<DialogTurnResult> GreetingStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.BeginDialogAsync($"{nameof(MainSpaceXDialog)}.greeting", null, cancellationToken);
        }

        private async Task<DialogTurnResult> InitialStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var result = stepContext.Result;
            return await stepContext.NextAsync(stepContext, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(stepContext, cancellationToken);
        }
    }
}
