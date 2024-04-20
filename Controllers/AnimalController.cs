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
            new Animal { IdAnimal = 1, Name = "Leopard", Description = "Leopards are agile predators.", Category = "Mammal", Area = "Various" },
            new Animal { IdAnimal = 2, Name = "Gorilla", Description = "Gorillas are the largest living primates.", Category = "Mammal", Area = "Africa" },
            new Animal { IdAnimal = 3, Name = "Koala", Description = null, Category = "Mammal", Area = "Australia" },
            new Animal { IdAnimal = 4, Name = "Lizard", Description = "Lizards are reptiles with long bodies and tails.", Category = "Reptile", Area = "Various" },
            new Animal { IdAnimal = 5, Name = "Ostrich", Description = "Ostriches are flightless birds with long necks.", Category = "Bird", Area = "Africa" },
            new Animal { IdAnimal = 6, Name = "Shark", Description = "Sharks are cartilaginous fish known for their sharp teeth.", Category = "Fish", Area = "Ocean" },
            new Animal { IdAnimal = 7, Name = "Polar Bear", Description = "Polar bears are marine mammals native to the Arctic.", Category = "Mammal", Area = "Arctic" },
            new Animal { IdAnimal = 8, Name = "Gazelle", Description = "Gazelles are swift and graceful antelopes.", Category = "Mammal", Area = "Africa" },
            new Animal { IdAnimal = 9, Name = "Hippopotamus", Description = "Hippos are large herbivorous mammals.", Category = "Mammal", Area = "Africa" },
            new Animal { IdAnimal = 10, Name = "Zebra", Description = null, Category = "Mammal", Area = "Africa" }
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
