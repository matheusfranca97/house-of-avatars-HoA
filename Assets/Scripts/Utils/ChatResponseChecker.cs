using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using UnityEngine;

public static class ChatResponseChecker
{
    public static bool TryGetMatchingChatResponse(AIChatResponse[] chatResponses, string text, out AIChatResponse chatResponse)
    {
        // Regex para remover caracteres não alfabéticos
        Regex rgx = new Regex("[^a-zA-Z]");

        // Split do texto recebido, convertendo para lowercase e removendo duplicados
        string[] textSplit = text.ToLower().Split(" ").Distinct().ToArray();
        Debug.Log($"Input text split into: {string.Join(", ", textSplit)}");

        // Remover caracteres não alfabéticos das palavras
        for (int i = 0; i < textSplit.Length; i++)
        {
            textSplit[i] = rgx.Replace(textSplit[i], "");
        }
        Debug.Log($"Cleaned text split into: {string.Join(", ", textSplit)}");

        foreach (AIChatResponse response in chatResponses)
        {
            Debug.Log($"Checking response with keywords: {string.Join(", ", response.keywords)}");
            Debug.Log($"Keywords to match: {response.keywordsToMatch}");

            // Conta quantas keywords do response coincidem com o texto
            int matchingCount = response.keywords.Count(keyword => textSplit.Any(word => keyword.Equals(word)));
            Debug.Log($"Matching count for response '{response.response}': {matchingCount}");

            if (matchingCount < response.keywordsToMatch)
            {
                Debug.Log($"Not enough matches for response '{response.response}' (Required: {response.keywordsToMatch}, Found: {matchingCount})");
                continue;
            }

            // Se encontramos um match suficiente, retornamos o response
            Debug.Log($"Match found! Response: {response.response}");
            chatResponse = response;
            return true;
        }

        // Se nenhum match foi encontrado
        Debug.Log("No matching response found.");
        chatResponse = default;
        return false;
    }
}
