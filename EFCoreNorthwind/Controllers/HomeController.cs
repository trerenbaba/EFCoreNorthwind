using EFCoreNorthwind.Data;
using EFCoreNorthwind.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreNorthwind.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NORTHWNDContext _db;
        public HomeController(ILogger<HomeController> logger, NORTHWNDContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            //en çok sipariş verilen 5 adet ürün çekelim ve kaç adet sipariş verildiğini gösterelim.
            var pro1 = _db.Orders.Include(x => x.OrderDetails).ThenInclude(p => p.Product).SelectMany(u => u.OrderDetails).GroupBy(c => c.Product.ProductName).Select(y => new
            {
                ProductName = y.Key,
                Quantity = y.Sum(z => z.Quantity)
            }).OrderByDescending(c => c.Quantity).Take(5).ToList();
            /*
             * 
             * select SUM(od.Quantity) 'adet',od.ProductID , c.CustomerID
                from Orders o inner join 
                [Order Details] od on o.OrderID = od.OrderID 
                inner join Products p on p.ProductID = od.ProductID 
                inner  join Customers c on c.CustomerID = o.CustomerID
                 group by  od.ProductID, c.CustomerID
             * 
             * 
             */


            //hangi kategoride kaç adet ürün var
            var query2 = _db.Categories.Include(x => x.Products).Select(a => new
            {
                CategoryName = a.CategoryName,
                TotalProduct = a.Products.Count()
            });
            //hangi ülkede kaç çalışanım var
            var query3 = _db.Employees.GroupBy(x => x.Country).Select(y => new
            {
                Count = y.Count(),
                Country = y.Key
            }).ToList();

            //tüm ürünlerin maliyeti ne kadar
            var query4 = _db.Products.Sum(x => x.UnitPrice * x.UnitsInStock);

            //şimdiye kadar ne kadar ciro yaptık
            var query5 = _db.Orders.Include(x => x.OrderDetails).SelectMany(a => a.OrderDetails).Sum(x => x.Quantity * x.UnitPrice * (decimal)(1 - x.Discount));

            //hangi müşteri hangi üründen kaç adet ürün sipariş etti
            var query6 = _db.Orders.Include(x => x.OrderDetails).ThenInclude(x => x.Product).Include(x => x.Customer).SelectMany(y => y.OrderDetails).GroupBy(y => new
            {
                y.Product.ProductName,
                y.Order.CustomerId
            }).Select(a => new
            {
                Product = a.Key.ProductName,
                Customer = a.Key.CustomerId,
                TotalProductQuantity = a.Sum(x => x.Quantity)
            }).OrderByDescending(x => x.TotalProductQuantity).ToList();

            //hangi müşteri kaç adet sipariş verdi
            var query61 = _db.Orders.Include(x => x.OrderDetails).ThenInclude(x => x.Product).Include(x => x.Customer).SelectMany(y => y.OrderDetails).GroupBy(y => y.Order.CustomerId).Select(a => new
            {
                Customer = a.Key,
                TotalProductQuantity = a.Sum(x => x.Quantity)
            }).OrderByDescending(x => x.TotalProductQuantity).ToList();



            //tost seven çalışanların sorgusu
            var q7 = _db.Employees.Where(x => EF.Functions.Like(x.Notes, "%toast%")).ToList();

            //fiyatı 50 liranın üstünde olan ürünleri fiyata göre artan azalana sıralayalım
            var q8 = _db.Products.Where(x => x.UnitPrice >= 50).OrderByDescending(x => x.UnitPrice).ToList();
            //rapor veren çalışanların listesi (yani bir müdürü bulunan çalışanlar)
            var q9 = _db.Employees.Where(x => x.ReportsTo != null).ToList();

            //bütün müdürleri bul
            var q10 = _db.Employees.Where(x => _db.Employees.Select(y => y.ReportsTo).Distinct().ToList().Contains(x.EmployeeId)).ToList();

            //hangi tedarikçi kaç adet ürün tedarik ediyor?

            //50 yaş üzerindeki çalışanların listesi

            //hangi çalışan kaç adet sipariş almış
            var pro12 = _db.Orders.Include(x => x.Employee).GroupBy(x =>new {x.Employee.FirstName,x.Employee.LastName }).Select(a => new
            {
                EmployeeName = a.FirstOrDefault().Employee.FirstName,
                Count = a.Count()
            }).ToList();

            //hangi ürün hangi kategoride hangi tedarikçi tarafından getirilmiştir.KateAdı,ürünadı,fiyatı,stoğu,tedariçi bilgileri ekrana getir
            var query13 = _db.Products.Include(x => x.Category).Include(x => x.Supplier).Select(a => new
            {
                ProductName = a.ProductName,
                CategoryName = a.Category.CategoryName,
                SupplierName = a.Supplier.ContactName,
                UnitPrice = a.UnitPrice,
                Stock = a.UnitsInStock
            }).AsNoTracking().ToList();

            //Not eğer soruglama operasyonları yapıyorsak EF Core sorgulanan nesnelerin her biri takip alıyor. buda performans kaybına yol açıyor. select işlemlerinde çok gereksiz bir durum. Sorgu performansını artırmak asNoTracking olarak işaretleyelim.

            //en çok adet ürün sipariş edilen siparişin toplam tutarını bulalım
            var query14 = _db.Orders.Include(x => x.OrderDetails).SelectMany(x => x.OrderDetails).GroupBy(x => x.OrderId).Select(a => new
            {
                OrderId = a.Key,
                OrderQuantity = a.Sum(x => x.Quantity),
                OrderTotalPrice = a.Sum(x => x.Quantity * x.UnitPrice * (decimal)(1 - x.Discount))
            }).OrderByDescending(x => x.OrderQuantity).Take(1).FirstOrDefault().OrderTotalPrice;

            //en çok sipariş edilen ilk 5 ürünün toplam tutarını hesaplayalım.ürün adı, toplam tutar şeklinde ekranda gösterilecek.

            var query15 = _db.Orders.Include(x => x.OrderDetails).ThenInclude(x => x.Product).SelectMany(x => x.OrderDetails).GroupBy(x => x.Product.ProductName).Select(a => new
            {
                OrderId = a.Key,
                ProductCount = a.Count(),
                ProductTotalPrice = a.Sum(x => x.Quantity * x.UnitPrice * (decimal)(1 - x.Discount))
            }).OrderByDescending(x => x.ProductCount).Take(5).Sum(x=>x.ProductTotalPrice);



            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
