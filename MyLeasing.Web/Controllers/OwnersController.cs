using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLeasing.Web.Data;
using MyLeasing.Web.Data.Entities;
using MyLeasing.Web.Helpers;
using MyLeasing.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyLeasing.Web.Controllers
{
    [Authorize(Roles = "Manager")]
    public class OwnersController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ICombosHelpers _combosHelpers;
        private readonly IConverterHelper _converterHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;
        public OwnersController(DataContext dataContext,
            IUserHelper userHelper,
            ICombosHelpers combosHelpers,
            IConverterHelper converterHelper,
            IImageHelper imageHelper,
            IMailHelper mailHelper)
        {
            _dataContext = dataContext;
            _userHelper = userHelper;
            _combosHelpers = combosHelpers;
            _converterHelper = converterHelper;
            _imageHelper = imageHelper;
            _mailHelper = mailHelper;
        }

        [HttpGet]
        public async Task<IActionResult> AddProperty(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Owner owner = await _dataContext.Owners.FindAsync(id.Value);

            if (owner == null)
            {
                return NotFound();
            }
            else
            {
                PropertyViewModel view = new PropertyViewModel
                {
                    OwnerId = owner.Id,
                    PropertyTypes = _combosHelpers.GetComboPropertyTypes(),
                };

                return View(view);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddProperty(PropertyViewModel model)
        {
            if (ModelState.IsValid)
            {
                Property property = await _converterHelper.toPropertyAsync(model, true);

                _dataContext.Properties.Add(property);
                await _dataContext.SaveChangesAsync();
                return RedirectToAction("Details", new { id = model.OwnerId });
            }

            model.PropertyTypes = _combosHelpers.GetComboPropertyTypes();
            return View(model);
        }

        // GET: Owners
        public IActionResult Index()
        {
            return View(_dataContext.Owners.
                Include(o => o.User).
                Include(p => p.Properties).
                Include(c => c.Contracts));
        }

        // GET: Owners/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Owner owner = await _dataContext.Owners
                .Include(o => o.User)
                .Include(p => p.Properties)
                .ThenInclude(p => p.PropertyImages)
                .Include(p => p.Properties)
                .ThenInclude(p => p.PropertyType)
                .Include(o => o.Contracts)
                .ThenInclude(o => o.Lessee)
                .ThenInclude(l => l.User)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var view = new AddUserViewModel { RoleId = 2 };
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await CreateUserAsync(model);

                if (user != null)
                {
                    Owner owner = new Owner
                    {
                        Properties = new List<Property>(),
                        Contracts  = new List<Contract>(),
                        User = user
                    };

                    _dataContext.Owners.Add(owner);
                    await _dataContext.SaveChangesAsync();

                    var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                    var tokenLink = Url.Action("ConfirmEmail", "Account", new
                    {
                        userid = user.Id,
                        token = myToken
                    }, protocol: HttpContext.Request.Scheme);

                    _mailHelper.SendMail(model.Username, "Email confirmation", $"<h1>Email Confirmation</h1>" +
                        $"To allow the user, " +
                        $"plase click in this link:</br></br><a href = \"{tokenLink}\">Confirm Email</a>");

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User with this email is already exist !.");
                }
            }

            return View(model);
        }

        private async Task<User> CreateUserAsync(AddUserViewModel model)
        {
            User user = new User
            {
                Address     = model.Address,
                Document    = model.Document,
                Email       = model.Username,
                FirstName   = model.FirstName,
                LastName    = model.LastName,
                PhoneNumber = model.PhoneNumber,
                UserName    = model.Username,
            };

            IdentityResult result = await _userHelper.AddUserAsync(user, model.Password);

            if (result != IdentityResult.Success)
            {
                return null;
            }
            else
            {
                User newUser = await _userHelper.GetUserByEmailAsync(model.Username);
                await _userHelper.AddUserToRoleAsync(newUser, "Owner");
                return newUser;
            }
        }

        // GET: Owners/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Owner owner = await _dataContext.Owners
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id.Value);
            if (owner == null)
            {
                return NotFound();
            }

            EditUserViewModel view = new EditUserViewModel
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
                Owner owner = await _dataContext.Owners
                    .Include(o => o.User)
                    .FirstOrDefaultAsync(o => o.Id == view.Id);

                owner.User.Address = view.Address;
                owner.User.Document = view.Document;
                owner.User.FirstName = view.FirstName;
                owner.User.LastName = view.LastName;
                owner.User.PhoneNumber = view.PhoneNumber;

                await _userHelper.UpdateUserAsync(owner.User);
                return RedirectToAction(nameof(Index));
            }

            return View(view);
        }


        // GET: Owners/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Owner owner = await _dataContext.Owners
                .Include(o => o.User)
                .Include(o => o.Properties)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (owner == null)
            {
                return NotFound();
            }

            if (owner.Properties.Count != 0)
            {

                ModelState.AddModelError(string.Empty, "Owner can´t be delete beacause it has properties");
                return RedirectToAction(nameof(Index));
            }

            _dataContext.Owners.Remove(owner);
            await _dataContext.SaveChangesAsync();
            await _userHelper.DeleteUserAsync(owner.User.Email);
            return RedirectToAction(nameof(Index));
        }

        private bool OwnerExists(int id)
        {
            return _dataContext.Owners.Any(e => e.Id == id);
        }

        [HttpGet]
        public async Task<IActionResult> EditProperty(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Property property = await _dataContext.Properties
                .Include(p => p.Owner)
                .Include(p => p.PropertyType)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (property == null)
            {
                return NotFound();
            }

            Property model = _converterHelper.ToPropertyViewModel(property);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProperty(PropertyViewModel model)
        {
            if (ModelState.IsValid)
            {
                Property property = await _converterHelper.toPropertyAsync(model, false);

                _dataContext.Properties.Update(property);
                await _dataContext.SaveChangesAsync();
                return RedirectToAction("Details", new { id = model.OwnerId });
            }
            return View(model);
        }

        public async Task<IActionResult> DetailsProperty(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Property property = await _dataContext.Properties
                                            .Include(o => o.Owner)
                                            .ThenInclude(o => o.User)
                                            .Include(o => o.Contracts)
                                            .ThenInclude(c => c.Lessee)
                                            .ThenInclude(l => l.User)
                                            .Include(o => o.PropertyType)
                                            .Include(p => p.PropertyImages)
                                            .FirstOrDefaultAsync(m => m.Id == id);

            if (property == null)
            {
                return NotFound();
            }

            return View(property);
        }

        public async Task<IActionResult> AddImage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Property property = await _dataContext.Properties.FindAsync(id.Value);
            if (property == null)
            {
                return NotFound();
            }

            PropertyImageViewModel model = new PropertyImageViewModel
            {
                Id = property.Id
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddImage(PropertyImageViewModel model)
        {
            if (ModelState.IsValid)
            {
                string path = string.Empty;

                if (model.ImageFile != null)
                {
                    path = await _imageHelper.UploadImageAsync(model.ImageFile);
                }

                PropertyImage propertyImage = new PropertyImage
                {
                    ImageUrl = path,
                    Property = await _dataContext.Properties.FindAsync(model.Id)
                };

                _dataContext.PropertyImages.Add(propertyImage);
                await _dataContext.SaveChangesAsync();
                return RedirectToAction("DetailsProperty", new { id = model.Id });
                //return RedirectToAction($"{nameof(DetailsProperty)}/{model.Id}");
            }

            return View(model);
        }

        public async Task<IActionResult> AddContract(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Property property = await _dataContext.Properties
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.Id == id.Value);

            if (property == null)
            {
                return NotFound();
            }

            ContractViewModel model = new ContractViewModel
            {
                OwnerId = property.Owner.Id,
                PropertyId = property.Id,
                Lessees = _combosHelpers.GetComboLessees(),
                Price = property.Price,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddYears(1)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddContract(ContractViewModel model)
        {
            if (ModelState.IsValid)
            {
                Contract contract = await _converterHelper.ToContractAsync(model, true);
                _dataContext.Contracts.Add(contract);
                await _dataContext.SaveChangesAsync();
                //return RedirectToAction($"{nameof(DetailsProperty)}/{model.OwnerId}");
                return RedirectToAction("DetailsProperty", new { id = model.PropertyId });
            }

            model.Lessees = _combosHelpers.GetComboLessees();
            return View(model);
        }

        public async Task<IActionResult> EditContract(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Contract contract = await _dataContext.Contracts
                .Include(p => p.Owner)
                .Include(p => p.Lessee)
                .Include(p => p.Property)
                .FirstOrDefaultAsync(p => p.Id == id.Value);

            if (contract == null)
            {
                return NotFound();
            }

            ContractViewModel view = _converterHelper.ToContractViewModel(contract);
            return View(view);
        }

        [HttpPost]
        public async Task<IActionResult> EditContract(ContractViewModel model)
        {
            if (ModelState.IsValid)
            {
                Contract contract = await _converterHelper.ToContractAsync(model, false);
                _dataContext.Contracts.Update(contract);
                await _dataContext.SaveChangesAsync();
                return RedirectToAction("DetailsProperty", new { id = model.PropertyId });
            }

            return View(model);
        }

        public async Task<IActionResult> DeleteImage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            PropertyImage propertyImage = await _dataContext.PropertyImages
                .Include(pi => pi.Property)
                .FirstOrDefaultAsync(pi => pi.Id == id.Value);

            if (propertyImage == null)
            {
                return NotFound();
            }

            _dataContext.PropertyImages.Remove(propertyImage);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction("DetailsProperty", new { id = propertyImage.Property.Id });
        }

        public async Task<IActionResult> DeleteContract(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Contract contract = await _dataContext.Contracts
                .Include(c => c.Property)
                .FirstOrDefaultAsync(c => c.Id == id.Value);

            if (contract == null)
            {
                return NotFound();
            }

            _dataContext.Contracts.Remove(contract);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction("DetailsProperty", new { id = contract.Property.Id });
        }

        public async Task<IActionResult> DeleteProperty(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Property property = await _dataContext.Properties
                                .Include(p => p.Owner)
                                .Include(p => p.PropertyImages)
                                .Include(p => p.Contracts)
                                .FirstOrDefaultAsync(pi => pi.Id == id.Value);

            if (property == null)
            {
                return NotFound();
            }

            if (property.Contracts.Count != 0)
            {
                ModelState.AddModelError(string.Empty, "This property cannot be deleted because it has contracts");
                return RedirectToAction($"{nameof(Details)}/{property.Owner.Id}");
            }

            _dataContext.PropertyImages.RemoveRange(property.PropertyImages);
            _dataContext.Contracts.RemoveRange(property.Contracts);
            _dataContext.Properties.Remove(property);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction($"{nameof(Details)}/{property.Owner.Id}");
        }

        public async Task<IActionResult> DetailsContract(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Contract contract = await _dataContext.Contracts
                .Include(c => c.Owner)
                .ThenInclude(o => o.User)
                .Include(c => c.Lessee)
                .ThenInclude(o => o.User)
                .Include(c => c.Property)
                .ThenInclude(p => p.PropertyType)
                .FirstOrDefaultAsync(pi => pi.Id == id.Value);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }
    }
}