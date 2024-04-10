using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace GrpcGreeter.Services;

public class InviterService : Inviter.InviterBase
{
    public override Task<InviteResponse> Invite(InviteRequest request, ServerCallContext context)
    {
        // начало мероприятия - условно следующий день 
        var eventDateTime = DateTime.UtcNow.AddDays(1);
        
        // длительность мероприятия - условно 2 часа
        var eventDuration = TimeSpan.FromHours(2);
        
        // отправляем ответ
        return Task.FromResult(new InviteResponse
        {
            Invitation = $"{request.Name}, приглашаем вас посетить мероприятие",
            Start = Timestamp.FromDateTime(eventDateTime),
            Duration = Duration.FromTimeSpan(eventDuration)
        });
    }
}