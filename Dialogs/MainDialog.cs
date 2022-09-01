using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Demo.Bot.v4.Models;
using Demo.Bot.v4.Services;
using Microsoft.Bot.Builder.Dialogs;

namespace Demo.Bot.v4.Dialogs
{
    /// <summary>
    /// 🧩 The glue dialog that tights GreetingDialog and BugReportDialog
    /// </summary>
    public class MainDialog : ComponentDialog
    {
        private readonly StateService _stateService;
        private readonly GithubApi    _githubApi;

        public MainDialog(StateService stateService, GithubApi githubApi) : base(nameof(MainDialog))
        {
            _stateService = stateService ?? throw new ArgumentNullException(nameof(stateService));
            _githubApi    = githubApi;

            InitializeWaterfallDialog();
        }

        private void InitializeWaterfallDialog()
        {
            // Waterfall steps
            var waterfallSteps = new WaterfallStep[] { InitialStep, FinalStep };

            // Add named dialogs
            AddDialog(new GreetingDialog($"{nameof(MainDialog)}.greeting", _stateService, _githubApi)); // <-- greeting subDialog
            AddDialog(new BugReportDialog($"{nameof(MainDialog)}.bugReport", _stateService));           // <-- bugReport subDialog
            AddDialog(new WaterfallDialog($"{nameof(MainDialog)}.mainFlow", waterfallSteps));

            // Set the starting dialog
            InitialDialogId = $"{nameof(MainDialog)}.mainFlow";
        }

        private async Task<DialogTurnResult> InitialStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Check if we have the user's name in state
            var userProfile = await _stateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);
            if (string.IsNullOrEmpty(userProfile.Login))
                return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.greeting", null, cancellationToken);

            return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.bugReport", null, cancellationToken);
        }

        private static async Task<DialogTurnResult> FinalStep(WaterfallStepContext stepContext, CancellationToken cancellationToken) => await stepContext.EndDialogAsync(null, cancellationToken);
    }
}
