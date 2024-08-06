using Microsoft.AspNetCore.Mvc;
using SalesWebMvc.Services;
using SalesWebMvc.Models;
using SalesWebMvc.Models.ViewModels;
using System.Collections.Generic;
using SalesWebMvc.Services.Exceptions;
using System.Diagnostics;

namespace SalesWebMvc.Controllers
{
    public class SellersController : Controller
    {
        private readonly SellerService _sellerService;
        private readonly DepartmentService _departmentService;

        public SellersController(SellerService sellerService, DepartmentService departmentService)
        {
            _sellerService = sellerService;
            _departmentService = departmentService;
        }

        public IActionResult Index()
        {
            var list = _sellerService.FindAll();

            return View(list);
        }

        public IActionResult Create()
        {
            var departments = _departmentService.FindAll();
            var viewModel = new SellerFormViewModel { Departments = departments };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Evitar ataques CSFR 
        public IActionResult Create(Seller seller)
        {
            _sellerService.Insert(seller);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { Message = "Id não informado."});
            }

            var obj = _sellerService.FindById(id.Value);

            if (obj == null)
            {
                return RedirectToAction(nameof(Error), new { Message = "Vendedor não existe" }); ;
            }

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _sellerService.Remove(id);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { Message = "Id não informado." });
            }

            var obj = _sellerService.FindById(id.Value);

            if (obj == null)
            {
                return RedirectToAction(nameof(Error), new { Message = "Vendedor não encontrado." });
            }

            return View(obj);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { Message = "Id não informado." });
            }

            var obj = _sellerService.FindById(id.Value);

            if (obj == null)
            {
                return RedirectToAction(nameof(Error), new { Message = "Vendedor não encontrado." });
            }

            
            List<Department> departments = _departmentService.FindAll();
            var viewModel = new SellerFormViewModel { Departments = departments, Seller = obj };
            return View(viewModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int? id,Seller seller)
        {
            if (id != seller.Id) 
            {
                return RedirectToAction(nameof(Error), new { Message = "Id não corresponde ao Vendedor informado." });
            }
            try
            {
                _sellerService.Update(seller);
                return RedirectToAction(nameof(Index));
            }
            catch (NotFoundException e)
            {
                return RedirectToAction(nameof(Error), new { Message = e.Message});
            }
            catch (DbConcurrencyException e)
            {
                return RedirectToAction(nameof(Error), new { Message = e.Message });
            }

            
        }

        public IActionResult Error(string message)
        {
            var viewModel = new ErrorViewModel
            {
                Message = message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };
            return View(viewModel);
        }





        public IActionResult Update(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obj = _sellerService.FindById(id.Value);

            if (obj == null)
            {
                return NotFound();
            }

            //return View(obj);
        
            List<Department> departments = _departmentService.FindAll();
            var viewModel = new SellerFormViewModel { Departments = departments, Seller = obj };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Seller seller)
        {
            _sellerService.Update(seller);
            return RedirectToAction(nameof(Index));
        }

    }
}
