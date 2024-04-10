using Grpc.Core;

namespace GrpcGreeter.Services;

public class MessengerServerStreamService : ServerStreamMessenger.ServerStreamMessengerBase
{
    string[] messages = { "Привет", "Как дела?", "Че молчишь?", "Ты че, спишь?", "Ну пока" };
    
    public override async Task ServerDataStream(ServerStreamRequest request, IServerStreamWriter<ServerStreamResponse> responseStream, ServerCallContext context)
    {
        foreach (var message in messages)
        {
            await responseStream.WriteAsync(new ServerStreamResponse() { Content = message });
            // для имитации работы делаем задержку в 1 секунду
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
}