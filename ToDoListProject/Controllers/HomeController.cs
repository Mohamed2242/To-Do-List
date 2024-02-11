using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ToDoListProject.Data;
using ToDoListProject.Models;

namespace ToDoListProject.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string id)
        {
            var filters = new Filters(id);
            ViewBag.Filters = filters;

            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Statuses = _context.Statuses.ToList();
            ViewBag.DueFilters = Filters.DueFilterValues;

            IQueryable<ToDo> query = _context.ToDos.Include(l => l.Category).Include(l => l.Status);

            if (filters.HasCategory)
            {
                query = query.Where(l => l.CategoryId == filters.CategoryId);
            }

            if (filters.HasStatus)
            {
                query = query.Where(l => l.StatusId == filters.StatusId);
            }

            if (filters.HasDue)
            {
                var today = DateTime.Today;
                if (filters.IsPast)
                {
                    query = query.Where(l => l.DueDate < today);
                }
                else if (filters.IsFuture)
                {
                    query = query.Where(l => l.DueDate > today);
                }
                else if (filters.IsToday)
                {
                    query = query.Where(l => l.DueDate == today);
                }
            }

            var toDos = query.OrderBy(l => l.DueDate).ToList();
            return View(toDos);
        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Statuses = _context.Statuses.ToList();
            var toDos = new ToDo { StatusId = "open" };

            return View();
        }


        [HttpPost]
        public IActionResult Add(ToDo toDos)
        {
            if (ModelState.IsValid)
            {
                _context.ToDos.Add(toDos);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Categories = _context.Categories.ToList();
                ViewBag.Statuses = _context.Statuses.ToList();
                return View(toDos);
            }
        }



		[HttpGet]
		public IActionResult Edit(int id)
		{
			ToDo toDo = _context.ToDos.FirstOrDefault(l => l.Id == id);

			if (toDo == null)
			{
				return NotFound();
			}
			else
			{
				ViewBag.Categories = new SelectList(_context.Categories.ToList(), "CategoryId", "Name");
				ViewBag.Statuses = new SelectList(_context.Statuses.ToList(), "StatusId", "Name");
				return View("Edit", toDo);
			}
		}


		[HttpPost]
		public IActionResult Edit(ToDo toDos)
		{
			if (ModelState.IsValid)
			{
				_context.ToDos.Update(toDos);
				_context.SaveChanges();
				return RedirectToAction("Index");
			}
			else
			{
				ViewBag.Categories = _context.Categories.ToList();
				ViewBag.Statuses = _context.Statuses.ToList();
				return View(toDos);
			}
		}



		[HttpPost]
        public IActionResult Filter(string[] filter)
        {
            string id = string.Join("-", filter);
            return RedirectToAction("Index", new { ID = id });
        }

        [HttpPost]
        public IActionResult MarkComplete([FromRoute] string id, ToDo selected)
        {
            selected = _context.ToDos.Find(selected.Id);

            if (selected != null)
            {
                selected.StatusId = "closed";
                _context.SaveChanges();
            }
            return RedirectToAction("Index", new { ID = id });
        }

        [HttpPost]
        public IActionResult DeleteComplete(string id)
        {
            var toDelete = _context.ToDos.Where(l => l.StatusId == "closed").ToList();

            foreach (var task in toDelete)
            {
                _context.ToDos.Remove(task);
            }
            _context.SaveChanges();

            return RedirectToAction("Index", new { ID = id });
        }
    }
}