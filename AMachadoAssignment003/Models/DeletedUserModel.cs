using System.ComponentModel.DataAnnotations;

namespace AMachadoAssignment003.Models
{
    public class DeletedUserModel
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "First Name is a required field")]
        [RegularExpression(@"^[A-Za-z]{3,10}$", ErrorMessage = "First Name must be 3-10 letters")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is a required field")]
        [RegularExpression(@"^[A-Za-z]{3,10}$", ErrorMessage = "Last Name must be 3-10 letters")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Please select a department")]
        public string Department { get; set; }
        [Required(ErrorMessage = "Please select a position")]
        public string Position { get; set; }
        [Required(ErrorMessage = "Years Of Experience must be filled")]
        [Range(0, 30, ErrorMessage = "Years of Experience must be from 0-30")]
        public int YearsOfExperience { get; set; }
        [Required(ErrorMessage = "Salary must be filled")]
        [Range(1, 100000, ErrorMessage = "Salary must not exceed $100,000")]
        public double Salary { get; set; }

        public Boolean IsDeleted { get; set; }

        public String DeletionReason { get; set; }
    }
}
