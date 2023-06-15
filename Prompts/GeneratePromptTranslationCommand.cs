using FlutterAutoL10n.Common;

namespace FlutterAutoL10n.Prompts;

public sealed class GeneratePromptTranslationRequest : PromptRequest
{
    public string LanguageOptions { get; set; } = string.Empty;
    public string MainFile { get; set; } = string.Empty;
}