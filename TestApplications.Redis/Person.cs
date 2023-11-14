using StackExchange.Redis;

public class Person {
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Surname { get; set; } = "";
    public int Age { get; set; } = 0;
}

public class PersonRedisStore {
    private readonly IDatabase database;

    public PersonRedisStore(IDatabase database) {
        this.database = database;
    }

    public async Task StorePerson(Person person) {
        var personKey = $"Stores:Person:Entries:{person.Id}";

        var b = database.CreateBatch();

        var metaDataTask = database.SetAddAsync("Stores:Person:$Meta", $"{person.Id}");
        var property1Task = database.HashSetAsync(personKey, "Name", person.Name);
        var property2Task = database.HashSetAsync(personKey, "Surname", person.Surname);
        var property3Task = database.HashSetAsync(personKey, "Age", person.Age);

        b.Execute();

        b.WaitAll(metaDataTask, property1Task, property2Task, property3Task);
    }

    public static async Task<Person> GetPersonAsync(int id) {
        return null;
    }
}