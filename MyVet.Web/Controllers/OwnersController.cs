using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyVet.Web.Data;
using MyVet.Web.Data.Entities;
using MyVet.Web.Helpers;
using MyVet.Web.Models;

namespace MyVet.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OwnersController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IImageHelper _imageHelper;

        public OwnersController(
            DataContext context,
            IUserHelper userHelper,
            ICombosHelper combosHelper,
            IConverterHelper converterHelper,
            IImageHelper imageHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _combosHelper = combosHelper;
            _converterHelper = converterHelper;
            _imageHelper = imageHelper;
        }

        // GET: Owners
        public IActionResult Index()
        {
            return View(_context.Owners
                .Include(o => o.User)
                .Include(p => p.Pets));
        }

        // GET: Owners/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _context.Owners
                .Include(o => o.User)
                .Include(o => o.Pets)
                .ThenInclude(o => o.PetType)
                .Include(o => o.Pets)
                .ThenInclude(o => o.Histories)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }

        // GET: Owners/Create
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var usuario = new User
                {
                    Address = model.Address,
                    Document = model.Document,
                    Email = model.Username,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    UserName = model.Username
                };
                var response = await _userHelper.AddUserAsync(usuario, model.Password);
                // si ya existe  no se crea y hay q validar....
                if (response.Succeeded)
                {
                    var userInDb = await _userHelper.GetUserByEmailAsync(model.Username);
                    await _userHelper.AddUserToRoleAsync(userInDb, "Customer");

                    var owner = new Owner
                    {
                        Agendas = new List<Agenda>(),
                        Pets = new List<Pet>(),
                        User = userInDb
                    };

                    _context.Owners.Add(owner);

                    try
                    {
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, ex.ToString());
                        return View(model);
                    }


                }
                ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault().Description);
            }

            return View(model);
        }

        public async Task<IActionResult> AddPet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _context.Owners.FindAsync(id.Value);

            if (owner == null)
            {
                return NotFound();
            }

            var petViewModel = new PetViewModel
            {
                Born = DateTime.Today,
                OwnerId = owner.Id,
                PetTypes = _combosHelper.GetComboPetTypes()
            };

            return View(petViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> AddPet(PetViewModel model)
        {
            if (ModelState.IsValid)
            {
                // asumo q no hay imagen
                var path = string.Empty;

                if (model.ImageFile != null)
                {
                    path = await _imageHelper.UpLoadImageAsync(model.ImageFile); 
                    
                }

                var pet = await _converterHelper.ToPetAsync(model, path,true);
                _context.Pets.Add(pet);
                await _context.SaveChangesAsync();
                return RedirectToAction($"Details/{model.OwnerId}");
            }
            model.PetTypes = _combosHelper.GetComboPetTypes();                  
            return View(model);
        }

        public async Task<IActionResult> EditPet(int? id)
        {
            if (id == null)
            {
                return NotFound();

            }

            var pet = await _context.Pets
                .Include(p=>p.Owner)
                .Include(p=>p.PetType)
                .FirstOrDefaultAsync(p=>p.Id==id);
            if (pet == null)
            {
                return NotFound();
            }

            return View(_converterHelper.ToPetViewModel(pet));
        }

        [HttpPost]
        public async Task<IActionResult> EditPet(PetViewModel model)
        {
            if (ModelState.IsValid)
            {
                // asumo q no hay imagen
                var path = model.ImageUrl;

                if (model.ImageFile != null)
                {
                    path = await _imageHelper.UpLoadImageAsync(model.ImageFile);

                }

                var pet = await _converterHelper.ToPetAsync(model, path, false);
                _context.Pets.Update(pet);
                await _context.SaveChangesAsync();

                return RedirectToAction($"Details/{model.OwnerId}");

            }

            model.PetTypes = _combosHelper.GetComboPetTypes();
            return View(model);
        }


        public async Task<IActionResult> DetailsPet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _context.Pets
                .Include(p => p.Owner)
                .ThenInclude(o => o.User)
                .Include(p => p.Histories)
                .ThenInclude(h => h.ServiceType)
                .FirstOrDefaultAsync(o => o.Id == id.Value);
            //hay 2 formas de buscar :
            //    firstorDefault ---- esta es mas dolorosa pero es necesaria cuando necesitemos incluir mas relaciones
            // y  findasync ---- esta es mas rapida porque busca por clave primaria  pero no puedo incluir las relaciones.....ya que include =innerjoin

            if (pet == null)
            {
                return NotFound();
            }

            return View(pet);
        }


        public async Task<IActionResult> AddHistory(int? id)
        {
            //id de la pet
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _context.Pets.FindAsync(id.Value);
            if (pet == null)
            {
                return NotFound();
            }

            var model = new HistoryViewModel
            {
                Date = DateTime.Now,
                PetId = pet.Id,
                ServiceTypes = _combosHelper.GetComboServiceTypes(),
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddHistory(HistoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var history = await _converterHelper.ToHistoryAsync(model, true);
                _context.Histories.Add(history);
                await _context.SaveChangesAsync();
                return RedirectToAction($"{nameof(DetailsPet)}/{model.PetId}");
            }
            model.ServiceTypes = _combosHelper.GetComboServiceTypes();
            return View(model);
        }

        public async Task<IActionResult> EditHistory(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var history = await _context.Histories
                .Include(h => h.Pet)
                .Include(h => h.ServiceType)
                .FirstOrDefaultAsync(p => p.Id == id.Value);
            if (history == null)
            {
                return NotFound();
            }

            return View(_converterHelper.ToHistoryViewModel(history));
        }

        [HttpPost]
        public async Task<IActionResult> EditHistory(HistoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var history = await _converterHelper.ToHistoryAsync(model, false);
                _context.Histories.Update(history);
                await _context.SaveChangesAsync();
                return RedirectToAction($"{nameof(DetailsPet)}/{model.PetId}");
            }
            model.ServiceTypes = _combosHelper.GetComboServiceTypes();
            return View(model);
        }

        public async Task<IActionResult> DeleteHistory(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var history = await _context.Histories
                .Include(h => h.Pet)
                .FirstOrDefaultAsync(h => h.Id == id.Value);
            if (history == null)
            {
                return NotFound();
            }

            _context.Histories.Remove(history);
            await _context.SaveChangesAsync();
            return RedirectToAction($"{nameof(DetailsPet)}/{history.Pet.Id}");
        }

        public async Task<IActionResult> DeletePet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _context.Pets
                .Include(p => p.Owner)
                .Include(p=>p.Histories)
                .FirstOrDefaultAsync(p => p.Id == id.Value);
            if (pet == null)
            {
                return NotFound();
            }
            if (pet.Histories.Count>0)
            {
                ModelState.AddModelError(string.Empty, "The pet can't be deleted because it has related records.");
                return RedirectToAction($"{nameof(Details)}/{pet.Owner.Id}");
            }
            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();
            return RedirectToAction($"{nameof(DetailsPet)}/{pet.Owner.Id}");
        }

        

        //// POST: Owners/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id")] Owner owner)
        //{
        //    if (id != owner.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(owner);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!OwnerExists(owner.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(owner);
        //}

        // GET: Owners/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _context.Owners
                .Include(o=>o.Pets)
                .Include(o=>o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (owner == null)
            {
                return NotFound();
            }
            if (owner.Pets.Count > 0)
            {
                ModelState.AddModelError(string.Empty, "The owner can't be deleted because it has related records.");
                return RedirectToAction(nameof(Index));
            }

            await _userHelper.DeleteUserAsync(owner.User.Email);
            _context.Owners.Remove(owner);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _context.Owners
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id.Value);
            if (owner == null)
            {
                return NotFound();
            }

            var view = new EditUserViewModel
            {
                Address = owner.User.Address,
                Document = owner.User.Document,
                FirstName = owner.User.FirstName,
                Id = owner.Id,
                LastName = owner.User.LastName,
                PhoneNumber = owner.User.PhoneNumber
            };

            return View(view);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel view)
        {
            if (ModelState.IsValid)
            {
                var owner = await _context.Owners
                    .Include(o => o.User)
                    .FirstOrDefaultAsync(o => o.Id == view.Id);

                owner.User.Document = view.Document;
                owner.User.FirstName = view.FirstName;
                owner.User.LastName = view.LastName;
                owner.User.Address = view.Address;
                owner.User.PhoneNumber = view.PhoneNumber;

                await _userHelper.UpdateUserAsync(owner.User);
                return RedirectToAction(nameof(Index));
            }

            return View(view);
        }


        private bool OwnerExists(int id)
        {
            return _context.Owners.Any(e => e.Id == id);
        }
    }
}
