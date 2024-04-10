using Grpc.Core;
namespace GrpcGreeter.Services;

public class TranslatorService : Translator.TranslatorBase
{
    Dictionary<string, string> words = new () { {"red", "красный"}, {"green", "зеленый" }, {"blue", "синий" } };
 
    public override Task<Response> Translate(Request request, ServerCallContext context)
    {
        // получаем отправленное слово
        var word = request.Word;
        Console.WriteLine($"Запрошено слово: {word}");
        
        // ищем в словаре и получаем его в переменную translation
        var translation = words.GetValueOrDefault(word, "не найдено");

        // отправляем ответ
        return Task.FromResult(new Response
        {
            Word = word,
            Translation = translation
        });
    }
}