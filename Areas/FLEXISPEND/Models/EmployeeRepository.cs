using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IEM.Areas.FLEXISPEND.Models
{
    public interface EmployeeRepository
    {
        EmployeeMaster_Model GetEmployeeDetailbyid(int id);
    }
}