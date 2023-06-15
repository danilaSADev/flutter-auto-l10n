using FlutterAutoL10n.Common;
using OpenAI.Managers;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;

namespace FlutterAutoL10n.Prompts;

public class GenerateTranslationsPrompt : Prompt
{
    public GenerateTranslationsPrompt(OpenAIService aiService)
        : base(aiService)
    {
    }

    public override async Task<PromptResponse> Execute(PromptRequest request)
    {
        var promptRequest = (GeneratePromptTranslationRequest)request;

        return await SendPrompt(new ChatCompletionCreateRequest
        {
            Messages = new List<ChatMessage>
            {
                new (StaticValues.ChatMessageRoles.System, "Act as a localization generator for Flutter Application."),
                new(StaticValues.ChatMessageRoles.User, $@"
                    This is a flutter l10n file.
                    I want this to be translated (ignoring keys) into such options {promptRequest.LanguageOptions}. 
                    Preferably, you should place each translated file from a new line in format: 'lang_code : output !stop!'.
                    Do not lose any of the keys (including @@locale), translate only the values, save the file formatting.
                    Here is the file: {promptRequest.MainFile}!")
            },
            Model = Models.ChatGpt3_5Turbo,
            Temperature = 0.2f
        });
    }
}