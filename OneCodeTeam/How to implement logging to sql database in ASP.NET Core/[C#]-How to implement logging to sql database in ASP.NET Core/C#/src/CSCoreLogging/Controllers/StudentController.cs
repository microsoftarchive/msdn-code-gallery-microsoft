using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CSCoreLogging.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using CSCoreLogging.LogProvider;

namespace CSCoreLogging.Controllers
{
    public class StudentController : Controller
    {
        private CustomLoggerDBContext _context;
        private readonly ILogger<StudentController> _logger;

        public StudentController( ILogger<StudentController> logger, CustomLoggerDBContext context)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var StudentList = _context.Student.ToList();
            _logger.LogInformation((int)LoggingEvents.GENERATE_ITEMS, "Show student list.");
            return View(StudentList);
        }

        [HttpGet]
        public IActionResult StudentDetail(int id)
        {
            var entity = _context.Student.FirstOrDefault(m => m.Id == id);
            _logger.LogInformation((int)LoggingEvents.GET_ITEM, $"Get student by id:{id}.");
            return View(entity);
        }

        [HttpPost]
        public IActionResult StudentDetail(Models.Student entity)
        {
            _context.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            _logger.LogInformation((int)LoggingEvents.UPDATE_ITEM, $"Student {entity.Id} updated.");
            return View(entity);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(Models.Student studentEntity)
        {
           var student =  _context.Student.Add(studentEntity);
            _context.SaveChanges();
            _logger.LogInformation((int)LoggingEvents.INSERT_ITEM, $"Student {student.Entity.Id}  created.");
            return Redirect($"./StudentDetail/{studentEntity.Id}");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var entity = _context.Student.FirstOrDefault(m => m.Id == id);
            _context.Student.Remove(entity);
            _context.SaveChanges();
            _logger.LogInformation((int)LoggingEvents.DELETE_ITEM, $"Student {id}  deleted.");
            return Redirect("/");
        }
    }
}
