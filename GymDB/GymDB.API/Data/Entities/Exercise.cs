﻿using GymDB.API.Data.ValidationAttributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymDB.API.Data.Entities
{
    public class Exercise
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [StringLength(130, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 130 characters long!")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Instructions must be up to 500 characters long!")]
        public string Instructions { get; set; }

        [StringLength(100, ErrorMessage = "MuscleGroups must be up to 100 characters long!")]
        public string MuscleGroups { get; set; }

        [ExerciseType]
        [StringLength(30)]
        public string Type { get; set; }

        [ExerciseDifficulty]
        [StringLength(12)]
        public string Difficulty { get; set; }

        [StringLength(100, ErrorMessage = "Equipment must be up to 100 characters long!")]
        public string? Equipment { get; set; }

        public bool IsCustom { get; set; }

        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        public User User { get; set; }
    }
}