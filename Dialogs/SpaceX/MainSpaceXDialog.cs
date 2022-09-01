using Demo.Bot.v4.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System.Collections.Generic;
using System.IO;
using AdaptiveCards;
using AdaptiveCards.Templating;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

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
                    "PastLaunch",
                    "NextLaunch",
                    "Launches",
                    "Launchpad",
                    "Crew"
                })
            };
            return await stepContext.PromptAsync($"{nameof(MainSpaceXDialog)}.choice", promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var value = ((FoundChoice)stepContext.Result).Value;
            stepContext.Values["choice"] = value;

            switch (stepContext.Values["choice"])
            {
                case "PastLaunch":
                    var launch   = await _spaceXApi.GetPastLaunch();
                    var cardJson = await GetLaunchCardAsJson(launch);
                    var adaptiveCardAttachment = new Attachment
                    {
                        ContentType = AdaptiveCard.ContentType,
                        Content     = JsonConvert.DeserializeObject(cardJson)
                    };

                    var activity = MessageFactory.Attachment(adaptiveCardAttachment);
                    await stepContext.Context.SendActivityAsync(activity, cancellationToken);
                    break;
                case "NextLaunch":
                    launch   = await _spaceXApi.GetNextLaunch();
                    activity = MessageFactory.Text($"Upcoming launch {launch.Name} is planned for {launch.DateLocal:F}");
                    await stepContext.Context.SendActivityAsync(activity, cancellationToken);
                    break;
            }
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }

        private static async Task<string> GetLaunchCardAsJson(Launch launch)
        {
            // Create a Template instance from the template payload
            using var streamReader            = new StreamReader(@"Dialogs\SpaceX\Cards\Launch.json");
            var       summaryCardTemplateJson = await streamReader.ReadToEndAsync();
            var       template                = new AdaptiveCardTemplate(summaryCardTemplateJson);

            var cardData = new
            {
                title       = launch.FlightNumber,
                launchName  = launch.Name,
                description = launch.Details,
                viewUrl     = launch.LaunchLinks.Reddit.Launch,
                launchUtc   = launch.DateUtc,
                launchPatch = launch.LaunchLinks.Patch.Small
            };

            // "Expand" the template - this generates the final Adaptive Card payload
            var cardJson = template.Expand(cardData);
            return cardJson;
        }
    }
}
