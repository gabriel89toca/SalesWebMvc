using Microsoft.AspNetCore.Mvc;
using SalesWebMvc.Services;
using SalesWebMvc.Models;
using SalesWebMvc.Models.ViewModels;
using System.Collections.Generic;
using SalesWebMvc.Services.Exceptions;
using System.Diagnostics;
using System.Threading.Tasks;

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

        public async Task<IActionResult> Index()
        {
            var list = await _sellerService.FindAllAsync();

            return View(list);
        }

        public async Task<IActionResult> Create()
        {
            var departments = await _departmentService.FindAllAsync();
            var viewModel = new SellerFormViewModel { Departments = departments };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Evitar ataques CSFR 
        public async Task<IActionResult> Create(Seller seller)
        {

            if(!ModelState.IsValid)
            {
                var departments = await _departmentService.FindAllAsync();
                var viewModel = new SellerFormViewModel { Seller = seller, Departments = departments }; 
                return View(viewModel);
            }
            await _sellerService.InsertAsync(seller);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { Message = "Id não informado."});
            }

            var obj = await _sellerService.FindByIdAsync(id.Value);

            if (obj == null)
            {
                return RedirectToAction(nameof(Error), new { Message = "Vendedor não existe" }); ;
            }

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _sellerService.RemoveAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { Message = "Id não informado." });
            }

            var obj = await _sellerService.FindByIdAsync(id.Value);

            if (obj == null)
            {
                return RedirectToAction(nameof(Error), new { Message = "Vendedor não encontrado." });
            }

            return View(obj);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { Message = "Id não informado." });
            }

            var obj = await _sellerService.FindByIdAsync(id.Value);

            if (obj == null)
            {
                return RedirectToAction(nameof(Error), new { Message = "Vendedor não encontrado." });
            }

            
            List<Department> departments = await _departmentService.FindAllAsync();
            var viewModel = new SellerFormViewModel { Departments = departments, Seller = obj };
            return View(viewModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id,Seller seller)
        {
            if (id != seller.Id) 
            {
                return RedirectToAction(nameof(Error), new { Message = "Id não corresponde ao Vendedor informado." });
            }
            try
            {
                await _sellerService.UpdateAsync(seller);
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





        public async Task<IActionResult> UpdateAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obj = await _sellerService.FindByIdAsync(id.Value);

            if (obj == null)
            {
                return NotFound();
            }

            //return View(obj);
        
            List<Department> departments = await _departmentService.FindAllAsync();
            var viewModel = new SellerFormViewModel { Departments = departments, Seller = obj };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Seller seller)
        {
            await _sellerService.UpdateAsync(seller);
            return RedirectToAction(nameof(Index));
        }

    }
}
