
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

public static class ChatResponseChecker
{
    public static bool TryGetMatchingChatResponse(AIChatResponse[] chatResponses, string text, out AIChatResponse chatResponse)
    {
        Regex rgx = new Regex("[^a-zA-Z]");
        string[] textSplit = text.ToLower().Split(" ").Distinct().ToArray();
        for (int i = 0; i < textSplit.Length; i++)
            textSplit[i] = rgx.Replace(textSplit[i], "");
        foreach(AIChatResponse response in chatResponses)
        {
            int matchingCount = response.keywords.Count(i => textSplit.Any(j => i.Equals(j)));
            if (matchingCount < response.keywordsToMatch)
                continue;

            chatResponse = response;
            return true;
        }

        chatResponse = default;
        return false;
    }
}
