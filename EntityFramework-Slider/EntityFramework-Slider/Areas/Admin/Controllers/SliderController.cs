﻿using EntityFramework_Slider.Data;
using EntityFramework_Slider.Helpers;
using EntityFramework_Slider.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SliderController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly AppDbContext _context;
        public SliderController(AppDbContext context,
                                IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {

           IEnumerable<Slider> sliders = await _context.Sliders.Where(m => !m.SoftDelete).ToListAsync();

           return View(sliders);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Slider slider)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (!slider.Photo.CheckFileType("image/"))
            {

                ModelState.AddModelError("Photo", "File type must be image");
                return View();

            }


            if (slider.Photo.CheckFileSize(200))
            {

                ModelState.AddModelError("Photo", "Photo size must be max 200Kb");
                return View();

            }


            //Bize gelen slaydin adinin qarsina uniqe bir ad qoyur !
            string fileName = Guid.NewGuid().ToString() + "_" + slider.Photo.FileName;

            //Root'a el catanliq yaratmaq ucun WebRootPath yaziriq ,"img" bu hansi folderde olacaq,fileName - bu ise faylin
            //adi olacaq !Path bize gelen faylin hara qoyulacagini gosterir !

            //string path = Path.Combine(_env.WebRootPath, "img", fileName);

            string path = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

            using(FileStream stream = new(path,FileMode.Create))
            {
                await slider.Photo.CopyToAsync(stream);
            }

            slider.Image = fileName;

            await _context.Sliders.AddAsync(slider);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {

            if (id == null) return BadRequest();

            Slider slider = await _context.Sliders.FirstOrDefaultAsync(m => m.Id == id);

            if (slider == null) return NotFound();


            string path = FileHelper.GetFilePath(_env.WebRootPath, "img", slider.Image);


            FileHelper.DeleteFile(path);

            _context.Sliders.Remove(slider);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));



        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null) return BadRequest();

            var slider = _context.Sliders.FirstOrDefault(m => m.Id == id);

            if (slider == null) return NotFound();

            return View(slider);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id,Slider newSlider)
        {
            if (id == null) return BadRequest();

            Slider slider = await _context.Sliders.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

            if (slider == null) return NotFound();

			if (!ModelState.IsValid)
			{
				return View(slider);
			}

			if (!newSlider.Photo.CheckFileType("image/"))
			{
				ModelState.AddModelError("Photo", "File type must be image");
				return View(slider);
			}


			if (newSlider.Photo.CheckFileSize(200))
			{
				ModelState.AddModelError("Photo", "Photo size must be max 200Kb");
				return View(slider);
            }

			string fileName = Guid.NewGuid().ToString() + "_" + newSlider.Photo.FileName;

			string path = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

			using (FileStream stream = new(path, FileMode.Create))
			{
				await newSlider.Photo.CopyToAsync(stream);
			}

			string expath = FileHelper.GetFilePath(_env.WebRootPath, "img", slider.Image);

			FileHelper.DeleteFile(expath);

			newSlider.Image = fileName;

			_context.Sliders.Update(newSlider);

			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));

		}


		[HttpGet]
		public async Task<IActionResult> Detail(int? id)
		{
			if (id == null) return BadRequest();

			Slider slider = await _context.Sliders.FirstOrDefaultAsync(m => m.Id == id);

			if (slider == null) return NotFound();

			return View(slider);

		}
	}
}
