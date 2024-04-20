using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using REST.Models;

namespace REST.Controllers
{
    [Route("api/animals")]
    [ApiController]
    public class AnimalController : ControllerBase
    {
        private static readonly List<Animal> _animals = new()
        {
            new Animal { IdAnimal = 1, Name = "Kuba", Description = "abc", Category = "Ssak", Area = "Warszawskie zoo"},
            new Animal { IdAnimal = 2, Name = "Przemek", Description = "acb", Category = "Plaz", Area = "Warszawskie zoo"},
            new Animal { IdAnimal = 3, Name = "AATomek", Description = "bca", Category = "Ryba", Area = "Warszawskie zoo"},
        };

        [HttpGet]
        public IActionResult GetAnimals([FromQuery] string orderBy = "name")
        {
            var sortedAnimals = SortAnimals(orderBy);
            return Ok(sortedAnimals);
        }

        [HttpPost]
        public IActionResult CreateAnimal(Animal animal)
        {
            _animals.Add(animal);
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateAnimal(int id, Animal animal)
        {
            var animalToEdit = _animals.FirstOrDefault(an => an.IdAnimal == id);

            if (animalToEdit == null)
            {
                return NotFound($"Animal with id {id} was not found");
            }

            _animals.Remove(animalToEdit);
            _animals.Add(animal);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteAnimal(int id)
        {
            var animalToDelete = _animals.FirstOrDefault(an => an.IdAnimal == id);
            if (animalToDelete == null)
            {
                return NotFound($"Animal with id {id} was not found");
            }

            _animals.Remove(animalToDelete);
            return NoContent();
        }

        private IEnumerable<Animal> SortAnimals(string orderBy)
        {
            return orderBy switch
            {
                "name" => _animals.OrderBy(a => a.Name),
                "description" => _animals.OrderBy(a => a.Description),
                "category" => _animals.OrderBy(a => a.Category),
                "area" => _animals.OrderBy(a => a.Area),
                _ => throw new ArgumentException("Invalid orderBy parameter"),
            };
        }
    }
}
