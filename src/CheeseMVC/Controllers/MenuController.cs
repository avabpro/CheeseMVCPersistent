using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheeseMVC.Data;
using CheeseMVC.Models;
using CheeseMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CheeseMVC.Controllers
{
    public class MenuController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View(context.Menus.ToList());
        }

        private readonly CheeseDbContext context;

        public MenuController(CheeseDbContext dbContext)
        {
            context = dbContext;
        }

        public IActionResult Add()
        {
            AddMenuViewModel newAddMenuViewModel = new AddMenuViewModel();
            return View(newAddMenuViewModel);
        }

        [HttpPost]
        public IActionResult Add(AddMenuViewModel newAddMenuViewModel)
        {
            if (ModelState.IsValid)
            {
                Menu newMenu = new Menu
                {
                    Name = newAddMenuViewModel.Name
                };
                context.Menus.Add(newMenu);
                context.SaveChanges();
                return Redirect("/Menu/ViewMenu/" + newMenu.ID);
            }
            return View(newAddMenuViewModel);
        }
        public IActionResult ViewMenu(int id)
        {
            // TODO: use single method on dbSet?
            Menu menu = context.Menus.Find(id);
            List<CheeseMenu> items = context.CheeseMenus.Include(item => item.Cheese).Where(cm => cm.MenuID == id).ToList();
            ViewMenuViewModel newViewMenuViewModel = new ViewMenuViewModel
            {
                Menu = menu, 
                Items = items
            };
            return View(newViewMenuViewModel);
        }

        public IActionResult AddItem(int id)
        {
            Menu menu = context.Menus.Find(id);
            IEnumerable<Cheese> cheeseList = context.Cheeses;

            AddMenuItemViewModel newAddMenuItemViewModel = new AddMenuItemViewModel(menu, cheeseList);
            return View(newAddMenuItemViewModel);
        }

        [HttpPost]
        public IActionResult AddItem(AddMenuItemViewModel newAddMenuItemViewModel)
        {
            if (ModelState.IsValid)
            {
                IList<CheeseMenu> existingItems = context.CheeseMenus.Where(cm => cm.CheeseID == newAddMenuItemViewModel.CheeseID)
                    .Where(cm => cm.MenuID == newAddMenuItemViewModel.MenuID).ToList();
                if (!existingItems.Any())
                {
                    CheeseMenu cheeseMenu = new CheeseMenu
                    {
                        CheeseID = newAddMenuItemViewModel.CheeseID,
                        MenuID = newAddMenuItemViewModel.MenuID
                    };
                    context.CheeseMenus.Add(cheeseMenu);
                    context.SaveChanges();
                }
                return Redirect("/Menu/ViewMenu/" + newAddMenuItemViewModel.MenuID);
            }
            return View(newAddMenuItemViewModel);
        }
    }
}
