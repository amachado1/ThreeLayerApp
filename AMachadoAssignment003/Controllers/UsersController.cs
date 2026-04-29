using AMachadoAssignment003.Data;
using AMachadoAssignment003.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace AMachadoAssignment003.Controllers
{
    public class UsersController : Controller
    {
        public readonly UsersDbContext _context; //DbContext

        //Salary budget limits
        public const double totalBudget = 1000000;
        public const double ITBudget = 500000;
        public const double BusinessBudget = 500000;

        public UsersController(UsersDbContext _context)
        {
            this._context = _context;
        }

        //Create a user list view that displays a list of all users
        public async Task<IActionResult> UserList()
        {
            var Users = await _context.Users.ToListAsync();
            return View(Users);
        }

        //Create a IT Department list view that displays all users that belong to the IT Department
        public async Task<IActionResult> ITList()
        {
            var ITUsers = await _context.Users.Where(u => u.Department == "IT").ToListAsync();
            return View(ITUsers);
        }

        //Create a Business Department list view that displays all users that belong to the Business Department
        public async Task<IActionResult> BusinessList()
        {
            var BusinessUsers = await _context.Users.Where(u => u.Department == "Business").ToListAsync();
            return View(BusinessUsers);
        }

        //Create a Deleted User list view that displays all users that have been deleted from the lists
        public async Task<IActionResult> DeletedList()
        {
            var DeletedUsers = await _context.DeletedUsers.ToListAsync();
            return View(DeletedUsers);
        }

        //Returns the Create User view
        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        //Create user view that creates a user based on the user model
        [HttpPost]
        public async Task<IActionResult> CreateUser(UserModel user)
        {
            //Checks if the model state is valid
            if (ModelState.IsValid)
            {
                var userCount = await _context.Users.CountAsync();
                double totalSpending = await _context.Users.SumAsync(u => u.Salary);
                double deptSpending = await _context.Users.Where(u => u.Department == user.Department).SumAsync(u => u.Salary);

                //Limits the list to have only 10 users max
                if (userCount >= 10 || totalSpending + user.Salary > totalBudget)
                {
                    return View("CreateUser", user);
                }

                //Checks to see if the IT Department does not exceed the budget of $500,000 for their department
                if (user.Department == "IT") 
                {
                    if (deptSpending + user.Salary > ITBudget) 
                    {
                        return View("CreateUser", user);
                    }
                }

                //Checks to see if the Business Department does not exceed the budget of $500,000 for their department
                else if (user.Department == "Business")
                {
                    if (deptSpending + user.Salary > BusinessBudget)
                    {
                        return View("CreateUser", user);
                    }
                }

                var newUser = new UserModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Department = user.Department,
                    Position = user.Position,
                    YearsOfExperience = user.YearsOfExperience,
                    Salary = user.Salary
                };

                //Adds user to the list
                await _context.Users.AddAsync(newUser);
                await _context.SaveChangesAsync();

                return RedirectToAction("UserList");
            }

            return View("CreateUser", user);
        }

        //Create a details view that shows the details of a specific user by their ID
        public async Task<IActionResult> Details(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.ID == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        //Edit method for giving options to change a user's years of experience or their salary
        public async Task<IActionResult> EditOptions(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.ID == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        //Return the EditYearsOfExperience view of a specific user by their ID
        [HttpGet]
        public async Task<IActionResult> EditYearsOfExperience(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.ID == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        //Method edits only a user's years of experience
        [HttpPost]
        public async Task<IActionResult> EditYearsOfExperience(UserModel editedUser)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.ID == editedUser.ID);

            if (ModelState.IsValid)
            {
                if (existingUser == null)
                {
                    return NotFound();
                }

                existingUser.YearsOfExperience = editedUser.YearsOfExperience;
                await _context.SaveChangesAsync();

                if (existingUser.Department == "IT")
                {
                    return RedirectToAction("ITList");
                }
                else if (existingUser.Department == "Business")
                {
                    return RedirectToAction("BusinessList");
                }
            }

            return View(editedUser);
        }

        //Return the EditSalary view of a specific user by their ID
        [HttpGet]
        public async Task<IActionResult> EditSalary(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.ID == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        //Method edits only a user's salary while adhering to the list's added limitations in budget
        [HttpPost]
        public async Task<IActionResult> EditSalary(UserModel editedUser)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.ID == editedUser.ID);

            if (ModelState.IsValid)
            {
                if (existingUser == null)
                {
                    return NotFound();
                }

                //Make sure the spending and budget in total and for each department is updated in accordance to the user's updated salary
                double existingSalary = existingUser.Salary;
                double editedSalary = editedUser.Salary;

                double newSalary = editedSalary - existingSalary;
                double currentTotal = await _context.Users.SumAsync(u => u.Salary);
                double currentDeptTotal = await _context.Users.Where(u => u.Department == existingUser.Department).SumAsync(u => u.Salary);

                if (currentTotal + newSalary > totalBudget)
                {
                    return View(editedUser);
                }

                if (existingUser.Department == "IT" && currentDeptTotal + newSalary > ITBudget)
                {
                    return View(editedUser);
                }

                if (existingUser.Department == "Business" && currentDeptTotal + newSalary > BusinessBudget)
                {
                    return View(editedUser);
                }

                existingUser.Salary = editedSalary;
                await _context.SaveChangesAsync();

                if (existingUser.Department == "IT")
                {
                    return RedirectToAction("ITList");
                }

                else if (existingUser.Department == "Business")
                {
                    return RedirectToAction("BusinessList");
                }
            }     

            return View(editedUser);
        }

        //Returns delete view to delete a specific user by ID
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.ID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        //Method that deletes a user from all respective lists, re-calculates budget and spending to keep total and department budget consistency, and adds the user to the deleted user list alongside a reason for deletion comment
        [HttpPost]
        public async Task<IActionResult> Delete(UserModel deletedUser, string deleteComment)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.ID == deletedUser.ID);

            if (existingUser == null)
            {
                return NotFound();
            }

            var delUser = new DeletedUserModel
            {
                FirstName = existingUser.FirstName,
                LastName = existingUser.LastName,
                Department = existingUser.Department,
                Position = existingUser.Position,
                YearsOfExperience = existingUser.YearsOfExperience,
                Salary = existingUser.Salary,
                IsDeleted = true,
                DeletionReason = deleteComment
            };

            await _context.DeletedUsers.AddAsync(delUser);
            _context.Users.Remove(existingUser);

            await _context.SaveChangesAsync();

            //Recalculates the spending budget for IT to keep up with the total and IT department budget set in place
            //It also deletes the user from both the IT department list and general user list
            if (existingUser.Department == "IT")
            {
                return RedirectToAction("ITList");
            }

            //Recalculates the spending budget for Business to keep up with the total and Business department budget set in place
            //It also deletes the user from both the Business department list and general user list
            else if (existingUser.Department == "Business")
            {
                return RedirectToAction("BusinessList");
            }

            return View(deletedUser);
        }
    }
}
