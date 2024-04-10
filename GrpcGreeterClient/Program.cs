using Grpc.Core;
using Grpc.Net.Client;
using GrpcGreeterClient;
using Metanit;

const string ChannelUrl = "http://localhost:5262";

await Main();
async Task Main()
{
    Console.WriteLine(" 1 - greeter\n 2 - translator \n 3 - inviter \n 4 - server stream \n 5 - client stream \n 6 - crud api \n");
    int.TryParse(Console.ReadLine(), out var num);

    switch (num)
    {
        case 1:
            await RunGreeter();
            break;
        case 2:
            await RunTranslator();
            break;
        case 3:
            await RunInviter();
            break;
        case 4:
            await RunServerStreamMessenger();
            break;
        case 5:
            await RunClientStreamMessenger();
            break;
        case 6:
            await RunCrudApp();
            break;
        default:
            Console.WriteLine("Enter one of the nums");
            break;
    }
}

async Task RunCrudApp()
{
    // создаем канал для обмена сообщениями с сервером
    // параметр - адрес сервера gRPC
    using var channel = GrpcChannel.ForAddress(ChannelUrl);
 
    // создаем клиент
    var client = new UserService.UserServiceClient(channel);
 
    // получение списка
    ListReply users = await client.ListUsersAsync(new Google.Protobuf.WellKnownTypes.Empty());
 
    Console.WriteLine("\nGet all users");
    foreach (var user in users.Users)
    {
        Console.WriteLine($"{user.Id}. {user.Name} - {user.Age}");
    }

    // Get by id
    Console.WriteLine("\nGet by id");
    try
    {
        // получение одного объекта по id = 1
        UserReply user1 = await client.GetUserAsync(new GetUserRequest { Id = 1 });
        Console.WriteLine($"{user1.Id}. {user1.Name} - {user1.Age}");
    }
    catch (RpcException ex)
    {
        Console.WriteLine(ex.Status.Detail);    // получаем статус ответа
    }
    
    // добавление одного объекта
    UserReply user2 = await client.CreateUserAsync(new CreateUserRequest { Name = "Sam", Age = 28 });
    Console.WriteLine("\nAdd user");
    Console.WriteLine($"{user2.Id}. {user2.Name} - {user2.Age}");
    
    // delete user
    try
    {
        //обновление одного объекта - изменим имя у объекта с id = 1 на Tomas
        UserReply user = await client.UpdateUserAsync(new UpdateUserRequest { Id = 1, Name = "Tomas", Age = 38 });
        Console.WriteLine("\nDelete user");
        Console.WriteLine($"{user.Id}. {user.Name} - {user.Age}");
    }
    catch (RpcException ex)
    {
        Console.WriteLine(ex.Status.Detail);
    }
}

async Task RunClientStreamMessenger()
{
    // данные для отправки
    string[] messages = { "Привет", "Как дела?", "Че молчишь?", "Ты че, спишь?", "Ну пока" };
 
 
    // создаем канал для обмена сообщениями с сервером
    // параметр - адрес сервера gRPC
    using var channel = GrpcChannel.ForAddress(ChannelUrl);
 
    // создаем клиент
    var client = new ClientStreamMessenger.ClientStreamMessengerClient(channel);
 
    var call = client.ClientDataStream();
 
    // посылаем каждое сообщение
    foreach(var message in messages)
    {
        await call.RequestStream.WriteAsync(new ClientStreamRequest { Content= message });
    }
    
    // завершаем отправку сообшений в потоке
    await call.RequestStream.CompleteAsync();
    
    // получаем ответ сервера
    ClientStreamResponse response = await call.ResponseAsync;
    Console.WriteLine($"Ответ сервера: {response.Content}");
}

async Task RunServerStreamMessenger()
{
    // создаем канал для обмена сообщениями с сервером
    // параметр - адрес сервера gRPC
    using var channel = GrpcChannel.ForAddress(ChannelUrl);
 
    // создаем клиент
    var client = new ServerStreamMessenger.ServerStreamMessengerClient(channel);
 
    // посылаем пустое сообщение и получаем набор сообщений
    var serverData = client.ServerDataStream(new ServerStreamRequest());
 
    // получаем поток сервера
    var responseStream = serverData.ResponseStream;
    // с помощью итераторов извлекаем каждое сообщение из потока
    while (await responseStream.MoveNext(new CancellationToken()))
    {
        var response = responseStream.Current;
        Console.WriteLine(response.Content);
    }
}

async Task RunInviter()
{
    // создаем канал для обмена сообщениями с сервером
    // параметр - адрес сервера gRPC
    using var channel = GrpcChannel.ForAddress(ChannelUrl);
 
    // создаем клиент
    var client = new Inviter.InviterClient(channel);
 
    // посылаем имя и получаем приглашение на мероприятие
    var response = await client.InviteAsync(new InviteRequest { Name = "Tom" });

    var eventInvitation = response.Invitation;
    var eventDateTime = response.Start.ToDateTime();
    var eventDuration = response.Duration.ToTimeSpan();

    // выводим данные на консоль
    Console.WriteLine(eventInvitation);
    Console.WriteLine($"Начало: {eventDateTime:dd.MM HH:mm}   Длительность: {eventDuration.TotalHours} часа");
}

async Task RunTranslator()
{
    var words = new List<string>() { "red", "yellow", "green" };

    // создаем канал для обмена сообщениями с сервером
    // параметр - адрес сервера gRPC
    using var channel = GrpcChannel.ForAddress(ChannelUrl);

    var client = new Translator.TranslatorClient(channel);

    // отправляем каждое слово сервису для получения перевода
    foreach(var word in words)
    { 
        // формируем сообщение для отправки
        Request request = new Request { Word = word };
        
        // отправляем сообщение и получаем ответ
        Response response = await client.TranslateAsync(request);
        
        // выводим слово и его перевод
        Console.WriteLine($"{response.Word} : {response.Translation}");
    }
}

async Task RunGreeter()
{
    using var channel = GrpcChannel.ForAddress(ChannelUrl);
    
    // создаем клиент
    var client = new Greeter.GreeterClient(channel);
    Console.Write("Введите имя: ");
    string? name = Console.ReadLine();

    // обмениваемся сообщениями с сервером
    var reply = await client.SayHelloAsync(new HelloRequest { Name = name });
    Console.WriteLine($"Ответ сервера: {reply.Message}");
    Console.ReadKey();
}