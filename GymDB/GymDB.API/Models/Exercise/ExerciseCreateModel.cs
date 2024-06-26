﻿using System.ComponentModel.DataAnnotations;
using GymDB.API.Data.Enums;

namespace GymDB.API.Models.Exercise
{
    public class ExerciseCreateModel
    {
        [StringLength(70, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 70 characters long!")]
        public string Name { get; set; }

        [StringLength(250, MinimumLength = 1, ErrorMessage = "Instructions must be between 1 and 250 characters long!")]
        public string Instructions { get; set; }

        public string MuscleGroups { get; set; }

        public ExerciseType Type { get; set; }

        public ExerciseDifficulty Difficulty { get; set; }

        public string? Equipment { get; set; }

        public ExerciseVisibility Visibility { get; set; }

        public List<IFormFile>? Images { get; set; }
    }
}
