using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.DAL;
using WebApplication1.Models;

namespace WebApplication1.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class TeamController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public TeamController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            List<Team> teammates = _context.Teammates.ToList();
            return View(teammates);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Team teammate)
        {
            if (teammate == null) return View();
            if (teammate.Photo != null)
            {
                string fileName = Guid.NewGuid().ToString() + teammate.Photo.FileName;
                using (FileStream fs = new FileStream(Path.Combine(_env.WebRootPath, "assets/images", fileName), FileMode.Create))
                {
                    teammate.Photo.CopyTo(fs);
                }
                teammate.PhotoUrl = fileName;
            }
            _context.Teammates.Add(teammate);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Edit(int id)
        {
            Team teammate = _context.Teammates.Find(id);
            if (teammate == null) return NotFound();
            return View(teammate);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Team teammate)
        {
            if (teammate == null) return View(teammate);
            Team oldTeammate = _context.Teammates.Find(teammate.Id);
            if (oldTeammate == null) return NotFound();
            if (teammate.Photo != null)
            {
                if (System.IO.File.Exists(Path.Combine(_env.WebRootPath, "assets/images", oldTeammate.PhotoUrl)))
                {
                    System.IO.File.Delete(Path.Combine(_env.WebRootPath, "assets/images", oldTeammate.PhotoUrl));
                }
                string fileName = Guid.NewGuid().ToString() + teammate.Photo.FileName;
                using (FileStream fs = new FileStream(Path.Combine(_env.WebRootPath, "assets/images", fileName), FileMode.Create))
                {
                    await teammate.Photo.CopyToAsync(fs);
                }
                oldTeammate.PhotoUrl = fileName;
            }
            oldTeammate.FullName = teammate.FullName;
            oldTeammate.Role = teammate.Role;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            Team teammate = _context.Teammates.Find(id);
            if (teammate == null) return NotFound();
            if (teammate.PhotoUrl != null)
            {
                if (System.IO.File.Exists(Path.Combine(_env.WebRootPath, "assets/images", teammate.PhotoUrl)))
                {
                    System.IO.File.Delete(Path.Combine(_env.WebRootPath, "assets/images", teammate.PhotoUrl));
                }
            }

            _context.Teammates.Remove(teammate);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
