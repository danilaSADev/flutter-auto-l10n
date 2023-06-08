using System.Text;
using Newtonsoft.Json;
using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;

var config = LoadConfiguration();

if (config == null)
{
    Console.WriteLine("Config file was now found or in the wrong format!");
    return;
}

var openAiService = new OpenAIService(new OpenAiOptions
{
    ApiKey = config.Key
});

var languageOptions = config.Languages;
var parsedFile = ReadPrimaryFile($"{config.Folder}/{config.FilePrefix}_en.arb");

var languageOptionsBuilder = new StringBuilder();
languageOptionsBuilder.AppendJoin(", ", languageOptions);

var completionResult = await openAiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
{
    Messages = new List<ChatMessage>
    {
        new(StaticValues.ChatMessageRoles.User, $@"
        This is a flutter l10n file.
        I want this to be translated (ignoring keys) into such options {languageOptionsBuilder}. 
        Preferably, you should place each translated file from a new line in format: 'lang_code : output !stop!'.
        Do not lose any of the keys (including @@locale), translate only the values, save the file formatting.
        Here is the file: {parsedFile}!")
    },
    Model = Models.ChatGpt3_5Turbo
});

if (completionResult.Successful)
{
    var allOptions = completionResult.Choices.First().Message.Content.Split("!stop!");
    foreach (var option in languageOptions)
    {
        var translation = allOptions.First(o => o.TrimStart().StartsWith(option)).TrimStart().TrimEnd();
        File.WriteAllText($"{config.Folder}/{config.FilePrefix}_{option}.arb", new string(translation.Skip(option.Length + 2).ToArray()));
    }
}
else
{
    if (completionResult.Error == null)
    {
        throw new Exception("Unknown Error");
    }

    Console.WriteLine($"{completionResult.Error.Code}: {completionResult.Error.Message}");
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