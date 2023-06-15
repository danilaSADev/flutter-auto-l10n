using FlutterAutoL10n.Prompts;
using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;

namespace FlutterAutoL10n.Common;

public abstract class Prompt
{
    private readonly OpenAIService _aiService;

    protected Prompt(OpenAIService aiService)
    {
        _aiService = aiService;
    }

    protected async Task<PromptResponse> SendPrompt(ChatCompletionCreateRequest request)
    {
        var completionResult = await _aiService.ChatCompletion.CreateCompletion(request);

        if (!completionResult.Successful)
        {
            if (completionResult.Error == null)
            {
                throw new Exception("Unknown Error");
            }

            Console.WriteLine($"{completionResult.Error.Code}: {completionResult.Error.Message}");
        }
        
        return new GenerateTranslationPromptResponse
        {
            Result = completionResult.Choices.First().Message.Content
        };
    }
    public abstract Task<PromptResponse> Execute(PromptRequest request);
}