using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace ToDoListProject.Models
{
    public class ToDo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter a description.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a due date.")]
        public DateTime DueDate { get; set; }


        //Foreign Key Property
        [Required(ErrorMessage = "Please enter a category.")]
        public string CategoryId { get; set; } = string.Empty;
        //connecting the todo to the category table
        [ValidateNever]
        public Category Category { get; set; } = null!;


        //Foreign Key Property
        [Required(ErrorMessage = "Please enter a status.")]
        public string StatusId { get; set; } = string.Empty;
        //connecting the todo to the status table
        [ValidateNever]
        public Status Status { get; set; } = null!;


        public bool Overdue => StatusId == "open" && DueDate < DateTime.Now;
    }
}
