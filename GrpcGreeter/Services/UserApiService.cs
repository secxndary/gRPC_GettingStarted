using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Metanit;

namespace GrpcGreeter.Services;

public class UserApiService : UserService.UserServiceBase
{
    static int id = 0;  // счетчик для генерации id
    
    // условная база данных
    static List<User> users = new()
    {
        new User(++id, "Tom", 38), 
        new User(++id, "Bob", 42)
    };
     
    // отправляем список пользователей
    public override Task<ListReply> ListUsers(Empty request, ServerCallContext context)
    {
        var listReply = new ListReply();    // определяем список
        
        // преобразуем каждый объект из списка users в объект UserReply
        var userList = users.Select(item => new UserReply
        {
            Id = item.Id, 
            Name = item.Name, 
            Age = item.Age
        }).ToList();
        
        listReply.Users.AddRange(userList);
        
        return Task.FromResult(listReply);
    }
    
    // отправляем одного пользователя по id
    public override Task<UserReply> GetUser(GetUserRequest request, ServerCallContext context)
    {
        var user = users.Find(u => u.Id == request.Id);
        
        // если пользователь не найден, генерируем исключение
        if (user == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
        }
        
        UserReply userReply = new UserReply()
        {
            Id = user.Id, 
            Name = user.Name, 
            Age = user.Age
        };
        
        return Task.FromResult(userReply);
    }
    
    // добавление пользователя
    public override Task<UserReply> CreateUser(CreateUserRequest request, ServerCallContext context)
    {
        // формируем из данных объект User и добавляем его в список users
        var user = new User(++id, request.Name, request.Age);
        users.Add(user);
        
        var reply = new UserReply()
        {
            Id = user.Id, 
            Name = user.Name, 
            Age = user.Age
        };
        
        return Task.FromResult(reply);
    }
    
    // обновление пользователя
    public override Task<UserReply> UpdateUser(UpdateUserRequest request, ServerCallContext context)
    {
        var user = users.Find(u => u.Id == request.Id);
 
        if (user == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
        }
        
        // обновляем даннные
        user.Name = request.Name;
        user.Age = request.Age;
 
        var reply = new UserReply()
        {
            Id = user.Id, 
            Name = user.Name, 
            Age = user.Age
        };
        
        return Task.FromResult(reply);
    }
    
    // удаление пользователя
    public override Task<UserReply> DeleteUser(DeleteUserRequest request, ServerCallContext context)
    {
        var user = users.Find(u => u.Id == request.Id);
 
        if (user == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
        }
 
        users.Remove(user);
        
        var reply = new UserReply()
        {
            Id = user.Id, 
            Name = user.Name, 
            Age = user.Age
        };
        
        return Task.FromResult(reply);
    }
}

// модель пользователя - класс User
class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public User(int id, string name, int age)
    {
        Id = id;
        Name = name; 
        Age = age; 
    }
}