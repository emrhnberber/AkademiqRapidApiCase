using System.Collections.Generic;

namespace AkademiqRapidApiCase.Models
{
    public class RecipeResponse
    {
        public List<Meal> meals { get; set; }
    }

    public class Meal
    {
        public string strMeal { get; set; }
        public string strCategory { get; set; }
        public string strArea { get; set; }
        public string strInstructions { get; set; }
        public string strMealThumb { get; set; }
    }
} 