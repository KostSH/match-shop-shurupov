using Microsoft.EntityFrameworkCore;

Console.WriteLine("Магазин спичек «Чирк»: проект запущен!");

// Дальше — код по шагам задания.
using var db = new ShopContext();
db.Database.EnsureDeleted();   // удалить базу, если она осталась с прошлого запуска
db.Database.EnsureCreated();   // создать базу и таблицы по классам
Console.WriteLine("=== Шаг 3. Заполнение ===");
// Покупатели
var ivan = new Customer
{
    Name = "Иван Петров",
    Phone = "+7 900 111-22-33",
    City = "Омск"
};
var olga = new Customer
{
    Name = "Ольга Смирнова",
    Phone = "+7 900 444-55-66",
    City = "Томск"
};
var anna = new Customer
{
    Name = "Анна Кузнецова",
    Phone = "+7 901 123-45-67",
    City = "Новосибирск"
};
var dmitriy = new Customer
{
    Name = "Дмитрий Орлов",
    Phone = "+7 902 222-33-44",
    City = "Омск"
};
var maria = new Customer
{
    Name = "Мария Соколова",
    Phone = "+7 903 555-66-77",
    City = "Тюмень"
};
db.Customers.Add(ivan);
db.Customers.Add(olga);
db.Customers.Add(anna);
db.Customers.Add(dmitriy);
db.Customers.Add(maria);
// Товары
var kaminnye = new Product
{
    Name = "Каминные",
    Price = 150m,
    MatchesCount = 40,
    Stock = 20,
    Description = "Длинные спички для камина"
};
var ohotnichyi = new Product
{
    Name = "Охотничьи",
    Price = 95m,
    MatchesCount = 30,
    Stock = 15,
    Description = "Горят даже при ветре"
};
var pohodnye = new Product
{
    Name = "Походные",
    Price = 80m,
    MatchesCount = 25,
    Stock = 8,
    Description = null
};
var dekorativnye = new Product
{
    Name = "Декоративные",
    Price = 120m,
    MatchesCount = 50,
    Stock = 12,
    Description = "Красивые коробки"
};
var obychnye = new Product
{
    Name = "Обычные",
    Price = 45m,
    MatchesCount = 38,
    Stock = 100,
    Description = null
};
db.Products.Add(kaminnye);
db.Products.Add(ohotnichyi);
db.Products.Add(pohodnye);
db.Products.Add(dekorativnye);
db.Products.Add(obychnye);
db.SaveChanges();   // сначала сохраняю покупателей и товары
Console.WriteLine("Покупатели и товары добавлены.");
// Заказы
var order1 = new Order
{
    Customer = ivan,
    Product = kaminnye,
    Quantity = 3,
    CreatedAt = DateTime.UtcNow.AddDays(-7)
};
var order2 = new Order
{
    Customer = ivan,
    Product = ohotnichyi,
    Quantity = 2,
    CreatedAt = DateTime.UtcNow.AddDays(-6)
};
var order3 = new Order
{
    Customer = anna,
    Product = obychnye,
    Quantity = 5,
    CreatedAt = DateTime.UtcNow.AddDays(-5)
};
var order4 = new Order
{
    Customer = dmitriy,
    Product = dekorativnye,
    Quantity = 1,
    CreatedAt = DateTime.UtcNow.AddDays(-4)
};
// Я юзнул DateTime.UtcNow, потошо с DateTime.Now иногда бывает ошибка из-за локального времени.
db.Orders.Add(order1);
db.Orders.Add(order2);
db.Orders.Add(order3);
db.Orders.Add(order4);
db.SaveChanges();
Console.WriteLine("Заказы добавлены.");
Console.WriteLine("\n=== Шаг 4. Выборка ===");
// 1. Спички дешевле 100 рублей
Console.WriteLine("Спички дешевле 100 рублей:");
var cheapProducts = db.Products
    .Where(p => p.Price < 100)
    .OrderBy(p => p.Price)
    .ToList();
foreach (var product in cheapProducts)
{
    Console.WriteLine($"{product.Name} - {product.Price} ₽");
}
// 2. Сколько видов спичек есть на складе
var inStockCount = db.Products.Count(p => p.Stock > 0);
Console.WriteLine($"\nКоличество товаров в наличии: {inStockCount}");
// 3. Три самых дорогих товара
Console.WriteLine("\nТри самых дорогих товара:");
var expensiveProducts = db.Products
    .OrderByDescending(p => p.Price)
    .Take(3)
    .ToList();
