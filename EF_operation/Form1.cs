using Microsoft.EntityFrameworkCore;
using Smart_Mart;

namespace EF_operation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            SeedDummyData();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void SeedDummyData()
        {
            try
            {
                using SmartMartContext ctx = new SmartMartContext();

                ctx.Database.EnsureCreated();


                if (ctx.Customers.Any() || ctx.Products.Any() || ctx.SalesRecords.Any())
                    return;

                var customers = new[]
                {
                    new Customer { Name = "Alice", City = "Seattle" },
                    new Customer { Name = "Bob",   City = "Chicago" },
                    new Customer { Name = "Carol", City = "Austin" },
                    new Customer { Name = "David", City = "Miami" },
                    new Customer { Name = "Eve", City = "New York" },
                    new Customer { Name = "Frank", City = "Los Angeles" } // This customer will have no purchases
                };
                ctx.Customers.AddRange(customers);

                var products = new[]
                {
                    new Product { ProductName = "Apple", Category = "Fruit" },
                    new Product { ProductName = "Milk",  Category = "Dairy" },
                    new Product { ProductName = "Bread", Category = "Bakery" },
                    new Product { ProductName = "Cheese", Category = "Dairy" },
                    new Product { ProductName = "Orange", Category = "Fruit" },
                    new Product { ProductName = "Yogurt", Category = "Dairy" }
                };
                ctx.Products.AddRange(products);

                ctx.SaveChanges();

                var sales = new[]
                {
                  
                    new SalesRecord { CustomerId = customers[0].CustomerId, ProductId = products[0].ProductId, SaleDate = DateTime.Now.AddDays(-5) }, // Alice buys Apple
                    new SalesRecord { CustomerId = customers[1].CustomerId, ProductId = products[1].ProductId, SaleDate = DateTime.Now.AddDays(-4) }, // Bob buys Milk
                    new SalesRecord { CustomerId = customers[2].CustomerId, ProductId = products[2].ProductId, SaleDate = DateTime.Now.AddDays(-3) }, // Carol buys Bread
                    new SalesRecord { CustomerId = customers[0].CustomerId, ProductId = products[3].ProductId, SaleDate = DateTime.Now.AddDays(-2) }, // Alice buys Cheese
                    new SalesRecord { CustomerId = customers[0].CustomerId, ProductId = products[4].ProductId, SaleDate = DateTime.Now.AddDays(-2) }, // Alice buys Orange
                    new SalesRecord { CustomerId = customers[2].CustomerId, ProductId = products[5].ProductId, SaleDate = DateTime.Now.AddDays(-1) }, // Carol buys Yogurt
                    new SalesRecord { CustomerId = customers[3].CustomerId, ProductId = products[1].ProductId, SaleDate = DateTime.Now.AddDays(-1) }, // David buys Milk
                    new SalesRecord { CustomerId = customers[4].CustomerId, ProductId = products[0].ProductId, SaleDate = DateTime.Now }              // Eve buys Apple
                };
                ctx.SalesRecords.AddRange(sales);
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Seeding failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }





        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using SmartMartContext ctx = new SmartMartContext();

                var customers = await ctx.Customers
                    .AsNoTracking()
                    .Select(c => new { c.CustomerId, c.Name, c.City })
                    .ToListAsync();

                dataGridView1.DataSource = customers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load customers: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void button6_Click(object sender, EventArgs e)
        {
            try
            {
                var ctx = new SmartMartContext();
                var products = await ctx.Products
                    .AsNoTracking()
                    .Select(p => new { p.ProductId, p.ProductName, p.Category })
                    .ToListAsync();

                dataGridView1.DataSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load Products: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private async void button5_Click(object sender, EventArgs e)
        {
            try
            {
                using var ctx = new SmartMartContext();

                var result = await (
                    from c in ctx.Customers
                    join s in ctx.SalesRecords on c.CustomerId equals s.CustomerId into salesGroup
                    from s in salesGroup.DefaultIfEmpty()
                    where s == null
                    select new
                    {
                        c.CustomerId,
                        c.Name,
                        c.City,
                        ProductName = "No Purchase"
                    })
                    .AsNoTracking()
                    .ToListAsync();

                dataGridView1.DataSource = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load customers with no purchases: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            try
            {
                using var ctx = new SmartMartContext();

                var result = await (
                    from c in ctx.Customers
                    join s in ctx.SalesRecords on c.CustomerId equals s.CustomerId into salesGroup
                    select new
                    {
                        CustomerName = c.Name ?? string.Empty,
                        TotalPurchasedProducts = salesGroup.Count()
                    })
                    .AsNoTracking()
                    .ToListAsync();

                dataGridView1.DataSource = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load purchase totals: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            try
            {
                using var ctx = new SmartMartContext();

                var result = await(
                    from p in ctx.Products
                    join s in ctx.SalesRecords on p.ProductId equals s.ProductId
                    group p by p.Category into g
                    select new
                    {
                        Category = g.Key ?? string.Empty,
                        TotalSales = g.Count()
                    })
                    .AsNoTracking()
                    .ToListAsync();

                dataGridView1.DataSource = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load category sales: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }


}
