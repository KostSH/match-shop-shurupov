public class Customer
{
    public int Id { get; set; }              // первичный ключ: EF узнаёт его по имени Id
    public string Name { get; set; } = "";   // имя
    public string Phone { get; set; } = "";  // телефон
    public string City { get; set; } = "";   // город
    public List<Order> Orders { get; set; } = new();
}

public class Product
{
    public int Id { get; set; }                       // первичный ключ
    public string Name { get; set; } = "";           // обязательное поле
    public decimal Price { get; set; }               // цена
    public int MatchesCount { get; set; }            // сколько спичек в коробке
    public int Stock { get; set; }                   // остаток на складе
    public string? Description { get; set; }         // может быть пустым
    public List<Order> Orders { get; set; } = new();
    // Ответ на вопрос из задания - Name будет NOT NULL, потому что это string. Description может быть NULL, потому что это string?
}

public class Order
{
    public int Id { get; set; }
    public int Quantity { get; set; }                // сколько коробок
    public DateTime CreatedAt { get; set; }          // дата и время заказа
    public int CustomerId { get; set; }              // внешний ключ на покупателя
    public Customer Customer { get; set; } = null!;
    public int ProductId { get; set; }               // внешний ключ на товар
    public Product Product { get; set; } = null!;
    // Ответ: Я выбрал тип DateTime. В PostgreSQL обычно получается тип timestamp with time zone
}