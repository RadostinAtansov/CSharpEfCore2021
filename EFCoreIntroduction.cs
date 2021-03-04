using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SoftUni.Data;
using SoftUni.Models;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            var softUniContex = new SoftUniContext();
            var result = RemoveTown(softUniContex);
            Console.WriteLine(result);
        }

        //--------------------------------------------------15--------------------------------------------------

        public static string RemoveTown(SoftUniContext context)
        {
            var town = context.Towns
                .Include(x => x.Addresses)
                .FirstOrDefault(x => x.Name == "Seattle");

            var allAddressIds = town.Addresses
                .Select(x => x.AddressId)
                .ToList();

            var employees = context.Employees
                .Where(x => x.AddressId.HasValue && allAddressIds.Contains(x.AddressId.Value))
                .ToList();

            foreach (var employee in employees)
            {
                employee.AddressId = null;
            }

            foreach (var addressId in allAddressIds)
            {
                var address = context.Addresses
                    .FirstOrDefault(x => x.AddressId == addressId);
                context.Addresses.Remove(address);
            }

            context.SaveChanges();

            context.Towns.Remove(town);

            context.SaveChanges();

            return $"{allAddressIds.Count} addresses in Seattle deleted";

        }

        //--------------------------------------------------14--------------------------------------------------

        public static string DeleteProjectById(SoftUniContext context)
        {


            var projectToDelete = context.Projects
                .FirstOrDefault(p => p.ProjectId == 2);

            var employeeProjectsToDelete = context.EmployeesProjects
                .Where(ep => ep.ProjectId == 2)
                .ToList();

            foreach (var employeeProject in employeeProjectsToDelete)
            {
                context.EmployeesProjects.Remove(employeeProject);
            }

            context.Projects.Remove(projectToDelete);

            context.SaveChanges();

            var projects = context.Projects
                .Select(p => p.Name)
                .Take(10)
                .ToList();

            var sb = new StringBuilder();

            foreach (var project in projects)
            {
                sb.AppendLine($"{project}");
            }

            return sb.ToString().TrimEnd();

            /*Let's delete the project with id 2. Then, take 10 projects and return their names, each on a new line. 
             * Remember to restore your database after this task.
             */
        }

        //--------------------------------------------------13--------------------------------------------------

        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var employee = context.Employees
                .Where(x => EF.Functions.Like(x.FirstName, "Sa%"))
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToList();

            var sb = new StringBuilder();

            foreach (var emp in employee)
            {
                sb.AppendLine($"{emp.FirstName} {emp.LastName} - {emp.JobTitle} - (${emp.Salary:F2})");
            }

            return sb.ToString().TrimEnd();
        }
        

        //--------------------------------------------------12--------------------------------------------------

        public static string IncreaseSalaries(SoftUniContext context)
        {

            string[] departments = new string[]{"Engineering", "Tool Design", "Marketing", "Information Services"};

            var employees = context.Employees
                .Where(x => departments.Contains(x.Department.Name))
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToList();

            foreach (var employee in employees)
            {
                employee.Salary *= 1.12m;
            }

            context.SaveChanges();

            var sb = new StringBuilder();

            foreach (var emp in employees)
            {
                sb.AppendLine($"{emp.FirstName} {emp.LastName} (${emp.Salary:F2})");
            }

            return sb.ToString().TrimEnd();
        }

        //--------------------------------------------------11--------------------------------------------------

        public static string GetLatestProjects(SoftUniContext context)
        {
            var last10Project = context.Projects
                .OrderByDescending(x => x.StartDate.Year)
                .OrderBy(x => x.Name)
                .Select(x => new 
                {
                    x.Name,
                    x.Description,
                    x.StartDate
                })
                .Take(10)
                .ToList();

            var sb = new StringBuilder();

            foreach (var projects in last10Project)
            {
                sb.AppendLine($"{projects.Name}\n{projects.Description}\n{projects.StartDate.ToString("M/d/yyyy h:mm:ss tt")}");
            }

            return sb.ToString().TrimEnd();
        }

        //--------------------------------------------------10--------------------------------------------------

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departments = context.Departments
                .Where(x => x.Employees.Count > 5)
                .OrderBy(x => x.Employees.Count)
                .ThenBy(x => x.Name)
                .Select(x => new
                {
                    x.Name,
                    x.Manager.FirstName,
                    x.Manager.LastName,
                    Employee = x.Employees.Select(x => new
                    {
                        x.FirstName,
                        x.LastName,
                        x.JobTitle
                    })
                    .OrderBy(x => x.FirstName)
                    .ThenBy(x => x.LastName)
                    .ToList()
                })
                .ToList();

            var sb = new StringBuilder();

            foreach (var departs in departments)
            {
                sb.AppendLine($"{departs.Name} {departs.FirstName} - {departs.LastName}");

                foreach (var emplpyee in departs.Employee)
                {
                    sb.AppendLine($"{emplpyee.FirstName} {emplpyee.LastName} - {emplpyee.JobTitle}");
                }
            }

            return sb.ToString().Trim();

        }

        //--------------------------------------------------9--------------------------------------------------

        public static string GetEmployee147(SoftUniContext context)
        {
            var employee147 = context.Employees
                .Select(x => new
                {
                    EmployeeId1 = x.EmployeeId,
                    EmployeeFirstName = x.FirstName,
                    EmployeeLastName = x.LastName,
                    EmployeeJob = x.JobTitle,
                    EmployeeProjects = x.EmployeesProjects.Select(x => new EmployeeProject
                    {
                        Project = x.Project
                    })
                    .OrderBy(x => x.Project.Name)
                    .ToList()

                })
                .FirstOrDefault(x => x.EmployeeId1 == 147);


            var sb = new StringBuilder();

            sb.AppendLine($"{employee147.EmployeeFirstName} {employee147.EmployeeLastName} {employee147.EmployeeJob}");

            foreach (var ep in employee147.EmployeeProjects)
            {
                sb.AppendLine($"{ep.Project.Name}");
            }

            return sb.ToString().TrimEnd();
        }

        //--------------------------------------------------8--------------------------------------------------

        public static string GetAddressesByTown(SoftUniContext context)
        {
            var adresses = context.Addresses
                .Select(x => new
                {
                    x.AddressText,
                    x.Town.Name,
                    x.Employees.Count
                })
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.Name)
                .OrderBy(x => x.AddressText)
                .Take(10)
                .ToList();

            var sb = new StringBuilder();

            foreach (var ad in adresses)
            {
                sb.AppendLine($"{ad.AddressText}, {ad.Name} - {ad.Count}");
            }

            return sb.ToString().TrimEnd();
                
        }

        //--------------------------------------------------7--------------------------------------------------

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var employeeWithProjectDate = context.Employees
                .Include(x => x.EmployeesProjects)
                .ThenInclude(x => x.Project)
                .Where(x => x.EmployeesProjects.Any(x => x.Project.StartDate.Year >= 2001 &&
                                                         x.Project.StartDate.Year <= 2003))
                .Select(x => new 
                {
                    EmployeeFirstName = x.FirstName,
                    EmployeeLastName = x.LastName,
                    ManagerFirstName = x.Manager.FirstName,
                    ManagerLastName = x.Manager.LastName,
                    Projects = x.EmployeesProjects.Select(p => new 
                    {
                       ProjectName = p.Project.Name,
                       StartDate = p.Project.StartDate,
                       EndDate = p.Project.EndDate
                    })

                })
                .Take(10)
                .ToList();

            var sb = new StringBuilder();

            foreach (var ep in employeeWithProjectDate)
            {
                sb.AppendLine($"{ep.EmployeeFirstName} {ep.EmployeeLastName} - Manager: {ep.ManagerFirstName} {ep.ManagerLastName}");

                foreach (var projects in ep.Projects)
                {

                    var endDate = projects.EndDate.HasValue
                       ? projects.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
                       : "not finished";

                    sb.AppendLine($"--{projects.ProjectName} - {projects.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)} - {endDate}");


                    //if (projects.EndDate.HasValue)
                    //{
                    //    sb.AppendLine($"{projects.ProjectName} {projects.StartDate} " +
                    //        $"{projects.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)}");
                    //}
                    //else
                    //{
                    //    sb.AppendLine($"{projects.ProjectName} {projects.StartDate} - not finished  ");
                    //}
                }
            }

            return sb.ToString().TrimEnd();
        }

        //--------------------------------------------------6--------------------------------------------------

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            var address = new Address
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            context.Addresses.Add(address);
            context.SaveChanges();

            var nakov = context.Employees
                .FirstOrDefault(x => x.LastName == "Nakov");

            nakov.AddressId = address.AddressId;

            context.SaveChanges();

            var employes = context.Employees
                .Select(x => new
                {
                    x.Address.AddressText,
                    x.Address.AddressId
                })
                .OrderByDescending(x => x.AddressId)
                .Take(10)
                .ToList();

            var sb = new StringBuilder();

            foreach (var add in employes)
            {
                sb.AppendLine(add.AddressText);
            }

            return sb.ToString().TrimEnd();
        }

        //--------------------------------------------------5--------------------------------------------------

        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var researchAndDevelopment = context.Employees
                .Where(x => x.Department.Name == "Research and Development")
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    x.Salary,
                    DepartmentName = x.Department.Name,
                })
                .OrderBy(x => x.Salary)
                .ThenByDescending(x => x.FirstName)
                .ToList();

            var sb = new StringBuilder();

            foreach (var employee in researchAndDevelopment)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} from {employee.DepartmentName} - ${employee.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        //--------------------------------------------------4--------------------------------------------------

        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employees = context.Employees
                .Select(x => new
                {
                    x.FirstName,
                    x.Salary
                })
                .Where(x => x.Salary > 50000)
                .OrderBy(x => x.FirstName)
                .ToList();

            var sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} - {employee.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        //--------------------------------------------------3--------------------------------------------------

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employees = context.Employees
                .Select(x => new
                {
                    x.EmployeeId,
                    x.FirstName,
                    x.LastName,
                    x.MiddleName,
                    x.JobTitle,
                    x.Salary
                })
                .OrderBy(x => x.EmployeeId)
                .ToList();

            var sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

    }
}
