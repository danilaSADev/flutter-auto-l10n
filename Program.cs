using System.Text;
using FlutterAutoL10n.Common;
using FlutterAutoL10n.Prompts;
using Newtonsoft.Json;
using OpenAI;
using OpenAI.Managers;

var config = LoadConfiguration();

if (config == null)
{
    Console.WriteLine("Config file was now found or in the wrong format!");
    return;
}

var languageOptions = config.Languages;
var parsedFile = ReadPrimaryFile($"{config.Folder}/{config.FilePrefix}_en.arb");

var languageOptionsBuilder = new StringBuilder();
languageOptionsBuilder.AppendJoin(", ", languageOptions);

var openAiService = new OpenAIService(new OpenAiOptions
{
    ApiKey = config.Key
});

Prompt prompt = new GenerateTranslationsPrompt(openAiService);

var response = await prompt.Execute(new GeneratePromptTranslationRequest
{
    LanguageOptions = languageOptions.ToString() ?? string.Empty,
    MainFile = parsedFile
});

var allOptions = response.Result.Split("!stop!");
foreach (var option in languageOptions)
{
    var translation = allOptions.First(o => o.TrimStart().StartsWith(option)).TrimStart().TrimEnd();
    File.WriteAllText($"{config.Folder}/{config.FilePrefix}_{option}.arb",
        new string(translation.Skip(option.Length + 2).ToArray()));
}

string ReadPrimaryFile(string path)
{
    using StreamReader r = new StreamReader(path);
    string json = r.ReadToEnd();
    return json;
}

AppConfiguration? LoadConfiguration()
{
    AppConfiguration? appConfiguration;

    using (StreamReader r = new StreamReader("config.json"))
    {
        string json = r.ReadToEnd();
        appConfiguration = JsonConvert.DeserializeObject<AppConfiguration>(json);
    }

    return appConfiguration;
}

record AppConfiguration(
    string Key,
    List<string> Languages,
    string Folder,
    string FilePrefix
);