foreach (var product in expensiveProducts)
{
    Console.WriteLine($"{product.Name} - {product.Price} ₽");
}
// Все заказы и общая выручка
Console.WriteLine("\nВсе заказы:");
var allOrders = db.Orders
    .Include(o => o.Customer)
    .Include(o => o.Product)
    .ToList();
decimal totalRevenue = 0;
foreach (var order in allOrders)
{
    decimal orderSum = order.Product.Price * order.Quantity;
    totalRevenue += orderSum;
    Console.WriteLine($"{order.Customer.Name} — {order.Product.Name} — {order.Quantity} шт. — {orderSum} ₽");
}
Console.WriteLine($"\nОбщая выручка: {totalRevenue} ₽");
Console.WriteLine("\n=== Шаг 5. Изменение ===");
// 1. Иван сменил номер телефона
var customer = db.Customers.First(c => c.Name == "Иван Петров");
customer.Phone = "+7 900 999-00-00";
// 2. Делаю все товары дороже на 10%
var allProducts = db.Products.ToList();
foreach (var product in allProducts)
{
    product.Price = Math.Round(product.Price * 1.1m, 2);
}
// 3. Один товар нема
var finishedProduct = db.Products.First(p => p.Name == "Походные");
finishedProduct.Stock = 0;
// Одного SaveChanges в конце норм
db.SaveChanges();
Console.WriteLine("Изменения сохранены.");
// Функция оформления заказа
// Удачный случай
var mariaForOrder = db.Customers.First(c => c.Name == "Мария Соколова");
var simpleProduct = db.Products.First(p => p.Name == "Обычные");
CreateOrder(db, mariaForOrder, simpleProduct, 3);
// Неудачный случай
var dmitriyForOrder = db.Customers.First(c => c.Name == "Дмитрий Орлов");
var expensiveProduct = db.Products.First(p => p.Name == "Каминные");
CreateOrder(db, dmitriyForOrder, expensiveProduct, 1000);
Console.WriteLine("\n=== Шаг 6. Удаление ===");
// Удалить покупателя без заказов
var leaving = db.Customers.First(c => c.Name == "Ольга Смирнова");
db.Customers.Remove(leaving);
db.SaveChanges();
Console.WriteLine("Ольга Смирнова удалена.");
// Удалить товары, которых нет на складе
var zeroStockProducts = db.Products
    .Where(p => p.Stock == 0)
    .ToList();
db.Products.RemoveRange(zeroStockProducts);
db.SaveChanges();
Console.WriteLine($"Удалено товаров без остатка: {zeroStockProducts.Count}");
var productsLeft = db.Products.Count();
Console.WriteLine($"Товаров осталось: {productsLeft}");
// Удаление покупателя, у которого есть заказы
var customerWithOrders = db.Customers
    .Include(c => c.Orders)
    .First(c => c.Name == "Иван Петров");
int ordersBeforeDelete = db.Orders.Count(o => o.CustomerId == customerWithOrders.Id);
db.Customers.Remove(customerWithOrders);
db.SaveChanges();
int ordersAfterDelete = db.Orders.Count(o => o.CustomerId == customerWithOrders.Id);
Console.WriteLine($"Заказов у покупателя до удаления: {ordersBeforeDelete}");
Console.WriteLine($"Заказов у покупателя после удаления: {ordersAfterDelete}");
// Вместе с покупателем делитнулись и его заказы из-за каскадного удаления. Для магазина это плохо, ведь теряется история заказов.
void CreateOrder(ShopContext db, Customer customer, Product product, int quantity)
{
    Console.WriteLine($"\nПробуем оформить заказ: {customer.Name} -> {product.Name}, количество: {quantity}");
    if (product.Stock < quantity)
    {
        Console.WriteLine("Недостаточно на складе");
        return;
    }
    var order = new Order
    {
        Customer = customer,
        Product = product,
        Quantity = quantity,
        CreatedAt = DateTime.UtcNow
    };
    product.Stock -= quantity;
    db.Orders.Add(order);
    // SaveChanges, шобыы и заказ, и новый остаток сейвились вместе.
    db.SaveChanges();
    Console.WriteLine("Заказ оформлен");
    Console.WriteLine($"Новый остаток товара {product.Name}: {product.Stock}");
}