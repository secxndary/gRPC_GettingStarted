using Grpc.Core;

namespace GrpcGreeter.Services;

public class MessengerClientStreamService : ClientStreamMessenger.ClientStreamMessengerBase
{
    string[] messages = { "Привет", "Как дела?", "Че молчишь?", "Ты че, спишь?", "Ну пока" };
    
    public override async Task<ClientStreamResponse> ClientDataStream(IAsyncStreamReader<ClientStreamRequest> requestStream, ServerCallContext context)
    {
        await foreach(ClientStreamRequest request in requestStream.ReadAllAsync())
        {
            Console.WriteLine(request.Content);
        }
        Console.WriteLine("Все данные получены...");
        return new ClientStreamResponse { Content = "все данные получены" };
    }
}