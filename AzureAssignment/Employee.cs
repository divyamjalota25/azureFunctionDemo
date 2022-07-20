using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureAssignment
{
    internal class EmployeeList
    { 
        public List<Employee> data { get; set; }
    }
    
    internal class Employee
    {
        public string EMPLOYEE_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public int DEPARTMENT_ID { get; set; }
        public int SALARY { get; set; }
    }
}
