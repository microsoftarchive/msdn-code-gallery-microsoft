using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using CSUsingMongoDBInNETCore.Models;

namespace CSUsingMongoDBInNETCore.Controllers
{
    public class PostController : Controller
    {
        public IActionResult Index()
        {
            MongoDBContext dbContext = new MongoDBContext();

            List<Post> postList = dbContext.Posts.Find(m => true).ToList();

            return View(postList);
        }

        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            MongoDBContext dbContext = new MongoDBContext();
            var entity = dbContext.Posts.Find(m => m.Id == id).FirstOrDefault();

            return View(entity);
        }

        [HttpPost]
        public IActionResult Edit(Post entity)
        {
            MongoDBContext dbContext = new MongoDBContext();

            //you can use the UpdateOne to get higher performance if you need.
            dbContext.Posts.ReplaceOne(m => m.Id == entity.Id, entity);

            return View(entity);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(Post entity)
        {
            MongoDBContext dbContext = new MongoDBContext();

            entity.Id = Guid.NewGuid();

            dbContext.Posts.InsertOne(entity);

            return Redirect("/");
        }

        [HttpGet]
        public IActionResult Delete(Guid id)
        {
            MongoDBContext dbContext = new MongoDBContext();

            dbContext.Posts.DeleteOne(m => m.Id == id);

            return Redirect("/");
        }
    }
}
