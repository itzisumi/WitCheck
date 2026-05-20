using APICaller.API;
using WebGUI.DTOs;

namespace WebGUI.Helpers;

public static class QuestionRouteHelper
{
    private static readonly string[] TrueFalseAnswers = ["FALSE", "TRUE"];

    public static async Task<string> ResolvePlayerProgressRouteAsync(
        LobbyCaller lobbyCaller,
        string lobbyCode,
        QuestionDTO? question)
    {
        if (question == null)
        {
            return $"/PersonalPlace/{lobbyCode}";
        }

        return await ResolvePlayerAnswerRouteAsync(lobbyCaller, lobbyCode);
    }

    public static async Task<string> ResolvePlayerAnswerRouteAsync(LobbyCaller lobbyCaller, string lobbyCode)
    {
        var answers = await lobbyCaller.GetAllAnswers(lobbyCode);
        return IsTrueFalseQuestion(answers)
            ? $"/TrueFalseAnswers/{lobbyCode}"
            : $"/MultipleChoiceAnswers/{lobbyCode}";
    }

    public static bool IsTrueFalseQuestion(IEnumerable<AnswerDTO>? answers)
    {
        if (answers == null)
        {
            return false;
        }

        var normalizedAnswers = answers
            .Where(answer => answer != null)
            .Select(answer => answer.Answer1?.Trim())
            .Where(answer => !string.IsNullOrWhiteSpace(answer))
            .Select(answer => answer!.ToUpperInvariant())
            .OrderBy(answer => answer)
            .ToArray();

        return normalizedAnswers.SequenceEqual(TrueFalseAnswers);
    }
}
