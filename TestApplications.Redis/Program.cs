using StackExchange.Redis;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Connecting to local redis instance");

using ConnectionMultiplexer connection = await ConnectionMultiplexer.ConnectAsync("localhost");

Console.WriteLine("Connected");

var database = connection.GetDatabase();


var test = new PersonRedisStore(database);


for (int i = 0; i < 10; i++) {
    await test.StorePerson(new Person { Id = i, Name = $"Person{i}Name", Surname = $"Person{i}Surname", Age = 20 });
}

Console.WriteLine("DoneSet");


database.Has

Console.WriteLine("DoneStore");